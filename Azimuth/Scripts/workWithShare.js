$(document).ready(function () {
    var volumeSlider = new SliderController({
        sliderSelector: '#volumeSlider',
        sliderBarClass: 'volumeBar',
        sliderClass: 'volume',
        dirrection: 'horizontall'
       });
    var progressBar = new ProgressBar();
    progressBar.bindListeners();

    var audioManager = new AudioManager(volumeSlider, progressBar);
    var manager = new ShareManager(audioManager);

    manager.bindListeners();
    audioManager.bindListeners();
    audioManager.bindPlayBtnListeners();
});