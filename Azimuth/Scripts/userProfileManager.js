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

    $('#followersBtn').hover(function() {
        $(this).data('focused', 'true');
        //$('#popup').css({ 'display': 'block' }).stop().animate({ left: -130, opacity: 1 }, 300); // анимация появления блока

        $('#popup').css({
            'top': event.clientX,
            'left': event.clientY
        });





        $(this).data('focused', 'false'); //устанавливаем для ссылки атрибут data-focused = "false"
        $('#popup').stop().animate({ bottom: '50px', opacity: 0 }, 200,
         function () {
             $('#popup').css({ 'display': 'none' });
         }); //анимация скрытия блока, когда курсор выйдет за пределы ссылки
    });
};
