$(document).ready(function () {
    var audioManager = new AudioManager();
    var manager = new SettingsManager(audioManager);
    
    manager.showPlaylists();
    manager.bindListeners();
    audioManager.bindListeners();
});