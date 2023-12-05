window.onload = function () {
    //var txt = document.getElementById('txt');
    //document.getElementById('link').onclick = function (code) {
    //    this.href = 'data:text/plain;charset=utf-11,' + encodeURIComponent(txt.value);
    //};
    $('.download-btn').on("click", function () {
        $(this).attr('href', 'data:text/plain;charset=utf-11,' + encodeURIComponent($(this).attr('description')));
    });
};
