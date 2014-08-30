$(document).ready(function () {

    $.fn.makeDraggable = function (options) {
        var moveTrackToNewPosition = options.onMoveTrackToNewPosition;
        var contextMenu = options.contextMenu;
        var saveVkTrackToPlaylist = options.saveVkTrack;
        var subContextMenu = options.showSubContextMenu;
        var movingBetweenPlaylists = options.movingTracksBetweenPlaylists;

        var $rootElement = this;
        var $currentItem = null;
        var $draggableStub = $('#draggableStub');
        var $container = $('#itemsContainer');
        var $contextMenuContainer = $('<div class="subContextMenu contextMenu">' +
                                         '</div>');

        var $subContextMenuContainer = $('<div class="contextMenu subMenu"></div>');
        var $contextMenuTemplate = $('#contextmenuTemplate');
        var $subContextMenuTemplate = $('#subContextmenuTemplate');

        var selectAllAction = null;
        var copyToPlaylistAction = null;
        var moveToPlaylistAction = null;
        var createPlaylistAction = null;
        var hideAction = null;
        var removeAction = null;

        var movingInfo = { "data": [] };
        var timerId = null;
        var lastEvent = null;
        var elementOffset = { x: 0, y: 0 };
        var mousedown = false;
        var deleteFlag = false;
        var contextMenuSelected = false;

        this.find('.draggable-list > .tableRow').each(function (index, item) {
            var $item = $(item);
            $item.toggleClass('draggable-item', true);
            $item.mousedown(_makeDraggable);
        });

        $(document).mousemove(function (event){
            lastEvent = event;
            //var $elem = _getCurrentTarget(event);
            if ($currentItem && mousedown) {
                var $elem = _getCurrentTarget(event);

                if ($currentItem.hasClass('draggable-item-selected')) {
                    var width = $currentItem.width();
                    $currentItem = $container;
                    $currentItem.css({
                        'width': width + 'px'
                });
                    $currentItem.append($('.draggable-item-selected'));
                }
                $currentItem.toggleClass('dragging', true);

                $draggableStub.show();

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
            }
        });

        $(document).mouseup(function () {
            mousedown = false;
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

                    if (!$currentItem.children().hasClass('vk-item') && $element.parent().attr('id') == 'playlistTracks' && !$('.draggable-stub').is(':hidden')) {
                        /moveTrackToNewPosition($currentItem, $draggableStub);
                        cop
                    }

                    if ($currentItem.children().hasClass('vk-item') && !$element.hasClass('vk-item') && !$element.parent().hasClass('vkMusicList')) {
                        var playlistId = -1;
                        if ($element.hasClass('playlist')) {
                            playlistId = $element.children('.playlistId').text();
                        } else {
                            playlistId = $('.playlist.active').children('.playlistId').text();
                            //index = $draggableStub.index();
                        }
                        saveVkTrackToPlaylist($currentItem, $draggableStub.index(), playlistId); // Maybe problem there
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
                        $currentItem.children().insertAfter($draggableStub);
                        $currentItem.trigger('AfterDropped', [movingInfo, $draggableStub.parent().attr("name")]);
                        _clearContainer();
                    }
                }
            }
            $draggableStub.hide();
        });


        for (var i = 0; i < contextMenu.length; i++) {
            var object = $contextMenuTemplate.tmpl(contextMenu[i]);
            switch (contextMenu[i].id) {
                case 'selectall':
                    selectAllAction = contextMenu[i].callback;
                    break;
                case 'copytoplaylist':
                    copyToPlaylistAction = contextMenu[i].callback;
                    break;
                case 'movetoplaylist':
                    moveToPlaylistAction = contextMenu[i].callback;
                    break;
                case 'removeselected':
                    removeAction = contextMenu[i].callback;
                    break;
                case 'hideselected':
                    hideAction = contextMenu[i].callback;
                    break;
                case 'createplaylist':
                    createPlaylistAction = contextMenu[i].callback;
            }

            if (contextMenu[i].hasSubMenu == true) {
                // WRONG!!!!
                object.append(">");
                object.toggleClass('hasSubMenu', true);
            }
            if (contextMenu[i].isNewSection == true) {

                $contextMenuContainer.append("<hr/>");
            }

            object.appendTo($contextMenuContainer);
        }

        this.mousedown(function (e) {
            var $target = $(e.target);
             if (contextMenuSelected == true && e.which != 3 && !$target.parent().hasClass('hasSubMenu')) {
                 $contextMenuContainer.hide();
                 $subContextMenuContainer.hide();
             }
             if ($target.hasClass('contextMenuActionName')) {
                 if ($target.parents().hasClass('subMenu')) {

                     var index = -1;

                     $currentItem = $container;
                     $currentItem.append($('.draggable-item-selected'));
                     if ($currentItem.children().length > 0) {
                         saveVkTrackToPlaylist($currentItem, index, $target.parent().children('.playlistId').text());
                     }
                 }


                var id = $target.attr('id');
                switch (id) {
                    case 'selectall':
                        selectAllAction($(this).find('.track'));
                        break;
                    case 'copytoplaylist':
                        showSubContextMenuAction($subContextMenuContainer, $(this));
                        break;
                    case 'movetoplaylist':
                        showSubContextMenuAction($subContextMenuContainer, $(this));
                        break;
                    case 'removeselected':
                        removeAction();
                        break;
                    case 'hideselected':
                        hideAction($(this).find('.track.draggable-item-selected'));
                        break;
                    case 'createplaylist':
                        createPlaylistAction();
                }
            }
        });

        $(document).mousedown(function (e) {
            var $target = $(e.target);
            if (!$target.parents().hasClass('draggable-list')) {
                document.oncontextmenu = function() {
                    return true;
                }
            }
        });

        function _makeDraggable(event) {

            if (event.which == 3) {
                document.oncontextmenu = function() {
                    return false;
                }
                contextMenuSelected = true;
                var $target = $(event.target).parents('.draggable-list').parent();
                var parentOffset = $target.parent().offset();
                    var x = event.pageX - parentOffset.left;
                    var y = event.pageY - parentOffset.top;
                    $contextMenuContainer.css({
                        'top': y + 'px',
                        'left': x + 'px'
                    });

                    $target.append($contextMenuContainer);
                    $contextMenuContainer.show();

                    //$rootElement.find('.contextMenu').off(':hover').on(':hover', function () {
                    //    alert("in");
                    //});

                //$rootElement.find('.contextMenu').hover(function () {
                    $rootElement.find('.contextMenu .tableRow').hover(function () {
                        var self = $(this);
                        var $elem = $('.tableRow:hover');
                            if ($elem.hasClass('hasSubMenu')) {

                                var contextMenuOffset = $('.contextMenu .tableRow:hover').parent().position();
                                var contextMenuItemOffset = $('.contextMenu .tableRow:hover').position();

                                if (contextMenuItemOffset != undefined) {
                                    $subContextMenuContainer.css({
                                        'top': contextMenuOffset.top + contextMenuItemOffset.top + 'px',
                                        'left': contextMenuOffset.left + $('.contextMenu .tableRow:hover').width() + 'px',
                                        'position': 'absolute'
                                    });

                                    var $playlists = $('#playlistsTable').children('.playlist');
                                    $subContextMenuContainer.children().remove('.tableRow');
                                    for (var j = 0; j < $playlists.length; j++) {
                                        var playlist = {
                                            "name": $($playlists[j]).children('.playlist-title').text(),
                                            "id": $($playlists[j]).children('.playlistId').text()
                                        };
                                        //var object = $contextMenuTemplate.tmpl(contextMenu[i]);
                                        $subContextMenuContainer.remove('.tableRow');
                                        var object = $subContextMenuTemplate.tmpl(playlist);
                                        object.appendTo($subContextMenuContainer);
                                    }

                                    self.parent().parent().parent().append($subContextMenuContainer);
                                    $subContextMenuContainer.show();
                                }
                            }
                    });

                    $rootElement.find('.contextMenu .tableRow').mouseleave(function (e) {
                        subContextMenu($subContextMenuContainer, $(this), $(e.toElement));
                        //var self = $(this);
                        //var $toElement = $(e.toElement);
                        //if (self.hasClass('hasSubMenu') && $toElement.parents('.subMenu').length < 1) {
                        //    $subContextMenuContainer.hide();
                        //}
                    });
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
                    } else if ($('.draggable-item-selected').length > 1) {
                        $('.draggable-item-selected:not(:hover)').toggleClass('draggable-item-selected', false);
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
                    return $elem;
                }
                if ($elem.parents('.playlist').length > 0 && event.type == "mouseup") {
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

        $('.draggable').on('MergeItems', function(e, mergeTo, mergeElem, pageY) {
            mergeToPos = mergeTo.offset();
            if ((!mergeTo.hasClass('draggable-stub')) && (mergeToPos.top + 0.15 * $('.draggable-item').height() < pageY) && (mergeToPos.top + 0.85 * $('.draggable-item').height() > pageY)) {
                if (mergeElem && mergeTo) {
                    if ($currentItem.hasClass('itemsContainer') && $('.draggable-item-selected').length > 0) {
                        $currentItem.children().each(function(index, item) {
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

        $('.container').on('delete', function (e) {
            $currentItem.empty();
            _clearContainer();
            deleteFlag = false;
            _setDeleteAreaCss();
        });

        $('.draggable-list').on('shuffle', function() {
            var $this = $(this);
            var elems = $this.children('li');
            elems.sort(function() { return (Math.round(Math.random()) - 0.5); });
            $this.remove(elems[0].tagName);
            $this.prepend(elems);
        });

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

        //$contextMenuContainer.children('.tableRow').children().hover(function () {
        //    var self = $(this);
        //    if (self.parent().hasClass('hasSubMenu')) {

        //        var contextMenuOffset = $('.contextMenu .tableRow:hover').parent().position();
        //        var contextMenuItemOffset = $('.contextMenu .tableRow:hover').position();

        //        if (contextMenuItemOffset != undefined) {
        //            $subContextMenuContainer.css({
        //                'top': contextMenuOffset.top + contextMenuItemOffset.top + 'px',
        //                'left': contextMenuOffset.left + $('.contextMenu .tableRow:hover').width() + 'px',
        //                'position': 'absolute'
        //            });
        //            self.parent().parent().parent().append($subContextMenuContainer);
        //            $subContextMenuContainer.show();
        //        }
        //    }
        //});
    };
});