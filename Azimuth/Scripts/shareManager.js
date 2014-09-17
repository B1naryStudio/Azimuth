var ShareManager = function (manager) {
    var self = this;
    this.audioManager = manager;
    this.tracksGlobal = [];
    this.$infoLoadingSpinner = $('#info-header-spinner');

    this._toFormattedTime = function (input, roundSeconds) {
        if (roundSeconds) {
            input = Math.ceil(input);
        }

        var withHours = false;

        if (input >= 60 * 60) {
            withHours = true;
        }

        var hoursString = '00';
        var minutesString = '00';
        var secondsString = '00';
        var hours = 0;
        var minutes = 0;
        var seconds = 0;

        hours = Math.floor(input / (60 * 60));
        input = input % (60 * 60);

        minutes = Math.floor(input / 60);
        input = input % 60;

        seconds = input;

        hoursString = (hours >= 10) ? hours.toString() : '0' + hours.toString();
        minutesString = (minutes >= 10) ? minutes.toString() : '0' + minutes.toString();
        secondsString = (seconds >= 10) ? seconds.toString() : '0' + seconds.toString();

        return ((withHours) ? hoursString + ':' : '') + minutesString + ':' + secondsString;
    };

    this._getTrackInfo = function () {
        var $self = $(this);
        var author = $self.children('.track-artist').html();
        var trackName = $self.children('.track-title').html();
        self.$infoLoadingSpinner.show();
        $.ajax({
            url: '/api/usertracks/trackinfo?artist=' + author + '&trackName=' + trackName,
            async: true,
            success: function (trackInfo) {
                self.$infoLoadingSpinner.hide();
                var $trackInfoTemplate = $('#trackInfoTemplate');
                var object = $trackInfoTemplate.tmpl(trackInfo);
                var $trackInfoContainer = $('.modal-body');
                $trackInfoContainer.text('');
                object.appendTo($trackInfoContainer);   
                if (trackInfo.Lyric != null) {
                    for (var i = 0; i < trackInfo.Lyric.length; i++) {
                        var $p = $('<p>');
                        $p.text(trackInfo.Lyric[i]);
                        $trackInfoContainer.find('.trackLyric').append($p);
                    }
                }
                if (trackInfo.Summary != null) {
                    $trackInfoContainer.find('.trackSummary').append(trackInfo.Summary);
                }
                self.topTracks = trackInfo.ArtistTopTrackList;
                $('#listenTopBtn').attr('disabled', false);
            }
        });
    };


    $('.track-info > a').click(self._getTrackInfo);
    $('.track-duration').each(function() {
        var $this = $(this);
        var oldDuration = $this.html();
        var formattedDuration = self._toFormattedTime(oldDuration, true);
        $this.html(formattedDuration);
    });
};

ShareManager.prototype.bindListeners = function () {
    var self = this;

    self.audioManager.bindPlayBtnListeners();

    $('#infoModal').on('hidden.bs.modal', function () {
        $('#infoModal .modal-body').text('');
        $('#listenTopBtn').attr('disabled', true);
    });

    $('#musicList-scrollable').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: { enable: true },
        updateOnContentResize: true,
        scrollInertia: 0,
        autoHideScrollbar: true,
        advanced: { updateOnSelectorChange: "true" }
    });
};
