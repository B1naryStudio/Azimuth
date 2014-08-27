var SettingsManager = function(manager) {
	var self = this;
	this.playlistsGlobal = [];
	this.stringForCreateBtn = "Create new playlist ";
	this.playlistTrackTemplate = $("#playlistTrackTemplate");
	this.playlistTemplate = $("#playlistTemplate");
	this.trackTemplate = $("#trackTemplate");
	this.$reloginForm = $("#relogin");
	this.$vkMusic = $("#vkontakteMusic");
	this.$playlistsTable = $('#playlistsTable');
	this.$searchInput = $('#searchPlaylistName');
	this.$createNewPlaylistBtn = $('#createNewPlaylistBtn');

	this._getTracks = function() {
		//var $dragList = $(this).next();
		//if (!$(this).hasClass('active') && $dragList.children().length === 0) {
		$.ajax({
		    url: "/api/usertracks?playlistId=" + $(this).find('.playlistId').text(), // TODO replace with class playlistID
			type: 'GET',
			async: false,
			success: function(playlistData) {
				var tracks = playlistData.Result;
				for (var i = 0; i < tracks.length; i++) {
					var track = tracks[i];
					track.Duration = Math.floor(track.Duration / 60) + ":" + (track.Duration % 60 < 10 ? "0" + track.Duration % 60 : track.Duration % 60);
					self.playlistTrackTemplate.tmpl(track).appendTo('#playlistTracks');
				}
				$('.draggable').makeDraggable({
					contextMenu:[
						{ 'id': '1', 'name': 'first action', "isNewSection": "false" },
						{ 'id': '2', 'name': 'second action', "isNewSection": "false" },
						{ 'id': '3', 'name': 'third action', "isNewSection": "true" },
						{ 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
					]
				});
			}
		});
		//}
		//$(this).next("#playlistTracksTable").slideToggle(100); // TODO replace with class name
		$('#playlistsTable').hide();
		$('#backToPlaylistsBtn').show();
		$('#playlistTracks').show();
		$(this).toggleClass("active");
	};
};

SettingsManager.prototype.showPlaylists = function() {
	var self = this;
	this.$playlistsTable.find(".tableHeader").remove();
	if (typeof this.playlists === 'undefined') { //Initial run to get playlists from db
		$.ajax({
			url: '/api/playlists',
			success: function(playlists) {
				if (typeof playlists.Message === 'undefined') {
					self.$reloginForm.hide();
					self.$vkMusic.show();
					self.playlists = playlists.Result;
					for (var i = 0; i < self.playlists.length; i++) {
						var playlist = self.playlists[i];
					    if (playlist.Accessibilty === 1) {
					        playlist.Accessibilty = "public";
					    } else {
					        playlist.Accessibilty = "private";
					    }
					    self.playlistsGlobal.push(playlist);
							self.$playlistsTable.append(self.playlistTemplate.tmpl(playlist));
					}
				} else {
					self.$reloginForm.show();
					self.$reloginForm.find('a').attr('href', reloginUrl);
					self.$vkMusic.hide();
				}
				$('.accordion .tableRow').on("click", self._getTracks);
			}
		});
	} else { //using to print playlists after using filter
		if (this.playlists.length !== 0) {
			for (var i = 0; i < this.playlists.length; i++) {
				this.$playlistsTable.append(this.playlistTemplate.tmpl(playlists[i]));
			}
		} else {
			this.$createNewPlaylistBtn.show();
			this.$createNewPlaylistBtn.text(this.stringForCreateBtn + this.$searchInput.val());
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
					self.$vkMusic.show();
					var list = $('.vkMusicList');
					for (var i = 0; i < tracks.length; i++) {
						var track = tracks[i];
						track.duration = Math.floor(track.duration / 60) + ":" + (track.duration % 60 < 10 ? "0" + track.duration % 60 : track.duration % 60);
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
					self.$vkMusic.hide();
				}
				$('.playBtn').on('click', _playTrack);
			}
		});
	});

	this.$createNewPlaylistBtn.click(function () {
		var playlistName = self.$searchInput.val();
		$.ajax({
			url: '/api/playlists?name=' + playlistName + '&accessibilty=Public',
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json',
			async: false
		});
		self.$createNewPlaylistBtn.hide();
		$(document).trigger({ type: 'PlaylistAdded', Name: playlistName, Accessibilty: 1 });
	});

	$('#checkall').click(function () {
		if ($(this).prop('checked')) {
			$('input:checkbox').prop('checked', true);
		} else {
			$('input:checkbox').prop('checked', false);
		}
	});

	this.$searchInput.on('input', function (e) {
		self.$createNewPlaylistBtn.hide();
		var searchParam = $(self).val().toLocaleLowerCase();
		showPlaylists(self.playlistsGlobal.filter(function (index) {
			self.$searchInput.next().children().remove();
			return (index.Name.toLocaleLowerCase().indexOf(searchParam) != -1);
		}));
		$('.accordion .tableRow').on("click", self._getTracks);
	});

	$('#backToPlaylistsBtn').click(function () {
	    $('#backToPlaylistsBtn').hide();
	    $('#playlistTracks').empty();
	    $('#playlistTracks').hide();
        $('#playlistsTable').show(); 
    });
};