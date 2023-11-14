$(document).ready(function () {
    jQuery(function () {
        
        if ($('#GenderInput').val() != '') {
            var elements = document.getElementsByClassName("nav-link-snow");
            let elFound = false
            for (var i = 0; i < elements.length; i++) {
                if ($('#GenderInput').val() === elements[i].innerHTML) {
                    elements[i].classList.add("nav-link-snow-active");
                    elFound = true;
                }
            }
            if (!elFound) {
                for (var i = 0; i < elements.length; i++) {
                    if (elements[i].innerHTML === 'My Gender Is...') {
                        elements[i].classList.add("nav-link-snow-active");
                    }
                }
            }
        }
    });
});

function selectGender(element) {
    var elements = document.getElementsByClassName("nav-link-snow");
    for (var i = 0; i < elements.length; i++) {
        if (elements[i].classList.contains("nav-link-snow-active")) {
            elements[i].classList.remove("nav-link-snow-active");
        }
    }
    if (element.innerHTML !== 'My Gender Is...') {
        element.classList.add("nav-link-snow-active");
        $('#GenderInput').val(element.innerHTML);
    }
    else {
        $('#GenderInput').val('');
        $('#GenderInput').focus();
    }
}

$('#btnChangePhoto').on('click', function () {
    $('#NewPhoto').click();
});

function fileCheck(obj) {
    var fileExtension = ['jpeg', 'jpg', 'png', 'bmp'];
    if ($.inArray($(obj).val().split('.').pop().toLowerCase(), fileExtension) == -1) {
        console.log("Wrong file type");
        $('#photoError').append(`<p class="text-aurora-red">Image format not accepted.</p>`);
        $('#NewPhoto').val('');
    }
    else {
        var photo = $('#NewPhoto').prop('files')[0];
        let profileId = $('#ProfileId').val();
        let userId = $('#UserId').val();
        data = new FormData();
        data.append('NewPhoto', photo);
        data.append('ProfileId', profileId);
        data.append('UserId', userId);
        url = "upload-photo";
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
                $("#ProfilePic").html('');
                console.log(result);
                $("#ProfilePic").append(`<img src="/images/user-uploaded/${result.photoId}" class="profile-pic p-0" alt="Profile Picture" style="object-fit: cover"/>`);
            }
        });
    }
}
