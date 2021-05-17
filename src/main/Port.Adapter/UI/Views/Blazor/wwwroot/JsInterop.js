window.jsFunctions = {
    getFieldValue: function (fieldName) {
        return document.getElementById(fieldName).value;
    }
}

window.PlaySound = function () {
    document.getElementById('sound').play();
}