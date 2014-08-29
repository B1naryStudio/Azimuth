var SettingsManager = function(manager) {
    var self = this;
    this.audioManager = manager;
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
			    //$('.draggable').makeDraggable({
                $('.stuck.draggable').makeDraggable({
					contextMenu:[
						{ 'id': '1', 'name': 'first action', "isNewSection": "false" },
						{ 'id': '2', 'name': 'second action', "isNewSection": "false" },
						{ 'id': '3', 'name': 'third action', "isNewSection": "true" },
						{ 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
					],
					onMoveTrackToNewPosition: moveTrackToNewPosition
				});
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

SettingsManager.prototype.showPlaylists = function(playlists) {
	var self = this;
	self.$playlistsTable.find(".tableHeader").remove();
	if (typeof this.playlists === 'undefined') { //Initial run to get playlists from db
		$.ajax({
			url: '/api/playlists',
			success: function(playlistsData) {
				if (typeof playlistsData.Message === 'undefined') {
					self.$reloginForm.hide();
					self.$vkMusic.show();
					self.playlists = playlistsData.Result;
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
					self.$vkMusic.hide();
				}
				$('.accordion .tableRow').on("click", self._getTracks);
			}
		});
	} else { //using to print playlists after using filter
		if (self.playlists.length !== 0) {
			for (var i = 0; i < this.playlists.length; i++) {
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
					self.$vkMusic.show();
					var list = $('.vkMusicList');
					for (var i = 0; i < tracks.length; i++) {
						var track = tracks[i];
						track.duration = self._toFormattedTime(track.duration, true);
						self.trackTemplate.tmpl(track).appendTo(list);
					}
					//$('.draggable').makeDraggable({
                    list.parent().parent().makeDraggable({
						contextMenu: [
							{ 'id': '1', 'name': 'first action', "isNewSection": "false" },
							{ 'id': '2', 'name': 'second action', "isNewSection": "false" },
							{ 'id': '3', 'name': 'third action', "isNewSection": "true" },
							{ 'id': '4', 'name': 'fourth action', "isNewSection": "false" }
						],
						saveVkTrack: saveTrackFromVkToPlaylist
					});
				} else {
					self.$reloginForm.show();
					self.$reloginForm.find('a').attr('href', reloginUrl);
					self.$vkMusic.hide();
				}
			    self.audioManager.bindPlayBtnListeners();
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

		var searchParam = $(this).val().toLocaleLowerCase();
		self.showPlaylists(self.playlistsGlobal.filter(function (index) {
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

var moveTrackToNewPosition = function($currentItem, $draggableStub) {
    var playlistId = $('.playlist.active').children('.playlistId').text();
    var tracksIds = [];

    $currentItem.children().each(function () {
        tracksIds.push($(this).find('.trackId').text());
    }).get();

    var index = $draggableStub.index();
    $.ajax({
        url: '/api/usertracks/put?playlistId=' + playlistId + "&newIndex=" + index,
        type: 'PUT',
        dataType: 'json',
        data: JSON.stringify(tracksIds),
        contentType: 'application/json; charset=utf-8'
    });
}

var saveTrackFromVkToPlaylist = function($currentItem, $draggableStub, $element) {
    var index = -1;
    var provider = $('.tab-pane.active').attr('id');
    var tracks = [];
    var playlistId = -1;
    $currentItem.children().toggleClass('vk-item', false);
    if ($element.hasClass('playlist')) {
        playlistId = $element.children('.playlistId').text();
    } else {
        playlistId = $('.playlist.active').children('.playlistId').text();
        index = $draggableStub.index();
    }
    $('.draggable-item-selected').each(function () {
        tracks.push($(this).closest('.tableRow').find('.trackId').text());
    }).get();

    $.ajax({
        url: '/api/usertracks?provider=' + provider + "&index=" + index,
        type: 'POST',
        data: JSON.stringify({
            "Id": playlistId,
            "TrackIds": tracks
        }),
        dataType: 'json',
        contentType: 'application/json',
        async: true,
        success: function() {
            
        }
    });
}