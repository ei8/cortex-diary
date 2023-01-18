// TODO: transfer following to tree plugin? if so, check how to include notifications-sw.js
// or transfer to blazor.common instead as it can be reused in different plugins

var dotNetHelper = null;
window.DotNetHelperSetter = (dotHelper) => {
    dotNetHelper = dotHelper;
};

// tree
// -- Subscriptions JS interop
window.RegisterServiceWorker = function(dotnet) {
    if (!('PushManager' in window)) {
        console.log('Push notifications are not supported in this browser.');
        return;
    }

    Notification.requestPermission().then(function (status) {
        console.log('Status: ' + status);
        dotnet.invokeMethodAsync('SetPermissionStatus', status);

        if (status === 'denied') {
            console.log('User denied notification permissions');
            return;
        }
        else if (status === 'granted') {
            if ('serviceWorker' in navigator) {
                navigator.serviceWorker.register('./notifications-sw.js').then(function (registration) {
                    if (registration.installing) {
                        console.log('Service worker installing');
                    } else if (registration.waiting) {
                        console.log('Service worker installed');
                    } else if (registration.active) {
                        console.log('Service worker active');
                    }
                });
            } else {
                console.log('Browser does not support service workers. Push notifications may not work.');
            }
        }
    });
}

// tree
window.Subscribe = function (dotnet, applicationServerPublicKey) {
    if (navigator.serviceWorker) {
        navigator.serviceWorker.ready.then(function (reg) {
            if (reg.active) {
                dotnet.invokeMethodAsync('SetServiceWorkerStatus', true);

                const subscribeParams = { userVisibleOnly: true };

                //Setting the public key of our VAPID key pair.
                const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
                subscribeParams.applicationServerKey = applicationServerKey;

                if (!(reg.showNotification)) {
                    console.log('Browser does not support off-site push notifications.');
                } else {
                    dotnet.invokeMethodAsync('SetSupportState', true);

                    reg.pushManager.subscribe(subscribeParams)
                        .then(function (subscription) {
                            const p256dh = base64Encode(subscription.getKey('p256dh'));
                            const auth = base64Encode(subscription.getKey('auth'));

                            console.log(subscription);

                            console.log(subscription.endpoint);
                            console.log(p256dh);
                            console.log(auth);

                            dotnet.invokeMethodAsync('SetDeviceProperties', detectBrowser(), p256dh, auth, subscription.endpoint);
                        })
                        .catch(function (e) {
                            console.log('[subscribe] Unable to subscribe to push', e);
                        });
                }
            }
        });
    }
}

// tree
function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

// tree
function base64Encode(arrayBuffer) {
    return btoa(String.fromCharCode.apply(null, new Uint8Array(arrayBuffer)));
}

// tree
function detectBrowser() {
    const agent = navigator.userAgent
    if ((agent.indexOf("Opera") || agent.indexOf('OPR')) != -1) {
        return 'Opera';
    } else if (agent.indexOf("Edge") != -1) {
        return 'MS Edge';
    } else if (agent.indexOf("Edg") != -1) {
        return 'Chromium Edge';
    } else if (agent.indexOf("Chrome") != -1) {
        return 'Chrome';
    } else if (agent.indexOf("Safari") != -1) {
        return 'Safari';
    } else if (agent.indexOf("Firefox") != -1) {
        return 'Firefox';
    } else if ((agent.indexOf("MSIE") != -1) || (!!document.documentMode == true)) {
        return 'IE';
    } else {
        return 'Unknown';
    }
}