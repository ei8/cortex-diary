self.addEventListener('push', function (event) {
    if (!(self.Notification && self.Notification.permission === 'granted')) {
        return;
    }

    let data = {};

    if (event.data) {
        data = event.data.json();
    }

    event.waitUntil(self.registration.showNotification(data.title, data.options));
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    const tag = event.notification.tag;
    const targetUrl = `/landing/${tag}`;

    event.waitUntil(clients.matchAll({
        type: "window"
    }).then(function (clientList) {
        // open a new window/tab to the target URL
        if (clients.openWindow)
            return clients.openWindow(targetUrl);
    }));
});