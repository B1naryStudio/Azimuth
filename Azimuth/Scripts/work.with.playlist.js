$(document).ready(function() {
    var manager = new SettingsManager();
    manager.showPlaylists();
    manager.bindListeners();
});