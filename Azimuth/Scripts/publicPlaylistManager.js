var PublicPlaylistManager = function (manager) {
    var self = this;
    self.audioManager = manager;
    self.playlists_global = null;
    self.$playlistTemplate = $("#playlistTemplate");
    self.$trackTemplate = $("#trackTemplate");
    self.$tracks = $('#tracks');
    self.$playlists = $('#playlists-plated');
    self.$playlistsArea = $('#playlistsArea');
    self.$tracksArea = $('#tracksArea');
    self.$playlistName = $('#playlistName');
    self.$listeners = $('#listeners');
    self.$backToPlaylistsBtn = $('#backToPlaylistsBtn');
    self.currentPlaylist = null;
    self.$likeBtn = $('#likeBtn');

    self.addCurrentUserAsListener = function(playlist) {
        var self = this;
        $.ajax({
            cache: false,
            type: "Post",
            url: "/api/listeners/" + playlist.Id,
            dataType: "json",
            async: false,
            success: function(data) {
                self.$listeners.text('This album listening ' + self.getPlaylistListeners(playlist) + ' people');
            }
        });
    };

    self.removeCurrentUserAsListener = function () {
        var self = this;
        $.ajax({
            cache: false,
            type: "delete",
            url: "/api/listeners/" + self.currentPlaylist.Id,
            dataType: "json",
            async: false,
            success: function(data) {
                self.$listeners.text(self.getPlaylistListeners(self.currentPlaylist));
            }
        });
    };

    self.getPlaylistListeners = function (playlist) {
        var self = this;
        self.$listeners.empty();
        var res = -1;
        $.ajax({
            cache: false,
            type: "GET",
            url: "/api/listeners/" + playlist.Id,
            dataType: "json",
            async: false,
            success: function(data) {
                res = data;
            }
        });
        return res;
    };

    self.showTracks = function (playlist) {
        var self = this;
        self.$playlistName.text(playlist.Name);
        self.$tracks.empty();
        $.ajax({
            url: "/api/usertracks?playlistId=" + playlist.Id, // TODO replace with class playlistID
            type: 'GET',
            async: false,
            success: function(tracksData) {
                var tracks = tracksData;
                for (var i = 0; i < tracks.length; ++i) {
                    var track = tracks[i];
                    var mins = Math.floor(track.Duration / 60);
                    var secs = ('0' + (track.Duration % 60)).slice(-2);
                    track.Duration = mins + ':' + secs;
                    self.$tracks.append($('#playlistTrackTemplate').tmpl(track));
                }
            }
        });
        self.audioManager.bindPlayBtnListeners();
        $(this).toggleClass("active");
    }

};


PublicPlaylistManager.prototype.bindListeners = function () {
    var self = this;
    self.$likeBtn.find('.icon').toggleClass("fa-thumbs-o-up");
    self.$backToPlaylistsBtn.click(function () {
        self.currentPlaylist = null;
        self.$playlistsArea.show();
        self.$tracksArea.hide();
        self.$backToPlaylistsBtn.hide();
        self.removeCurrentUserAsListener();
    });
    self.$playlists.click(function (event) {
        
        self.$playlistsArea.hide();
        self.$tracksArea.show();
        self.$backToPlaylistsBtn.show();
        var $playlist = $(event.target).closest('.playlist-plated');

        var playlistName = $playlist.find('.playlist-plated-info-name').text();
        self.currentPlaylist = self.playlists_global.filter(function (index) {
            return index.Name.valueOf() == playlistName;
        })[0];
        self.showTracks(self.currentPlaylist);
        $(document).trigger('newListenerAdded', [self.currentPlaylist]);
    });
    $(document).on('newListenerAdded', function (event, data) {
        self.$listeners.text(self.addCurrentUserAsListener(data));
    });

    self.$likeBtn.click(function() {
        //this.toggleClass("")
    });
};

PublicPlaylistManager.prototype.showPlaylists = function () {
    var self = this;
    self.$playlists.empty();
    $.ajax({
        cache: false,
        type: "GET",
        url: "/api/playlists/public",
        dataType: "json",
        async: false,
        success: function(data) {
            var playlists = data;
            self.playlists_global = playlists;
            for (var i = 0; i < playlists.length; i++) {
                var playlist = playlists[i];
                var mins = Math.floor(playlist.Duration / 60);
                var secs = ('0' + (playlist.Duration % 60)).slice(-2);
                playlist.Duration = mins + ':' + secs;
                playlist.Creator = playlist.Creator.Name;
                var $playlist = $('#playlistTemplate').tmpl(playlist);
                self.$playlists.append($playlist);
                var $listener = $playlist.find('.listeners');
                var returnVal = self.getPlaylistListeners(playlist);
                $listener.text('Listening ' + returnVal + ' people');
            }
        }
    });
};