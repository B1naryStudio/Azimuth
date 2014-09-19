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

    $('#followersBtn, #followedBtn').off('hover').hover(function (event) {

        if ($('#popup').is(":visible")) {
            //self._hidePopup(event);
            $('#popup').children('.data').empty();
        }

        self._showPopup($(this));
    });

    $('#followersBtn, #popup, #followedBtn').mouseleave(function (event) {
        setTimeout(function() {
            if (!$('#followersBtn').is(':hover') && !$('#followedBtn').is(':hover') && !$('#popup').is(':hover')) {
                self._hidePopup(event);
            }
        }, 200);
    });

    $('#followedBtn, #followersBtn, #popup').click(function (event) {
        var $target = $(event.target);
        if ($target.is('img'))
            return;
        var $followPeople;
        if ($target.attr('id') == "popup") {
            $followPeople = $target.find('.topPeople');
        }
        else {
            $followPeople = $target.parent().find('.topPeople');
        }
        if ($followPeople.length == 0)
            return;
        var $popupModal = $('#popupModal');
        $popupModal.find('.modal-header').append('<h3>' + $(event.target).text()  +'</h3>');
        $popupModal.find('.modal-body .users').append($followPeople.clone());
        $popupModal.find('.topPeople .userName').show();
        $popupModal.find('.topPeople').show();
        $popupModal.modal('show');
    });

    $('#popupModal').on('hidden.bs.modal', function () {
        $('#popupModal .modal-body .users').empty();
        $('#popupModal .modal-header').empty();
    });
    
    this._showPopup = function(element) {
        var $popup = $('#popup');
        if ($popup.find('.topPeople').length > 0)
            return;
        var $self = element;
        var $area = $self.parent();
        var $top = $area.find(".topPeople").slice(0, 6);
        if ($top.length == 0)
            return;
        var $btnPos = element.offset();
        $popup.children('.data').append($top.clone());
        $popup.find(".topPeople").show();

        $popup.css({
            'top': $btnPos.top - $popup.height() - 5,
            'left': $btnPos.left - 5
        }).animate({ bottom: '50px', opacity: 1 }, 600,
            function() {
                $('#popup').css({ 'display': 'block' });
            });
    };

    this._hidePopup = function(event) {
        var $toElement = $(event.toElement);
        if ($toElement.attr('id') == "popup")
            return;
        var $popup = $('#popup');
        $('#popup').stop().animate({ bottom: '50px', opacity: 0 }, 300,
         function () {
             $('#popup').css({ 'display': 'none' });
         });
        $popup.children('.data').empty();
    };
};
