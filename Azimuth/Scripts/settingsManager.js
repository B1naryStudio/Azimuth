var SettingsManager = function(manager) {
    var self = this;
    this.audioManager = manager;
    this.playlistsGlobal = [];
    this.playlistTracksGlobal = [];
	this.stringForCreateBtn = "Create new playlist ";
	this.playlistTrackTemplate = $("#playlistTrackTemplate");
	this.playlistTemplate = $("#playlistTemplate");
	this.trackTemplate = $("#trackTemplate");
	this.$reloginForm = $("#relogin");
	this.$vkMusic = $("#vkontakteMusic");
	this.$playlistsTable = $('#playlistsTable');
	this.$searchInput = $('#searchPlaylistName');
    this.$vkMusicTable = $('#vkMusicTable').parent();
	this.$createNewPlaylistLbl = $('#create-playlist-lbl');

	this._getTracks = function() {
		//var $dragList = $(this).next();
		//if (!$(this).hasClass('active') && $dragList.children().length === 0) {
	    $.ajax({
	        url: "/api/usertracks?playlistId=" + $(this).find('.playlistId').text(), // TODO replace with class playlistID
	        type: 'GET',
	        async: false,
	        success: function (tracksData) {
	            for (var i = 0; i < tracksData.length; i++) {
	                tracksData[i].Duration = Math.floor(tracksData[i].Duration / 60) + ":" + (tracksData[i].Duration % 60 < 10 ? "0" + tracksData[i].Duration % 60 : tracksData[i].Duration % 60);
	            }
	            self.playlistTracksGlobal = tracksData;
	            self.showTracks(tracksData);
	            $('.draggable').makeDraggable({
	                contextMenu: [
	                    { 'id': '1', 'name': 'first action', "isNewSection": "false" },
	                    { 'id': '2', 'name': 'second action', "isNewSection": "false" },
	                    { 'id': '3', 'name': 'third action', "isNewSection": "true" },
	                    { 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
	                ]
	            });
	            self.$searchInput.val('');
	        }
	});
		//}
		//$(this).next("#playlistTracksTable").slideToggle(100); // TODO replace with class name
		$('#playlistsTable').hide();
		$('#backToPlaylistsBtn').show();
		$('#playlistTracks').show();
		self.audioManager.bindPlayBtnListeners();
		$(this).toggleClass("active");
	};

	this._toFormattedTime = function(input, roundSeconds)
	{
	    if (roundSeconds)
	    {
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

	
};

SettingsManager.prototype.showTracks = function (tracks) {
    var self = this;

    $('#playlistTracks').find('.track').remove();
    for (var i = 0; i < tracks.length; i++) {
        self.playlistTrackTemplate.tmpl(tracks[i]).appendTo('#playlistTracks');
    }
};

SettingsManager.prototype.showPlaylists = function(playlists) {
	var self = this;
	self.$playlistsTable.find(".tableHeader").remove();
	if (typeof playlists === 'undefined') { //Initial run to get playlists from db
		$.ajax({
			url: '/api/playlists',
			success: function(playlistsData) {
				if (typeof playlistsData.Message === 'undefined') {
					self.$reloginForm.hide();
					self.$vkMusicTable.show();
					self.playlists = playlistsData;
					for (var i = 0; i < self.playlists.length; i++) {
						var playlist = self.playlists[i];
					    if (playlist.Accessibilty === 1) {
					        playlist.Accessibilty = "public";
					    } else {
					        playlist.Accessibilty = "private";
					    }
					    playlist.Duration = self._toFormattedTime(playlist.Duration, true);
					    self.playlistsGlobal.push(playlist);
						self.$playlistsTable.append(self.playlistTemplate.tmpl(playlist));
					}
				} else {
					self.$reloginForm.show();
					self.$reloginForm.find('a').attr('href', reloginUrl);
					self.$vkMusicTable.hide();
				}
				$('.accordion .tableRow').on("click", self._getTracks);
			}
		});
	} else { //using to print playlists after using filter
		if (self.playlists.length !== 0) {
			for (var i = 0; i < playlists.length; i++) {
			    self.$playlistsTable.append(this.playlistTemplate.tmpl(playlists[i]));
			}
		} else {
		    self.$createNewPlaylistBtn.show();
		    self.$createNewPlaylistBtn.text(this.stringForCreateBtn + this.$searchInput.val());
		}
	}    	
};

SettingsManager.prototype.bindListeners = function() {
    var self = this;

	$(document).on('PlaylistAdded', function(playlist) { // TODO Remove event triggering on document object
		self.playlistsGlobal.push({ Name: playlist.Name, Accessibilty: playlist.Accessibilty });
		self.$searchInput.trigger('input');
	});

	$('.providerBtn').click(function(e) {
		var provider = $(e.target).data('provider');
		var reloginUrl = $(e.target).data('reloginurl');
		$.ajax({
			url: '/api/usertracks?provider=' + provider,
			success: function(tracks) {
				if (typeof tracks.Message === 'undefined') {
					self.$reloginForm.hide();
					self.$vkMusicTable.show();
					var list = $('.vkMusicList');
					for (var i = 0; i < tracks.length; i++) {
						var track = tracks[i];
						track.duration = self._toFormattedTime(track.duration, true);
						self.trackTemplate.tmpl(track).appendTo(list);
					}
					$('.draggable').makeDraggable({
						contextMenu: [
							{ 'id': '1', 'name': 'first action', "isNewSection": "false" },
							{ 'id': '2', 'name': 'second action', "isNewSection": "false" },
							{ 'id': '3', 'name': 'third action', "isNewSection": "true" },
							{ 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
						]
					});
				} else {
					self.$reloginForm.show();
					self.$reloginForm.find('a').attr('href', reloginUrl);
					self.$vkMusicTable.hide();
				}
			    self.audioManager.bindPlayBtnListeners();
			}
		});
	});



	//$('#checkall').click(function () {
	//	if ($(this).prop('checked')) {
	//		$('input:checkbox').prop('checked', true);
	//	} else {
	//		$('input:checkbox').prop('checked', false);
	//	}
	//});

	this.$searchInput.keyup(function (e) {
	    var searchParam = $(this).val().toLocaleLowerCase();

	    var foundedPlaylist = self.playlistsGlobal.filter(function(index) {
	        self.$searchInput.next().children().remove();
	        return (index.Name.toLocaleLowerCase().indexOf(searchParam) != -1);
	    });

	    if (foundedPlaylist.length == 0) {
	        self.$createNewPlaylistLbl.show();
            if (e.keyCode == 13) {
                console.log(e);
                self._createPlaylist();
                self.playlistsGlobal = [];
                $(this).val("");
                self.showPlaylists();
            }
	    } else {
	        self.$createNewPlaylistLbl.hide();
	    }

	    self.showPlaylists(foundedPlaylist);
		self.showTracks(self.playlistTracksGlobal.filter(function (index) {
		    self.$searchInput.next().children().remove();
		    return (index.Name.toLocaleLowerCase().indexOf(searchParam) != -1);
		}));
		
		$('.accordion .tableRow').on("click", self._getTracks);
	});

	$('#backToPlaylistsBtn').click(function () {
	    $('#backToPlaylistsBtn').hide();
	    $('#playlistTracks').empty();
	    $('#playlistTracks').hide();
	    self.showPlaylists(self.playlistsGlobal);
	    self.$playlistsTable.show();
	    self.$searchInput.val('');
	});

    $('#playlists').mCustomScrollbar({
        theme: 'dark-3',
        scrollButtons: {enable: true}
    });

    this._createPlaylist = function () {
        var playlistName = self.$searchInput.val();
        $.ajax({
            url: '/api/playlists?name=' + playlistName + '&accessibilty=Public',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            async: false
        });
        self.$createNewPlaylistLbl.hide();
        $(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
    };
};