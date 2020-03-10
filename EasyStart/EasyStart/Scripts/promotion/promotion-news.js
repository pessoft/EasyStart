
var NewsManager = {
    newsList: [],
    loadNewsList: function () {
        const self = this

        this.clearNewsList();
        if (this.newsList &&
            this.newsList.length > 0) {
            this.addAllItemNews(this.newsList);
        } else {
            let loader = new Loader($("#news-list"));
            let successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    if (!result.Data || result.Data.length == 0) {
                        self.setEmptyNewsInfo();
                    } else {
                        self.newsList = result.Data;
                        self.addAllItemNews(self.newsList);
                    }
                } else {
                    showErrorMessage(result.ErrorMessage);
                    self.setEmptyNewsInfo();
                }
            }
            loader.start();

            $.post("/Admin/LoadNewsList", null, successCallBack(successFunc, loader));
        }
    },
    setEmptyNewsInfo: function () {
        let template = `
        <div class="empty-list">
            <i class="fal fa-smile-plus"></i>
            <span>Пока нет новостей</span>
        </div>`;

        $("#news-list").append(template);
    },
    addAllItemNews: function (data) {
        for (let news of data) {
            this.addNewsToList(news);
        }
    },
    getNewsTileTemplate: function (oneNews) {
        let $template = $($("#news-item-template").html());
        const self = this

        $template.attr("news-id", oneNews.Id);
        $template.find("img").attr("src", oneNews.Image);
        $template.find(".news-item-title").html(oneNews.Title);
        $template.find(".news-remove").bind("click", function () {
            let callback = () => self.removeNews(oneNews.Id);

            deleteConfirmation(callback);
        });
        $template.find(".news-edit").bind("click", function () {
            self.editNews(oneNews.Id);
        });

        return $template
    },
    replaceNewsInList: function (oneNews, replaceNewsId) {
        let $template = this.getNewsTileTemplate(oneNews)

        $(`#news-list [news-id="${replaceNewsId}"]`).replaceWith($template);
    },
    addNewsToList: function (news) {
        let $template = this.getNewsTileTemplate(news)

        $("#news-list").append($template);
    },
    addNewNews: function () {
        this.cleanNewsDialog()
        this.showNews()
    },
    showNews: function (newsId) {
        const $newsDialog = $('#newsDialog')

        if (newsId)
            this.setNewsData(newsId)

        Dialog.showModal($newsDialog)
    },
    setNewsData: function (newsId) {
        const news = this.getNewsById(newsId)

        $('#newsDialog').attr('news-id', news.Id)

        $('#promotion-news-title').val(news.Title)
        $('#promotion-news-description').val(news.Description)

        const $img = $("#newsDialog img")
        $img.attr("src", news.Image)
        $img.removeClass('hide')

        $('#newsDialog .dialog-image-upload').addClass('hide')

        this.toggleSave()
    },
    saveNewsFromDiaolog: function () {
        const self = this
        const newsIdToRemove = parseInt($('#newsDialog').attr('news-id'))
        let loader = new Loader($("#newsDialog .custom-dialog-body"));
        loader.start();

        let files = $("#newsDialog input[type=file]")[0].files;
        var dataImage = new FormData();

        for (var x = 0; x < files.length; x++) {
            dataImage.append("file" + x, files[x]);
        }

        const news = {
            id: $('#newsDialog').attr('news-id'),
            title: $('#promotion-news-title').val(),
            description: $('#promotion-news-description').val(),
            image: ''
        }

        const saveFunc = function (data) {
            news.image = data.URL

            const successFunc = function (result, loader) {
                loader.stop();
                if (result.Success) {
                    $("#news-list .empty-list").remove();

                    if (!Number.isNaN(newsIdToRemove) && newsIdToRemove > 0) {
                        const index = self.getIndexNewsById(newsIdToRemove)
                        self.newsList[index] = result.Data

                        self.replaceNewsInList(result.Data, newsIdToRemove)

                    } else {
                        if (self.newsList.length == 0)
                            self.clearNewsList()

                        self.newsList.push(result.Data);
                        self.addNewsToList(result.Data);
                    }

                    cancelDialog("#newsDialog");
                } else {
                    showErrorMessage(result.ErrorMessage)
                }
            }

            $.post("/Admin/SaveNews", news, successCallBack(successFunc, loader))
        }

        if (files.length == 0) {
            let data = {
                URL: $("#newsDialog img").attr("src")
            }

            saveFunc(data);

            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Admin/UploadImage',
            contentType: false,
            processData: false,
            data: dataImage,
            success: function (data) {
                saveFunc(data)
                cleatPrevInputImage()
            },
            error: function () {
                cleatPrevInputImage()
            }
        });
    },
    getIndexNewsById: function (searchId) {
        for (let id in this.newsList) {
            if (this.newsList[id].Id == searchId) {
                return id;
            }
        }
    },
    clearNewsList: function () {
        $("#news-list").empty();
    },
    cleanNewsDialog: function () {
        const $dialog = $('#newsDialog')

        $dialog.attr('news-id', -1)
        $dialog.find('#add-news-image').val('')
        const $img = $dialog.find('img')
        $img.attr('src', '')
        $img.addClass('hide')
        $dialog.find('.dialog-image-upload').removeClass('hide')
        $dialog.find('#btn-news-save').attr('disabled', true)
        $dialog.find('#promotion-news-title').val('')
        $dialog.find('#promotion-news-description').val('')
    },
    editNews: function (id) {
        this.cleanNewsDialog()
        this.showNews(id)
    },
    getNewsById: function (searchId) {
        for (let id in this.newsList) {
            if (this.newsList[id].Id == searchId) {
                return this.newsList[id];
            }
        }
    },
    removeNews: function (id) {
        const self = this
        $(`.news-item[news-id=${id}]`).fadeOut(1000, function () {
            $(this).remove();

            const newsIndex = self.getIndexNewsById(id);
            self.newsList.splice(newsIndex, 1);

            if (self.newsList.length == 0)
                self.setEmptyNewsInfo()
        });

        $.post("/Admin/RemoveNews", { id: id }, null);
    },
    toggleSave: function () {
        const isImageValid = !!$('#news-image').attr('src')
        const isTitleValid = !!$('#promotion-news-title').val()
        const isDescriptionValid = !!$('#promotion-news-description').val()

        if (isImageValid &&
            isTitleValid &&
            isDescriptionValid)
            $('#btn-news-save').removeAttr('disabled')
        else
            $('#btn-news-save').attr('disabled', true)
    }
}