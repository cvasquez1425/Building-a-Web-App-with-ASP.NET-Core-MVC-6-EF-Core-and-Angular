// tripsController.js Separate from the global scope
(function () {

    "use strict";

    // Getting the existing module
    angular.module("app-trips")
        .controller("tripsController", tripsController);  // Create a controller name tripsController, and use this function.

    function tripsController($http) {
        // expose some data from the controller.

        var vm = this;          // this represents the object that is returned from the tripsController, something like a class.

        //vm.name = "Shawn";

        //vm.trips = [{
        //    name: "US Trips",
        //    created: new Date()
        //}, {
        //    name: "World Trips",
        //    created: new Date()
        //}];

        vm.trips = [];

        // new member to our vm
        vm.newTrip = {};

        // new member of the View Model vm
        vm.errorMessage = "";
        vm.isBusy = true;           // busy flag member. Wait for the operation to complete.

        // Retrive data from the API using Angular. What is returned from the promise object is .then for call back for success and failure.
        $http.get("/api/trips")                             // relative service name.
            .then(function (response) {                     // The actual payload we got from the server. it's already being converted from JSON to an object.
                // Success
                angular.copy(response.data, vm.trips);
            }, function (error) {
                //Failure
                vm.errorMessage = "Failed to load data: " + error;
            })
            .finally(function () {
                vm.isBusy = false;
            });

        // will be called when someone click on the Add button, they will see the name they typed in.
        vm.addTrip = function () {

            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/trips", vm.newTrip)
                .then(function (response) {
                    // Success
                    vm.trips.push(response.data);            // data represents the new trip object.
                    vm.newTrip = {};                        // clear the form.
                }, function () {
                    // Failure
                    vm.errorMessage = "Failed to save new trip";
                })
                .finally(function () {
                    vm.isBusy = false;
                });

            //Remove due to Creating Data with AngularJS.
            //vm.trips.push({ name: vm.newTrip.name, created: new Date() });
            //vm.newTrip = {};          // clear the form
        };

    }    

})();