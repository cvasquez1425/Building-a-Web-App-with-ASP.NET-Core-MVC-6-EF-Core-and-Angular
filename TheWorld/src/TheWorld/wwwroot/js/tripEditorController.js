// tripEditorController.js
(function () {
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams, $http) {
        var vm = this;

        vm.tripName = $routeParams.tripName;
        vm.stops = [];                          //list of the stops we want to edit. Empty Array.
        vm.errorMessage = "";
        vm.isBusy = true;
        vm.newStop = {};

        var url = "/api/trips/" + vm.tripName + "/stops";

        $http.get(url)
            .then(function (response) {
                // success
                angular.copy(response.data, vm.stops);
                _showMap(vm.stops);
            }, function (err) {
                // failure
                vm.errorMessage = "Failed to load stops";
            })
            .finally(function () {
                vm.isBusy = false;
            });

        vm.addStop = function () {

            vm.isBusy = true;

            $http.post(url, vm.newStop)
                .then(function (response) {
                    // Success
                    vm.stops.push(response.data);
                    _showMap(vm.stops);
                    vm.newStop = {};                        // empty object again, clear out the forms
                }, function (err) {
                    // failure
                    vm.errorMessage = "Failed to add new stop";
                })
                .finally(function () {
                    vm.isBusy = false;
                });

        };
    }

    // Underscore as a beginning character says it is a private function, it is going to be used only on this file.
    function _showMap(stops) {                  

        if (stops && stops.length > 0) {
             
            // using the Underscore the lib/underscore/underscore.min.js library to map one structure to another.
            var mapStops = _.map(stops, function (item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                };
            });          

            // Show Map
            travelMap.createMap({
                stops: mapStops,
                selector: "#map",
                currentStop: 1,
                initialZoom: 3
            });

        }
    }


})();