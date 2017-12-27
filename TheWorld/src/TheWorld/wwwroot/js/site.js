// site.js. 
//Nameless function that executes immediately. Self-Executed Anonymous Function Or Immediately Invoked Function Expression to avoid the global scope problem.
// by wrapping it up in parenthesis, it becomes a big expression.
(function () {

    // JQuery exposes a single object to the Global Scope, and that global object is JQuery aliased in the Global Scope as dollar sign $

    //var ele = document.getElementById("username");
    //ele.innerHTML = "Carlos Vasquez"; //replace the text that is inside of that span, the HTML content of that span

    //JQuery instead plain javascript file
    //var ele = $("#username");
    //ele.text("Shawn Wildermuth");

    //var main = document.getElementById("main");
    // pattern of handling event by using callback functions is a pretty common one in Javascript.
    //main.onmouseenter = function () {
    //    main.style.background = "#888"; // which is a gray.
    //};

    //var main = $("#main");
    //main.on("mouseenter", function () {           This is called callback functions in JavaScript
    //    main.style = "background-color: #888;";  Gray Color
    //});

    //main.onmouseleave = function () {
    //    main.style.background = "";
    //};

    //main.on("onmouseleave", function () {
    //    main.style = "";
    //});

    // the unorder list with the Class of menu, li for the list item, and a for the anchor
    //var menuItems = $("ul.menu li a");
    //menuItems.on("click", function () {
    //    var me = $(this);
    //    alert(me.text());
    //});

    // get me the sidebar and wrapper and return it as what's called wrapped set of DOM elements.
    var $sidebarAndWrapper = $("#sidebar, #wrapper");
    var $icon = $("#sidebarToggle i.fa");            // it says go find the sidebarToggle, and as one of their children of it, find an italic that is classed with, you guessed it fa.

    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar"); // Method toggleClass it will add a class if it doesn't exist, or remove the class if it does exist. it eliminates the needs to add class and remove class.
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            //$(this).text("Hide Sidebar");
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            //$(this).text("Display Sidebar");
            $icon.addClass("fa-angle-left");
            $icon.removeClass("fa-angle-right");
        }
    });

})();     // this expression returns a function, and to execute it, we add two parenthesis to say execute. 
          // it execute outside of the global scope, so we don't have to worry about all that name collission nuisance.
