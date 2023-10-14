// Function to add nav-link-snow-active class to the element that was clicked
function addActiveClass(element) {
    console.log("remove class");
    // First remove the nav-link-snow-active class from any element that has it
    removeActiveClass();
    // Then add the nav-link-snow-active class to the element that was clicked
    element.classList.add("nav-link-snow-active");
}

// Function to remove nav-link-snow-active class from any element that has it
function removeActiveClass() {
    var navLinks = document.querySelectorAll(".nav-link-snow-active");
    console.log("remove class");
    navLinks.forEach(function (navLink) {
        navLink.classList.remove("nav-link-snow-active");
    });
}