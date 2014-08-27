var AudioManager = function () {
    var self = this;
    this.audio = new Audio();
    this.tracksGlobal = [];
    var volume = new Dragdealer('volume', {
        vertical: true,
        horizontal: false,
        bottom: 4,
        animationCallback: function (x, y) {
            var t = Math.round(y * 100);
            if (y >= 0 && y <= 1) {
                self.audio.volume = 1 - y;
            }
            if (t <= 33) {
                $('#volumeImg').css('background-position', '0 0');
            } else if (t <= 66) {
                $('#volumeImg').css('background-position', '0 -40px');
            } else if (t === 100) {
                $('#volumeImg').css('background-position', '0 -120px');
            } else {
                $('#volumeImg').css('background-position', '0 -79px');
            }
        }
    });
    volume.setValue(0, 0.5);
}

AudioManager.prototype.bindPlayBtnListeners = function () {
    var self = this;

    $('.track-play-btn').click(function () {
        self.tracksGlobal = $(this).parent().parent().children('.vk-item').children('.track-url');
        var url = $(this).parent().children('.track-url').text();
        if (self.audio.paused || self.audio.src != url) {
            self.audio.setAttribute('src', url);
            self.audio.play();
        } else {
            self.audio.pause();
        }
    });
}

AudioManager.prototype.bindListeners = function () {
    var self = this;

    $('#playTrackBtn').click(function () {
        if (self.audio.paused) {
            self.audio.play();
        } else {
            self.audio.pause();
        }
    });

    $('#nextTrackBtn').click(function () {
        $(self.tracksGlobal).each(function () {
            if ($(this).text() === self.audio.src) {
                var url = $($(self.tracksGlobal).get($(this).parent().index() + 1)).text();
                if (url === '') {
                    url = $($(self.tracksGlobal).get(0)).text();
                    //$($('.vk-item').get($(this).parent().index())).removeClass('draggable-item-selected');
                    //$($('.vk-item').get(0)).addClass('draggable-item-selected');
                } else {
                    //$($('.vk-item').get($(this).parent().index())).removeClass('draggable-item-selected');
                    //$($('.vk-item').get($(this).parent().index() + 1)).addClass('draggable-item-selected');
                }
                if (!self.audio.paused) {
                    self.audio.setAttribute('src', url);
                    self.audio.play();
                } else {
                    self.audio.setAttribute('src', url);
                }

                return false;
            }
        });
    });

    $('#prevTrackBtn').click(function () {
        $(self.tracksGlobal).each(function () {
            if ($(this).text() === self.audio.src) {
                var url = $($(self.tracksGlobal).get($(this).parent().index() - 1)).text();
                //$($('.vk-item').get($(this).parent().index())).removeClass('draggable-item-selected');
                //$($('.vk-item').get($(this).parent().index() - 1)).addClass('draggable-item-selected');
                if (!self.audio.paused) {
                    self.audio.setAttribute('src', url);
                    self.audio.play();
                } else {
                    self.audio.setAttribute('src', url);
                }

                return false;
            }
        });
    });
}