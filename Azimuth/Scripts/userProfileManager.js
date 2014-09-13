var UserProfileManager = function () {
    this.self = this;
    this.$loggedUser = $('#user-id');
    this.$notificationTemplate = $('#notificationTemplate');
    this.$notificationContainer = $('#notification-container');
    this.$recentActivity = $('#recent-activity');
};


UserProfileManager.prototype.showNotification = function () {
    var self = this;
    var userId = self.$loggedUser.val();
    $.ajax({
        url: '/api/notifications/' + userId,
        async: true,
        success: function (notifications) {
            console.log(notifications);

            for (var i = 0; i < notifications.length; i++) {
                var item = self.$notificationTemplate.tmpl(notifications[i]);
                self.$recentActivity.append(item);
            }
        }
    });
};

UserProfileManager.prototype.bindListeners = function () {
    var self = this;

    self.$notificationContainer.mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        scrollInertia: 0,
        autoHideScrollbar: true,
        advanced: { updateOnSelectorChange: "true" }
    });
};
