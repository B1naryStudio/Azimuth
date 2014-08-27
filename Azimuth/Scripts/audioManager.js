var AudioManager = function () {
    var self = this;
    var audio = new Audio();
    this.tracksGlobal = [];
}

AudioManager.prototype.bindPlayBtnListeners = function () {
    $('.track-play-btn').click(function () {
        tracksGlobal = $(this).parent().parent().children('.vk-item').children('.track-url');
        var url = $(this).parent().children('.track-url').text();
        if (audio.paused || audio.src != url) {
            audio.setAttribute('src', url);
            audio.play();
        } else {
            audio.pause();
        }
    });
}

AudioManager.prototype.bindListeners = function () {
    var self = this;

    $('#playTrackBtn').click(function () {
        if (audio.paused) {
            audio.play();
        } else {
            audio.pause();
        }
    });

    $('#nextTrackBtn').click(function () {
        $(tracksGlobal).each(function () {
            if ($(this).text() === audio.src) {
                var url = $($(tracksGlobal).get($(this).parent().index() + 1)).text();
                if (url === '') {
                    url = $($(tracksGlobal).get(0)).text();
                    //$($('.vk-item').get($(this).parent().index())).removeClass('draggable-item-selected');
                    //$($('.vk-item').get(0)).addClass('draggable-item-selected');
                } else {
                    //$($('.vk-item').get($(this).parent().index())).removeClass('draggable-item-selected');
                    //$($('.vk-item').get($(this).parent().index() + 1)).addClass('draggable-item-selected');
                }
                if (!audio.paused) {
                    audio.setAttribute('src', url);
                    audio.play();
                } else {
                    audio.setAttribute('src', url);
                }

                return false;
            }
        });
    });

    $('#prevTrackBtn').click(function () {
        $(tracksGlobal).each(function () {
            if ($(this).text() === audio.src) {
                var url = $($(tracksGlobal).get($(this).parent().index() - 1)).text();
                //$($('.vk-item').get($(this).parent().index())).removeClass('draggable-item-selected');
                //$($('.vk-item').get($(this).parent().index() - 1)).addClass('draggable-item-selected');
                if (!audio.paused) {
                    audio.setAttribute('src', url);
                    audio.play();
                } else {
                    audio.setAttribute('src', url);
                }

                return false;
            }
        });
    });
}