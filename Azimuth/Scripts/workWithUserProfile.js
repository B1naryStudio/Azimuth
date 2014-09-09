$(document).ready(function () {
    var manager = new UserProfileManager();

    manager.bindListeners();
    manager.showNotification();
});