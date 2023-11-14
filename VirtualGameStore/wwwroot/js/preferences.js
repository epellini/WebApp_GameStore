// Toggle the visibility of the all platforms list using jquery
function togglePlatforms() {
    
    $("#AllPlatformsList").toggleClass('d-none');
    if ($("#TogglePlatformsBtn").text().trim() === "Add More") {
        $("#TogglePlatformsBtn").html(`Hide All <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right-fill mb-1" viewBox="0 0 16 16"><path d="m7.247 4.86-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z"/></svg > `);
    }
    else {
        $("#TogglePlatformsBtn").html(`Add More <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right-fill mb-1" viewBox="0 0 16 16"><path d="M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" /></svg > `);
    }
}
function toggleGenres() {

    $("#AllGenresList").toggleClass('d-none');
    if ($("#ToggleGenresBtn").text().trim() === "Add More") {
        $("#ToggleGenresBtn").html(`Hide All <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right-fill mb-1" viewBox="0 0 16 16"><path d="m7.247 4.86-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z"/></svg > `);
    }
    else {
        $("#ToggleGenresBtn").html(`Add More <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right-fill mb-1" viewBox="0 0 16 16"><path d="M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" /></svg > `);
    }
}
function toggleLanguages() {

    $("#AllLanguagesList").toggleClass('d-none');
    if ($("#ToggleLanguagesBtn").text().trim() === "Add More") {
        $("#ToggleLanguagesBtn").html(`Hide All <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right-fill mb-1" viewBox="0 0 16 16"><path d="m7.247 4.86-4.796 5.481c-.566.647-.106 1.659.753 1.659h9.592a1 1 0 0 0 .753-1.659l-4.796-5.48a1 1 0 0 0-1.506 0z"/></svg > `);
    }
    else {
        $("#ToggleLanguagesBtn").html(`Add More <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-caret-right-fill mb-1" viewBox="0 0 16 16"><path d="M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z" /></svg > `);
    }
}

$(document).on('mouseenter', '.pref-link',
    function() {
        $(this).find("svg").toggleClass("d-none");
        $(this).toggleClass("px-4").toggleClass("px-3");
    }).on('mouseleave', '.pref-link',
    function () {
        $(this).find("svg").toggleClass("d-none");
        $(this).toggleClass("px-4").toggleClass("px-3");
    }
);

$(document).on('click', '.pref-link',
    function () {
        checkButtons();
        let name = $(this).text().trim();

        let className = '';
        if ($(this).hasClass("platform-link")) {
            className = 'platform-link';
        }
        else if ($(this).hasClass("genre-link")) {
            className = 'genre-link';
        }
        else if ($(this).hasClass("language-link")) {
            className = 'language-link';
        }
        
        $(this).closest(".selected-list").siblings(".unselected-list").find(".row").prepend(`
                                            <div class="col-auto p-0 m-0">
                                                <a role="button" onclick="" class="nav-link ${className} sort-link-snow p-1 px-3 my-2 m-1 text-uppercase fw-bold">${name}</a>
                                            </div>`
        );

        if ($(this).parent().siblings().length === 0) {
            $(this).closest(".selected-list").find(".row").append(`<div class="col-auto p-0 m-0"><p class="ms-2 mb-0">None...</p></div>`);
        }

        let collection = $(this).parent().siblings().children("a");
        let data = '';

        if (collection !== null) {
            for (var i = 0; i < collection.length; i++) {
                data += collection[i].text.trim() + ';';
            }
            if (data !== '') {

                //console.log("collection updated: " + data)
            }
            else {
                //console.log("collection empty.")
            }
        }

        updateModel(className, data);

        $(this).parent().remove();

        
    }
);

$(document).on('click', '.sort-link-snow',
    function () {
        checkButtons();
        let name = $(this).text().trim();

        let className = '';
        if ($(this).hasClass("platform-link")) {
            className = 'platform-link';
        }
        else if ($(this).hasClass("genre-link")) {
            className = 'genre-link';
        }
        else if ($(this).hasClass("language-link")) {
            className = 'language-link';
        }

        $(this).closest(".unselected-list").siblings(".selected-list").find(".row").append(`
                                            <div class="col-auto p-0 m-0">
                                                <a role="button" onclick="" class="nav-link ${className} pref-link pref-link-new p-1 px-4 my-2 m-1 text-uppercase fw-bold">${name}
                                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" class="bi bi-caret-right-fill mb-1 d-none" viewBox="0 0 16 16">
                                                            <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z" />
                                                            <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z" />
                                                </svg>
                                                </a>
                                            </div>`
        ).find("p").parent().remove();

        let collection = $(this).closest(".unselected-list").siblings(".selected-list").find(".row").find("a");
        let data = '';

        if (collection !== null) {
            for (var i = 0; i < collection.length; i++) {
                data += collection[i].text.trim() + ';';
            }
            if (data !== '') {

                //console.log("collection updated: " + data)
            }
            else {
                //console.log("collection empty.")
            }
        }

        updateModel(className, data);

        $(this).parent().remove();
    }
);

function checkButtons() {
    if ($(".btn-aurora-red-outline").text().trim() !== "Cancel") {
        $(".btn-aurora-red-outline").text("Cancel");
    }
    if ($(".btn-frost-primary").hasClass("d-none")) {
        $(".btn-frost-primary").removeClass("d-none");
    }
}

function updateModel(list, data) {
    if (list === 'platform-link') {
        $("#Platforms").val(data);
        //console.log($("#Platforms").val());
    }
    else if (list == 'genre-link') {
        $("#Genres").val(data);
        //console.log($("#Genres").val());
    }
    else if (list == 'language-link') {
        $("#Languages").val(data);
        //console.log($("#Languages").val());
    }
}
