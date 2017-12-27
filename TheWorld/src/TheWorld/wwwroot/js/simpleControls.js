// simpleControls.js. Creating a new module with a directive to reuse code across the app.
(function () {
    "use strict"

    // we will generate a new module "simpleControls"
    angular.module("simpleControls", [])        // create the module.
        .directive("waitCursor", waitCursor);   // Creating the directive. the waitCursor directive. A reusable component to show a waitCursot. Notice the camelCasing for the name of the directive. it ended up with wait-cursor, all lower-case with the dash in between.

    function waitCursor () {
        return {                                // return an object with some well-know properties such as scope, and templateUrl. Remember this is all Client side, so it is looking at wwwroot
            scope: {                            // the object we are binging our object to.  show is what is visible inside the template.
                show: "=displayWhen"            // =displayWhen is what is going to use from the consumer of that directive Trips.cshtml
            },
            restrict: "E",
            templateUrl: "/views/waitCursor.html"
        };
    }

})();