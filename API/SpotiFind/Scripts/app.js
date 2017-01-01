ko.bindingHandlers.select2 = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor());
        ko.unwrap(allBindings.get('selectedOptions'));

        $(element).select2(options);
        console.log('init');
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor());
        ko.unwrap(allBindings.get('selectedOptions'));

        $(element).select2(options);
        console.log('update');
    }
};

var ViewModel = function () {
    var self = this;
    self.locations = ko.observableArray();
    self.error = ko.observable();
    self.detail = ko.observableArray();

    self.newLocation = {
        Place: ko.observable(),
        PlaylistName: ko.observable()
    }

    self.allOptions = ko.observableArray([
        {
            id: '1706',
            text: 'Nathan Friend'
        },
        {
            id: '1707',
            text: 'Carlos Pabon'
        },
        {
            id: '1708',
            text: 'Ryan Hoffman'
        },
        {
            id: '1709',
            text: 'Judah DePaula'
        },
        {
            id: '1710',
            text: 'Alex Dirks'
        },
        {
            id: '1711',
            text: 'Andy Mitchell'
        },
        {
            id: '1712',
            text: 'Gopal Krishnan'
        },
        {
            id: '1713',
            text: 'Ryan Johnsen'
        },
    ]);

    self.selectedPeople = ko.observableArray();

    self.logs = ko.observableArray();

    self.selectedPeople.subscribe(function () {
        _this.logs.push(_this.selectedPeople().join(', '));
    });

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