var ProgressBar = function() {
    var self = this;

    this.drag = false;
    this.$progressBar = $('#progress-bar');
    this.$currentProgress = $('#progress-current');
    this.$cacheProgress = $('.progress-cache');
    this.$moveProgress = $('#progress-move');

    this._randomInt = function(max, min) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    };

    for (var i = 0; i < self.$progressBar.width() - 9; i += 3) {
        var randomHeight = self._randomInt(100, 20);
        var element = $('<div></div>').addClass('progress-bar-element');
        element.append($('<div></div>').addClass('progress-bar-background-element').css('height', (100 - randomHeight) + '%'));
        element.append($('<div></div>').addClass('progress-bar-current-element').css('height', randomHeight + '%'));
        self.$progressBar.append(element);
    }
};

ProgressBar.prototype.getPosition = function () {
    var self = this;
    return self.$currentProgress.width() / self.$progressBar.width();
};

ProgressBar.prototype.setPosition = function (position) {
    var self = this;
    self.$currentProgress.css('width', 100 * position + '%');
};

ProgressBar.prototype.setBackgroundPosition = function (position) {
    var self = this;
    self.$cacheProgress.css('width', 100 * position + '%');
};

ProgressBar.prototype.bindListeners = function() {
    var self = this;

    self.$progressBar.on('mousedown', function(e) {
        self.drag = true;
        var position = e.pageX - self.$progressBar.offset().left;
        var percentage = 100 * position / self.$progressBar.width();
        self.$currentProgress.css('width', percentage + '%');

        self.$progressBar.trigger('OnChange', [percentage]);
    });

    $(document).on('mousemove', function(e) {
        if (self.drag == false) {
            return;
        }
        if (e.pageX < self.$progressBar.offset().left) {
            position = 0;
        } else if (e.pageX > self.$progressBar.offset().left + self.$progressBar.width()) {
            position = self.$progressBar.width();
        } else {
            var position = e.pageX - self.$progressBar.offset().left;
        }
        var percentage = 100 * position / self.$progressBar.width();
        self.$currentProgress.css('width', percentage + '%');

        self.$progressBar.trigger('OnChange', [percentage]);
    });

    $(document).mouseup(function() {
        self.drag = false;
    });

    $('#progress-bar').on('mousemove', function(e) {
        if (e.pageX < self.$progressBar.offset().left) {
            position = 0;
        } else if (e.pageX > self.$progressBar.offset().left + self.$progressBar.width()) {
            position = self.$progressBar.width();
        } else {
            var position = e.pageX - self.$progressBar.offset().left;
        }
        var percentage = 100 * position / self.$progressBar.width();

        if (self.$currentProgress.width() > self.$moveProgress.width() && self.$currentProgress.hasClass('light-orange')) {
            self.$currentProgress.removeClass('light-orange').addClass('dark-orange');
            self.$moveProgress.removeClass('dark-orange').addClass('light-orange');
        } else if (self.$currentProgress.width() < self.$moveProgress.width() && self.$currentProgress.hasClass('dark-orange')) {
            self.$moveProgress.removeClass('light-orange').addClass('dark-orange');
            self.$currentProgress.removeClass('dark-orange').addClass('light-orange');
        }

        self.$moveProgress.css('width', percentage + '%');
    });

    $('#progress-bar').on('mouseleave', function () {
        if (self.$currentProgress.width() > self.$moveProgress.width()) {
            self.$moveProgress.removeClass('light-orange').addClass('dark-orange');
            self.$currentProgress.removeClass('dark-orange').addClass('light-orange');
        }

        self.$moveProgress.css('width', '0');
    });
};