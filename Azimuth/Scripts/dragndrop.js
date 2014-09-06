$(document).ready(function () {

    // Counter to different context menus on for plugin owners on one page
    var count = 0;
    var mouseClickPosition;
    $.fn.makeDraggable = function (options) {
        var $rootElement = this;
        // Callback on dragging item in new position in list
        var moveTrackToNewPosition = options.onMoveTrackToNewPosition;
        var $currentItem = null;
        //var $draggableStub = $('#draggableStub');
        var $container = $('#itemsContainer');
        // Call to create unique plugin owner id
        this._getCount = function () {
            return count++;
        };
        var contextMenu = new ContextMenu();
        contextMenu.initializeContextMenu(this._getCount(), options.contextMenu, this, options.manager);
        var $draggableStub = $('<div>');
        $draggableStub.addClass('tableRow');
        $draggableStub.addClass('draggable-item');
        $draggableStub.addClass('draggable-stub');
        $draggableStub.addClass('track');
        // Saving data about item moving (from -> to)
        var movingInfo = { "data": [] };
        var timerId = null;
        var lastEvent = null;
        var elementOffset = { x: 0, y: 0 };
        var mousedown = false;
        var deleteFlag = false;
        var moveEnable = false;
        // Mousemove start working after monig to mouseMovePxOffset pixels
        var mouseMovePxOffset = 10;

        var mousedownOnProgressBar = false;

        this.find('.draggable-list > .tableRow').each(function (index, item) {
            var $item = $(item);
            $item.toggleClass('draggable-item', true);
            $item.mousedown(_makeDraggable);
        });

        $(document).mousemove(function (event) {
            lastEvent = event;
            if ($currentItem && mousedown && !mousedownOnProgressBar) {
                var $elem = _getCurrentTarget(event);

                var currentMousePos = { x: event.clientX, y: event.clientY };
                if (moveEnable == false) {
                    if (Math.abs(mouseClickPosition.x - currentMousePos.x) > mouseMovePxOffset || Math.abs(mouseClickPosition.y - currentMousePos.y) > mouseMovePxOffset) {
                        moveEnable = true;
                    } else {
                        return;
                    }
                }

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

                        if ($elem.length > 0 &&
                            (!$elem.parents().hasClass('vkMusicList') || ($currentItem.children().hasClass('vk-item') && $elem.parents().hasClass('vkMusicList')))) {
                            clearTimeout(timerId);
                            var $savedItem = $currentItem;
                            timerId = setTimeout(function () {
                                var $hoverElement = _getCurrentTarget(lastEvent);
                                if ($elem.is($hoverElement)) {
                                    $elem.trigger('MergeItems', [$elem, $savedItem, event.pageY]);
                                }
                            }, 2000);
                            if (childPos && parentPos && childPos.top - parentPos.top < $($currentItem.children[0]).outerHeight() / 2 &&
                                (!$elem.parents().hasClass('vkMusicList') || ($currentItem.children().hasClass('vk-item') && $elem.parents().hasClass('vkMusicList')))) {
                                $draggableStub.insertBefore($elem);
                            } else if (!$elem.parents().hasClass('vkMusicList') || ($currentItem.children().hasClass('vk-item') && $elem.parents().hasClass('vkMusicList'))) {
                                $draggableStub.insertAfter($elem);
                            }
                        }
                    }
                }
                $draggableStub.show();
            }
        });

        $(document).mouseup(function (event) {
            if ($currentItem != null && !event.shiftKey && !event.ctrlKey && !$currentItem.hasClass('itemsContainer') && event.which != 3) {
                if ($('.draggable-item-selected').length > 1) {
                    $('.draggable-item-selected:not(:hover)').toggleClass('draggable-item-selected', false);
                }
            }

            mousedown = false;
            mousedownOnProgressBar = false;
            clearTimeout(timerId);
            if ($currentItem) {
                var $element = _getCurrentTarget(event);
                if ($element.length == 0) {
                    if ($currentItem.hasClass('itemsContainer')) {
                        $draggableStub.replaceWith($currentItem.children());
                        $currentItem = null;
                    } else {
                        $currentItem.removeAttr('style');
                        $currentItem.toggleClass('dragging', false);
                    }
                } else {

                    if (!$currentItem.children().hasClass('vk-item') && $element.parent().attr('id') == 'playlistTracks' && !$('.draggable-stub').is(':hidden') && moveEnable) {

                        $currentItem.children().insertAfter($draggableStub);
                        $draggableStub.detach();
                        //contextMenu._moveTrackToNewPosition('.draggable-list');
                        moveTrackToNewPosition($rootElement.find('.draggable-list'));
                        _clearContainer();
                        moveEnable = false;
                        return;
                    }
                    moveEnable = false;

                    if ($currentItem.children().hasClass('vk-item') && !$element.hasClass('vk-item') && !$element.parent().hasClass('vkMusicList')) {
                        var playlistId = -1;
                        if ($element.hasClass('playlist')) {
                            playlistId = $element.children('.playlistId').text();
                        } else if ($element.hasClass('track')) {
                            playlistId = $element.parent().children('.playlistId').text();
                        }
                        var index = -1;
                        if (!$draggableStub.parent().hasClass('vkMusicList')) {
                            index = $draggableStub.index();
                        }
                        contextMenu._saveTrackFromVkToPlaylist($currentItem, index, playlistId);
                        //saveVkTrackToPlaylist($currentItem, index, playlistId); //////////////////////////////////////////////????????????????????????????????????????????
                    }

                    if ($element.hasClass('delete-area')) {
                        $currentItem.trigger("delete");
                    }

                    if ($currentItem != null && ($currentItem.hasClass('itemsContainer') && $currentItem.children().length > 0) > 0 && !$element.hasClass('delete-area')) {
                        var $items = "Items ";
                        for (i = 0; i < $currentItem.children().length; i++) {
                            $items += $($currentItem.children()[i]).html() + " ";
                        }
                        $currentItem.trigger('OnDropped', [movingInfo, $draggableStub.parent().attr("name")]);
                        movingInfo.data.length = 0;
                        if ($currentItem.children().length > 0) {
                            $currentItem.children().insertAfter($draggableStub);
                            $currentItem.trigger('AfterDropped', [movingInfo, $draggableStub.parent().attr("name")]);
                            _clearContainer();
                        }
                    }
                }
            }
            $draggableStub.hide();
        });

        contextMenu.$contextMenuContainer.mousedown(function (event) {
            contextMenu.selectAction($currentItem);
        });

        $(document).mousedown(function (event) {
            var $target = $(event.target);
            if ($target.hasClass('progressBar') || $target.hasClass('progress') || $target.hasClass('cache')) {
                mousedownOnProgressBar = true;
            }
            //if (!$target.parents().hasClass('draggable-list')) {
            if (!$target.parents().hasClass('draggable-list') && !$target.parents().hasClass('list')) {
                document.oncontextmenu = function () {
                    return true;
                }
            }
            if (!$target.hasClass('contextMenuActionName') && event.which != 3) {
                $('.contextMenu').detach();
            }
        });

        function _makeDraggable(event) {
            mouseClickPosition = { x: event.clientX, y: event.clientY };
            if (event.which == 3) {

                var $currentList = $(event.target);
                $currentList = $currentList.parents('.draggable-list');
                if ($currentList.children().length > 0) {
                    if ($currentList.children('.draggable-item-selected').length == 0) {
                        $('.draggable-item-selected').toggleClass('draggable-item-selected', false);
                    }
                }

                contextMenu.drawContextMenu(event);
            } else {

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
                    if ($('.draggable-item-selected').last().index() < $currentItem.index()) {
                        indexFirst = $('.draggable-item-selected').last().index();
                        indexLast = $currentItem.index();
                    } else {
                        indexFirst = $currentItem.index();
                        indexLast = $('.draggable-item-selected').last().index();
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
                    var $selectedItems = $('.draggable-item-selected');

                    if (!$currentItem.parent().children().hasClass('draggable-item-selected') && $('.draggable-item-selected').length > 0) {
                        $selectedItems.toggleClass('draggable-item-selected', false);
                    } else if ($currentItem.parent().children().hasClass('draggable-item-selected') && $('.draggable-item-selected').length > 0 && !$currentItem.hasClass('draggable-item-selected')) {
                        $selectedItems.toggleClass('draggable-item-selected', false);
                    } else if ($currentItem.hasClass('draggable-item-selected') && $('.draggable-item-selected').length < 2) {
                        $selectedItems.toggleClass('draggable-item-selected', false);
                    } else if ($('.draggable-item-selected').length == 1) {
                        $selectedItems.toggleClass('draggable-item-selected', false);
                    }
                    $currentItem.toggleClass('draggable-item-selected', true);
                }
            }
        };

        function _setRelativePosition(event) {
            var relX = event.pageX - elementOffset.x;
            var relY = event.pageY - elementOffset.y;
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

            if (event.type == "mouseup") {
                if ($elem.hasClass('playlist')) {
                    $currentItem.show();
                    return $elem;
                }
                if ($elem.parents('.playlist').length > 0 && event.type == "mouseup") {
                    $currentItem.show();
                    return $elem.parents('.playlist');
                }
            }
            if ($elem.hasClass('delete-area'))
                _onDeleteArea();
            else if (deleteFlag == true) {
                _outDeleteArea();
            }
            $currentItem.show();

            if ($elem.hasClass('delete-area')) {
                return $elem;
            }

            if ($elem.closest('.draggable-list').find('.draggable-item').length === 0) {
                return $elem.closest('.draggable-list');
            }

            if ($elem.attr('id') == "playlistTracks" && event.type == "mouseup")
                return $($elem.children()[0]);

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
                });
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
        });

        //$('.container').on('delete', function (e) {
        //    $currentItem.empty();
        //    _clearContainer();
        //    deleteFlag = false;
        //    _setDeleteAreaCss();
        //});

        //$('.draggable-list').on('shuffle', function() {
        //    var $this = $(this);
        //    var elems = $this.children('li');
        //    elems.sort(function() { return (Math.round(Math.random()) - 0.5); });
        //    $this.remove(elems[0].tagName);
        //    $this.prepend(elems);
        //});

        //$('.draggable-list').on('add', function (e, data) {
        //    var $newItem = $('<li>').toggleClass('draggable-item', true).text(data);
        //    $newItem.mousedown(_makeDraggable);
        //    $(this).prepend($newItem);
        //});

        //$('.draggable-list').on('sort', function (e, dir) {
        //    $list = $(this);
        //    var sorted = $list.find("> .draggable-item").sort(function (a, b) {
        //        return $(a).text().toLowerCase() > $(b).text().toLowerCase() ? dir : -dir;
        //    });
        //    $list.prepend(sorted);
        //});
    };
});