var MyMusicManager = function (manager) {
    var self = this;
    this.audioManager = manager;
    this.$tracksContainer = $('#tracksTable');
    this.$playlistContainer = $('#playlistsTable');

    this._getTracks = function() {
        $.ajax({
            url: "/api/usertracks?playlistId=" + $(this).find('#playlistId').text(),
            type: 'GET',
            async: false,
            success: function(tracksData) {
                var tracks = tracksData;
                self.$tracksContainer.find('.tableRow').remove();
                for (var i = 0; i < tracks.length; i++) {
                    var track = tracks[i];
                    track.Duration = Math.floor(track.Duration / 60) + ":" + (track.Duration % 60 < 10 ? "0" + track.Duration % 60 : track.Duration % 60);
                    self.$tracksContainer.append($("#trackTemplate").tmpl(track));
                }
                self.audioManager.bindPlayBtnListeners();
            }
        });
    };
};

MyMusicManager.prototype.showPlaylists = function () {
    var self = this;
    $.ajax({
        cache: false,
        type: "GET",
        url: "/api/playlists/own",
        dataType: "json",
        async: false,
        success: function (playlists) {
            for (var i = 0; i < playlists.length; i++) {
                var playlist = playlists[i];
                if (playlist.Accessibilty == 1)
                    playlist.Accessibilty = "public";
                else
                    playlist.Accessibilty = "private";
                self.$playlistContainer.append($("#playlistTemplate").tmpl(playlist));
            }
        }
    });
};

MyMusicManager.prototype.bindListeners = function () {
    var self = this;
    $('.accordion .tableRow').on("click", self._getTracks);
};