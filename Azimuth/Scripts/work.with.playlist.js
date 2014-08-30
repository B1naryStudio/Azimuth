$(document).ready(function () {
    var volumeSlider = new SliderController('#volumeSlider', 'volumeBar', 'volume', 'vertical');
    var progressSlider = new SliderController('#progressSlider', 'progressBar', 'progress', 'horizontall');

    var audioManager = new AudioManager(volumeSlider, progressSlider);
    var manager = new SettingsManager(audioManager);
    
    manager.showPlaylists();
    manager.bindListeners();
    audioManager.bindListeners();
});