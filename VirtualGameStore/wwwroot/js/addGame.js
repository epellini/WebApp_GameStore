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