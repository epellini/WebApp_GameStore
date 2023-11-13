function fileCheck(obj) {
    var fileExtension = ['jpeg', 'jpg', 'png', 'bmp'];
    if ($.inArray($(obj).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
        console.log("Wrong file type");
        $('#pictureError').append(`<p class="text-aurora-red">Image format not accepted.</p>`);
        $('#picture').val('');  
    }
    else {
        var picture = $('#picture').prop('files')[0];  
        data = new FormData();
        data.append('picture', picture); 

        url = "upload-game-cover";
        $.ajax({
            type: "Post",
            url: url,
            data: data,
            enctype: 'multipart/form-data',
            processData: false,
            contentType: false,
            dataType: "json",
            async: true,
            success: function (result) {
                console.log(result);
                $("#picture").attr('src', `/images/user-uploaded/${result.pictureId}`); 
            }
        });
    }
}

$(document).on('click', '.form-check-input',
    function () {
        let data = '';

        if ($(this).hasClass('genre-check')) {
            let collection = $('.genre-check');
            for (var i = 0; i < collection.length; i++) {
                if (collection[i].checked) {
                    data += collection[i].value + ';'
                }
            }
            console.log(data);
            $('#Genres').val(data);
        }
        if ($(this).hasClass('platform-check')) {
            let collection = $('.platform-check');
            for (var i = 0; i < collection.length; i++) {
                if (collection[i].checked) {
                    data += collection[i].value + ';'
                }
            }
            console.log(data);
            $('#Platforms').val(data);
        }
        if ($(this).hasClass('language-check')) {
            let collection = $('.language-check');
            for (var i = 0; i < collection.length; i++) {
                if (collection[i].checked) {
                    data += collection[i].value + ';'
                }
            }
            console.log(data);
            $('#Languages').val(data);
        }
    });