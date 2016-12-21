var ViewModel = function () {
    var self = this;
    self.locations = ko.observableArray();
    self.error = ko.observable();

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

    getAllLocations();

};

ko.applyBindings(new ViewModel());