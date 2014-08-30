$(document).ready(function () {
    var volumeSlider = new SliderController('#volumeSlider');
    var audioManager = new AudioManager(volumeSlider);
    var manager = new SettingsManager(audioManager);
    
    manager.showPlaylists();
    manager.bindListeners();
    audioManager.bindListeners();
});