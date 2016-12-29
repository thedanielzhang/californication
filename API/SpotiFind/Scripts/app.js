var ViewModel = function () {
    var self = this;
    self.locations = ko.observableArray();
    self.error = ko.observable();
    self.detail = ko.observableArray();

    self.newLocation = {
        Place: ko.observable(),
        PlaylistName: ko.observable()
    }

    var locationsUri = 'api/locations/';

    function ajaxHelper(uri, method, data) {
        self.error('');
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getAllLocations() {
        ajaxHelper(locationsUri, 'GET').done(function (data) {
            self.locations(data);
        });
    }

    self.getLocationDetail = function (item) {
        ajaxHelper(locationsUri + item.Id, 'GET').done(function (data) {
            self.detail(data);
        });
    }

    //self.addLocation = function (formElement) {
    //    var location = {
    //        Place: self.newLocation.Place(),
    //        Playlist: self.newLocation.PlaylistName()
    //    };

    //    ajaxHelper(locationsUri, 'POST', location).done(function (item) {
    //        self.locations.push(item);
    //    });
    //}



    getAllLocations();

};

ko.applyBindings(new ViewModel());