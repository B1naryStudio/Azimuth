$(document).ready(function () {
    var audioManager = new AudioManager();
    var manager = new SettingsManager();
    
    manager.showPlaylists();
    manager.bindListeners(audioManager);
    audioManager.bindListeners();
});