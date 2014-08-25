$(document).ready(function () {

    $.fn.makeDraggable = function (options) {
        var $rootElement = this;
        var $currentItem = null;
        //var $draggableStub = $('<li>').toggleClass('draggable-item', true).toggleClass('draggable-stub', true);
        var $draggableStub = $('<div class="tableRow draggable-item">').toggleClass('draggable-item', true).toggleClass('draggable-stub', true);// +
                                //    '<div class="cell"></div>' +
                                //    '<div class="cell"></div>' +
                                //    '<div class="cell"><input type="checkbox" checked="checked"></div>' +
                                //    '<div class="cell" style="display: none" id="trackId">${id}</div>' +
                                //'</div>').toggleClass('draggable-item', true).toggleClass('draggable-stub', true);

        var $container = $('#itemsContainer');
        var parentId = "";
        var movingInfo = { "data": [] };
        var timerId = null;
        var lastEvent = null;
        var elementOffset = { x: 0, y: 0 };
        var mousedown = false;
        var deleteFlag = false;

        this.find('.draggable-list > .tableRow').each(function (index, item) {
            var $item = $(item);
            $item.toggleClass('draggable-item', true);
            $item.mousedown(_makeDraggable);
        });


        this.mousemove(function(event) {
            lastEvent = event;
            if ($currentItem && mousedown) {
                var $elem = _getCurrentTarget(event);
                var listCount = $currentItem.parent().children().length;

                if ($currentItem.hasClass('draggable-item-selected')) {
                    var width = $currentItem.width();
                    $currentItem = $container;
                    $currentItem.css({
                        'width': width + 'px'
                });
                    $currentItem.append($('.draggable-item-selected'));
                }
                $currentItem.toggleClass('dragging', true);

                if (!$currentItem.hasClass('itemsContainer')) {
                    $draggableStub.insertAfter($currentItem.children().last());
                }

                _setRelativePosition(event);
                if ($elem) {
                    if ($elem.hasClass('draggable-list')) {
                        $elem.append($draggableStub);
                    } else if (!$elem.hasClass('delete-area')) {
                        var childPos = $currentItem.offset();
                        var parentPos = $draggableStub.parent().offset();

                        if ($elem.length > 0) {
                            clearTimeout(timerId);
                            var $savedItem = $currentItem;
                            timerId = setTimeout(function () {
                                var $hoverElement = _getCurrentTarget(lastEvent);
                                if ($elem.is($hoverElement)) {
                                    $elem.trigger('MergeItems', [$elem, $savedItem, event.pageY]);
                                }
                            }, 2000);
                            if (childPos && parentPos && childPos.top - parentPos.top < $($currentItem.children[0]).outerHeight() / 2) {
                                $draggableStub.insertBefore($elem);
                            } else {
                                $draggableStub.insertAfter($elem);
                            }
                        }
                    }
                }
            }
        });

        $(document).mouseup(function () {
            mousedown = false;
            clearTimeout(timerId);
            if ($currentItem) {
                var $element = _getCurrentTarget(event);
                if ($element.length == 0) {
                    if ($currentItem.hasClass('itemsContainer')) {
                        $('#' + parentId).append($currentItem.children());
                        $currentItem = null;
                    } else {
                        $currentItem.removeAttr('style');
                        $currentItem.toggleClass('dragging', false);
                    }
                } else {

                    if ($element.hasClass('delete-area')) {
                        $currentItem.trigger("delete");
                    }

                    //if ($currentItem != null && $currentItem.children().length > 0 && !$element.hasClass('delete-area')) {
                    if ($currentItem != null && ($currentItem.hasClass('itemsContainer') && $currentItem.children().length > 0) > 0 && !$element.hasClass('delete-area')) {
                        var $items = "Items ";
                        for (i = 0; i < $currentItem.children().length; i++) {
                            $items += $($currentItem.children()[i]).html() + " ";
                        }
                        $currentItem.trigger('OnDropped', [movingInfo, $draggableStub.parent().attr("name")]);
                        movingInfo.data.length = 0;
                        $currentItem.children().insertAfter($draggableStub);
                        _clearContainer();
                    }
                }
            }
            $draggableStub.detach();
        });

        function _makeDraggable(event) {
            mousedown = true;
            $currentItem = $(this);
            var pos = $currentItem.offset();
            elementOffset.x = event.pageX - pos.left;
            elementOffset.y = event.pageY - pos.top;

            if (!$currentItem.hasClass('draggable-item-selected') && !event.shiftKey) {
                movingInfo.data.push({ "name": $currentItem.parent().attr('name'), "item": $currentItem.html() });
            }

            if (event.ctrlKey) {
                if ($currentItem.hasClass('draggable-item-selected')) {
                    $currentItem.toggleClass('draggable-item-selected', false);
                    for (i = 0; i < movingInfo.data.length; i++) {
                        if (movingInfo.data[i].name == $currentItem.parent().attr('name') && movingInfo.data[i].item == $currentItem.html()) {
                            movingInfo.data.splice(i, 1);
                        }
                    }
                } else {
                    $currentItem.toggleClass('draggable-item-selected', true);
                }
            } else if (event.shiftKey) {
                var indexFirst = -1;
                var indexLast = -1;
                if ($('.draggable-item-selected').index() < $currentItem.index()) {
                    indexFirst = $('.draggable-item-selected').index();
                    indexLast = $currentItem.index();
                } else {
                    indexFirst = $currentItem.index();
                    indexLast = $('.draggable-item-selected').index();
                }

                var currentChildren = $currentItem.parent().children();

                for (i = indexFirst; i <= indexLast; i++) {
                    var $currentChild = $(currentChildren[i]);
                    if (!$currentChild.hasClass('draggable-item-selected')) {
                        $currentChild.toggleClass('draggable-item-selected', true);
                        movingInfo.data.push({ "name": $currentChild.parent().attr('name'), "item": $currentChild.html() });
                    }
                }
            } else {
                if (!$currentItem.parent().children().hasClass('draggable-item-selected') && $('.draggable-item-selected').length > 0) {
                    $('.draggable-item-selected').toggleClass('draggable-item-selected', false);
                } else if ($currentItem.parent().children().hasClass('draggable-item-selected') && $('.draggable-item-selected').length > 0 && !$currentItem.hasClass('draggable-item-selected')) {
                    $('.draggable-item-selected').toggleClass('draggable-item-selected', false);
                } else if ($currentItem.hasClass('draggable-item-selected') && $('.draggable-item-selected').length < 2) {
                    $('.draggable-item-selected').toggleClass('draggable-item-selected', false);
                } else if ($('.draggable-item-selected').length == 1) {
                    $('.draggable-item-selected').toggleClass('draggable-item-selected', false);
                }
                $currentItem.toggleClass('draggable-item-selected', true);
            }
            parentId = $currentItem.parent().attr('id');
        };

        function _setRelativePosition(event) {
            var parentOffet = $rootElement.offset();
            var relX = event.pageX - elementOffset.x;// - parentOffet.left;
            var relY = event.pageY - elementOffset.y;// - parentOffet.top;
            $currentItem.css({
                'top': relY + 'px',
                'left': relX + 'px'
            });
        }

        function _getCurrentTarget(event) {
            if (navigator.userAgent.match('MSIE') || navigator.userAgent.match('Gecko')) {
                var x = event.clientX, y = event.clientY;
            } else {
                var x = event.pageX, y = event.pageY;
            }
            $currentItem.hide();
            var $elem = $(document.elementFromPoint(x, y));
            if ($elem.hasClass('delete-area'))
                _onDeleteArea();
            else if (deleteFlag == true) {
                _outDeleteArea();
            }
            $currentItem.show();
            //if ($elem.hasClass('draggable-stub-empty')) {
            //    return $elem;
            //}

            if ($elem.hasClass('delete-area')) {
                return $elem;
            }

            if ($elem.closest('.draggable-list').find('.draggable-item').length === 0) {
                return $elem.closest('.draggable-list');
            }
            return $elem.closest('.draggable-item:not(.dragging.draggable-stub)');
        }

        function _setDeleteAreaCss() {
            if (deleteFlag == true) {
                $('.delete-area').css({
                    'border': 'solid red 3px'
                })
            } else {
                $('.delete-area').css({
                    'border': 'solid black 1px'
                })
            }
        }

        function _clearContainer() {
            $currentItem.removeAttr('style');
            $currentItem.toggleClass('dragging', false);
            $('.draggable-item-selected').toggleClass('draggable-item-selected', false);
            $currentItem = null;
        }

        function _onDeleteArea() {
            deleteFlag = true;
            _setDeleteAreaCss();
        }

        function _outDeleteArea() {
            deleteFlag = false;
            _setDeleteAreaCss();
        }

        $('.draggable').on('MergeItems', function (e, mergeTo, mergeElem, pageY) {
            mergeToPos = mergeTo.offset();
            if ((!mergeTo.hasClass('draggable-stub')) && (mergeToPos.top + 0.15 * $('.draggable-item').height() < pageY) && (mergeToPos.top + 0.85 * $('.draggable-item').height() > pageY)) {
                if (mergeElem && mergeTo) {
                    if ($currentItem.hasClass('itemsContainer') && $('.draggable-item-selected').length > 0) {
                        $currentItem.children().each(function (index, item) {
                            mergeTo.text(mergeTo.html() + ' ' + $(item).html());
                        });
                        $currentItem.children().detach();
                    } else {
                        mergeTo.text(mergeTo.html() + ' ' + mergeElem.html());
                        $currentItem.detach();
                    }
                    mergeTo.trigger('OnCombined', [mergeTo.text()]);
                    _clearContainer();
                }
            }
        })

        $('.container').on('delete', function (e) {
            $currentItem.empty();
            _clearContainer();
            deleteFlag = false;
            _setDeleteAreaCss();
        });

        $('.draggable-list').on('shuffle', function () {
            var $this = $(this);
            var elems = $this.children('li');
            elems.sort(function () { return (Math.round(Math.random()) - 0.5); });
            $this.remove(elems[0].tagName);
            $this.prepend(elems);
        })

        $('.draggable-list').on('add', function (e, data) {
            var $newItem = $('<li>').toggleClass('draggable-item', true).text(data);
            $newItem.mousedown(_makeDraggable);
            $(this).prepend($newItem);
        });

        $('.draggable-list').on('sort', function (e, dir) {
            $list = $(this);
            var sorted = $list.find("> .draggable-item").sort(function (a, b) {
                return $(a).text().toLowerCase() > $(b).text().toLowerCase() ? dir : -dir;
            });
            $list.prepend(sorted);
        });
    };

    //$('.draggable').makeDraggable();
    //$('.draggable').on('OnDropped', function (e, info, to) {
    //    for (i = 0; i < info.data.length; i++) {
    //        console.log("Track " + info.data[i].item + " was moved from playlist with name: " + info.data[i].name + ", to playlist with name: " + to);
    //    }
    //});
    //$('.draggable').on('OnCombined', function (e, combinedText) {
    //    console.log(combinedText);
    //});

    //$('.draggable-btn.add').click(function () {
    //    $(this).parent().trigger('add', ['Some text']);
    //});

    //$('.draggable-btn.sort').click(function () {
    //    $(this).parent().trigger('sort', [-1]);
    //});

    //$('.draggable-btn.shuffle').click(function () {
    //    $(this).parent().trigger('shuffle');
    //});
});