var SliderController = function (options) {
    var self = this;

    this.drag = false;
    this.relativePosition = 0;
    this.dirrection = options.dirrection;
    this.$sliderBar = $(options.sliderSelector);
    self.$sliderBar.append('<div class="slider"></div>');
    this.$slider = $(self.$sliderBar.find('.slider'));

    this.size = (self.dirrection == 'vertical' ? 'height' : 'width');

    self.$sliderBar.addClass(options.sliderBarClass);
    self.$slider.addClass(options.sliderClass);
    self.bindListeners();

    this._getSize = function() {
        return (self.dirrection == 'vertical' ? self.$sliderBar.height() : self.$sliderBar.width());
    };

    this._getOffset = function () {
        return (self.dirrection == 'vertical' ? self.$sliderBar.offset().top : self.$sliderBar.offset().left);
    };

    this._getPagePos = function(e) {
        return (self.dirrection == 'vertical' ? e.pageY : e.pageX);
    };

    this._getCurrentPosition = function(e) {
        if (self.dirrection == 'vertical') {
            return self._getSize() - self._getPagePos(e) + self._getOffset();
        } else {
            return self._getPagePos(e) - self._getOffset();
        }
    };
}

SliderController.prototype.getSlider = function () {
    var self = this;
    return self.$sliderBar;
};

SliderController.prototype.getPosition = function () {
    var self = this;
    return self.relativePosition;
};

SliderController.prototype.setPosition = function (position) {
    var self = this;
    self.relativePosition = position;
    self.$slider.css(self.size, 100 * self.relativePosition + '%');
};

SliderController.prototype.bindListeners = function () {
    var self = this;

    self.$sliderBar.on('mousedown', function (e) {
        self.drag = true;
        var position = self._getCurrentPosition(e);
        var percentage = 100 * position / self._getSize();
        self.$slider.css(self.size, percentage + '%');
        self.relativePosition = position / self._getSize();

        self.$sliderBar.trigger('OnChange', [self.relativePosition]);
    });

    $(document).on('mousemove', function (e) {
        if (self.drag == false) {
            return;
        }
        if (self._getPagePos(e) < self._getOffset()) {
            //position = self._getSize();
            position = (self.dirrection == 'vertical' ? self._getSize() : 0);
        } else if (self._getPagePos(e) > self._getOffset() + self._getSize()) {
            //position = 0;
            position = (self.dirrection == 'vertical' ? 0 : self._getSize());
        } else {
            var position = self._getCurrentPosition(e);
        }
        var percentage = 100 * position / self._getSize();
        self.$slider.css(self.size, percentage + '%');
        self.relativePosition = position / self._getSize();

        self.$sliderBar.trigger('OnChange', [self.relativePosition]);
    });

    $(document).mouseup(function () {
        self.drag = false;
    });
};