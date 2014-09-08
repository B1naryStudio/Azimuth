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
    self.$favoriteBtn = $('#favoriteBtn');
    self.$likesCounter = $('.likesCounter');
    self.currentLikeStatus = null;
    self.currentFavoriteStatus = null;

    this._setNewImage = function ($playlist, id) {
        $.ajax({
            url: '/api/playlists/image/' + id,
            success: function (image) {
                var $logo = $playlist;
                $logo.fadeOut(500, function() {
                    if (image != "") {
                        $logo.css({
                            "background-image":'url('+image+')'});
                    } else {
                        $logo.css({
                            "background-image": 'url(http://cdns2.freepik.com/free-photo/music-album_318-1832.jpg)'
                        });
                    }
                });
                $logo.fadeIn(500);
            }
        });
    };

    self.addCurrentUserAsListener = function (playlist) {
        $.ajax({
            cache: false,
            type: "POST",
            url: "/api/listened/" + playlist.Id,
            dataType: "json",
            async: false,
            success: function (data) {
                
            }
        });
    };

    //self.removeCurrentUserAsListener = function () {
    //    console.log('remove');
    //    console.log(self.currentPlaylist);
    //    var playlist = self.currentPlaylist;
    //    $.ajax({
    //        cache: false,
    //        type: "delete",
    //        url: "/api/listeners/" + playlist.Id,
    //        dataType: "json",
    //        success: function (data) {
    //            //var playlist = self.currentPlaylist;
    //            console.log('remove');
    //            console.log(playlist);
    //            self.$listeners.text(self.getPlaylistListeners(playlist));
    //        }
    //    });
    //};

    self.getPlaylistListeners = function (playlist) {
        if (playlist == null)
            playlist = self.currentPlaylist;
        self.$listeners.empty();
        var res = -1;
        $.ajax({
            cache: false,
            type: "GET",
            url: "/api/listened/" + playlist.Id,
            dataType: "json",
            async: false,
            success: function (data) {
                res = data;
            }
        });
        return res;
    };

    self.showLikes = function() {
        $.ajax({
            cache: false,
            type: "GET",
            url: "/api/likes/" + self.currentPlaylist.Id,
            dataType: "json",
            success: function (data) {
                self.$likesCounter.text(data);
            }
        });
    };

    self.showTracks = function (playlist) {
        var self = this;
        self.$playlistName.text(playlist.Name);
        self.$tracks.empty();
        $.ajax({
            url: "/api/usertracks?playlistId=" + playlist.Id, // TODO replace with class playlistID
            type: 'GET',            
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

    self.setLikeBtnIcon = function () {
        self.$likeBtn.find('.icon').removeClass('fa-thumbs-o-up').removeClass('fa-thumbs-up');
        if (!self.currentLikeStatus) {
            self.$likeBtn.find('.icon').toggleClass('fa-thumbs-up');
            self.$likeBtn.css({
                'background-color': 'rgb(100,120,255)'
            });
        } else {
            self.$likeBtn.find('.icon').toggleClass('fa-thumbs-o-up');
            self.$likeBtn.css({
                'background-color': 'white'
            });
        }
    }

    self.setFavoriteBtnIcon = function () {
        if (!self.currentFavoriteStatus) {
            self.$favoriteBtn.css({
                'background-color': 'rgb(100,120,255)',
                'color' : 'white'
            });
        } else {
            self.$favoriteBtn.css({
                'background-color': 'white',
                'color' : 'black'
            });
        }
    }

    self.isLiked = function (playlist) {
        $.ajax({
            url: "/api/likes/status/" + playlist.Id,
            type: 'GET',
            async: false,
            success: function (data) {
                self.currentLikeStatus = data^true;
            }
        });
        return self.currentLikeStatus;
    }

    self.isFavorite = function (playlist) {
        $.ajax({
            url: "/api/favorite/status/" + playlist.Id,
            type: 'GET',
            async: false,
            success: function (data) {
                self.currentFavoriteStatus = data ^ true;
            }
        });
        return self.currentFavoriteStatus;
    }

};


PublicPlaylistManager.prototype.bindListeners = function () {
    var self = this;
    //self.currentLikeStatus = 0;
    //self.$likeBtn.find('.icon').toggleClass("fa-thumbs-o-up");
    console.log('likBTN');
    console.log(self.$likeBtn);
    self.$backToPlaylistsBtn.click(function () {
        //self.removeCurrentUserAsListener(); was needed when it worked with listeners
        
        self.$playlistsArea.show();
        self.showPlaylists();
        self.$tracksArea.hide();
        self.$backToPlaylistsBtn.hide();
        self.currentPlaylist = null;
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
        self.showLikes();
        $(document).trigger('newListenerAdded', [self.currentPlaylist]);
        self.isLiked(self.currentPlaylist);
        self.setLikeBtnIcon();
        self.isFavorite(self.currentPlaylist);
        self.setFavoriteBtnIcon();
    });
    
    self.$likeBtn.click(function () {
        var action = null;
        if (self.isLiked(self.currentPlaylist))
            action = "POST";
        else 
            action = "DELETE";
            
        $.ajax({
            cache: false,
            type: action,
            url: "/api/likes/" + self.currentPlaylist.Id,
            dataType: "json",
            async:false,
            success: function (data) {
                $(document).trigger('likeStatusChanged');
            }
        });
        self.isLiked(self.currentPlaylist);//TODO donno why, but I doing wasted request to server
        self.setLikeBtnIcon();
        
    });
    self.$favoriteBtn.click(function () {
        var action = null;
        if (self.isFavorite(self.currentPlaylist))
            action = "POST";
        else
            action = "DELETE";
        $.ajax({
            cache: false,
            type: action,
            url: "/api/favorite/" + self.currentPlaylist.Id,
            dataType: "json",
            async: false,
            success: function (data) {
            }
        });
        self.isFavorite(self.currentPlaylist);//TODO donno why, but I doing wasted request to server
        self.setFavoriteBtnIcon();

    });
    $(document).on('likeStatusChanged', function () {
        self.showLikes();
    });
    $(document).on('newListenerAdded', function (event, playlist) {
        self.addCurrentUserAsListener(playlist);
        self.$listeners.text('This playlist listened ' + self.getPlaylistListeners(playlist) + ' people');
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
        success: function(data) {
            var playlists = data;
            self.playlists_global = playlists;
            for (var i = 0; i < playlists.length; i++) {
                var playlist = playlists[i];
                var mins = Math.floor(playlist.Duration / 60);
                var secs = ('0' + (playlist.Duration % 60)).slice(-2);
                playlist.Duration = mins + ':' + secs;
                playlist.Creator = playlist.Creator.Name;
                playlist.CreatorId = playlist.Creator.Id;
                var $playlist = $('#playlistTemplate').tmpl(playlist);
                self.$playlists.append($playlist);
                var $listener = $playlist.find('.listeners');
                var returnVal = self.getPlaylistListeners(playlist);
                self._setNewImage($playlist, playlist.Id);
                $listener.text('Listening ' + returnVal + ' people');
            }
        }
    });
};