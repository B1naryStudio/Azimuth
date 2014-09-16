$(document).ready(function () {
    this.manager = new UserProfileManager();

    this.manager.bindListeners();
    this.manager.showNotification();
});