//  app-trip.js - conventionally the trip view of the app controller.
// Immediate Invoking Function Expression IIFE take all the code we are writing out of the global scope.
// Main module of Angular.
(function () {

    "use strict";

    // The second parameter makes a difference.  tells Angular, hey this is where I am Creating the module, the  . The difference between this angular.module and the one from tripsController is this. 
    // [] Empty array. this is how Dependencies are managed.
    angular.module("app-trips", ["simpleControls", "ngRoute"])    // this is going to create a module for us. This is the module for taking care of our whole page. SimpleControls is added as a Dependency.
    .config(function ($routeProvider) {                                         // the config takes a callback function. $routeProvider is the one who's going to allow client side routes.
                                                    // $routeProvider parm injection. property starts with Dollar sgn
        $routeProvider.when("/", {
            controller: "tripsController",
            controllerAs: "vm",
            templateUrl: "/views/tripsView.html"
        });

        $routeProvider.when("/editor/:tripName", {
            controller: "tripEditorController",
            controllerAs: "vm",
            templateUrl: "/views/tripEditorView.html"
        });

        //what to do is none of the routes match
        $routeProvider.otherwise({ redirecto: "/" });       // so if any route that isn't a match when route provider is called, simply redirecto to the first main route.
    });

})();
