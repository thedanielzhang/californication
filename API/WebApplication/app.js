
var locationsUri = 'http://localhost:61215/api/locations';
var authenticationUri = 'http://localhost:61215/api/authentication';
var authenticationString;
var map;
var myLatitude;
var myLongitude;


var currentLocation;

function basicCallback(result) {
    return result;
}
function authenticationAjax(callback) {
    $.ajax({
        type: "GET",
        url: authenticationUri,
        dataType: "text/plain",
        success: callback
    });
}

function addMultipleMarkers(result) {
    console.log(result);
    
    for (i = 0; i < result.length; i++) {   
        ajaxHelper(locationsUri + '?id=' + result[i].Id + '&accessToken=' + authenticationString, addMarker);
    }
    
}

function addMarker(result) {
    //console.log(result);
    //console.log(result.PlaceLatitude);
    
    map.addMarker({
        lat: result.PlaceLatitude, 
        lng: result.PlaceLongitude, // GPS coords
        //url: 'http://www.tiloweb.com', // Link to redirect onclick (optional)
        title: 'marker' + result.PlaylistId,
        click: function (e) {
            
            $("#custom-spotify-player").attr("src", "https://embed.spotify.com/?uri=spotify:user:danielberkeley:playlist:" + result.PlaylistId + "&theme=white");
        },
        infoWindow: {
            content: '<div class=' + 'customMarker id=' + result.PlaylistId + '>' + result.PlaceName + '</div>'
        }
    });
}

function ajaxHelper(uri, callback) {
    //var json = [];
    

    $.ajax({
        type: "GET",
        url: uri,
        dataType: "json",
        success: callback
    });

}

function dataStore(uri) {
    var json = [];

    $.ajax({
        type: "GET",
        url: uri,
        dataType: "json",
        success: function (data) {
            //console.log(data);
            for (i = 0; i < data.length; i++) {
                //console.log(d);
                json.push({
                    Id: data[i].Id,
                    PlaceId: data[i].Place,
                    PlaylistId: data[i].PlaylistId
                });
            }
            //console.log(json);
        }
    });



    return {

        getJson: function () {
            if (json) {
                //alert(json);
                //console.log(json);
                return json;
            }
            else alert("not loaded yet");
            // else show some error that it isn't loaded yet;
        }
    };
}

function getAllLocations() {
    ajaxHelper(locationsUri + '?lat=' + myLatitude + '&lon=' + myLongitude, 'GET').done(function (data) {
        self.locations(data);
    });
}

$(document).ready(function () {
    $('a[href*="#"]:not([href="#"])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                console.log(target);
                $('html, body').animate({
                    scrollTop: target.offset().top - 85
                }, 1000);
                return false;
            }
        }
    });

    $.ajax({
        type: "GET",
        url: authenticationUri,
        async: false,
        success: function (text) {
            authenticationString = text;
        }
    });



   

    
    
    
    var $select = $('#select-track').selectize({
        valueField: 'id',
        labelField: 'name',
        searchField: 'name',
        create: false,
        options: [],
        render: {
            option: function (item, escape) {
                
                return '<div class="selected-stuff">' +
                    '<div class="selected-options">' +
                        '<div class="selected-image">' +
					        '<img class="selected-album-image" src="' + escape(item.album.images[2].url) + '" alt="">' +
                        '</div>' +
                        '<div class="selected-item">' +
                            '<p class="selected-title">' + escape(item.name) + '</p>' + '<p class="selected-artist">' + escape(item.artists[0].name) + '</p>' + 
                        '</div>' +
                    '</div>' +
                '</div>';
                
            }
        },
        load: function(query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: "https://api.spotify.com/v1/search",
                dataType: 'json',
                delay: 250,
                data: { 
                        q: query, // search term
                        page_limit: 30,
                        type: 'track'
                    
                },
                error: function() {
                    callback();
                },
                success: function(res) {
                    callback(res.tracks.items);
                }
            });
        }
    });
    var selectize = $select[0].selectize;


    

    $('select.selectized,input.selectized').each(function () {

        var update = function (e) {
            var trackId = selectize.items
            var trackUri = "https://api.spotify.com/v1/tracks/" + trackId;
            $.ajax({
                type: 'GET',
                url: trackUri,
                dataType: 'json',
                success: function (jsonData) {
                    console.log(jsonData);
                    //console.log()
                    $('#song-img').attr("src", jsonData.album.images[1].url);
                    $('#display-data').prepend('<img id="theImg" src="' +  + '"/>');
                    $('#track-name').text("Track name: " + jsonData.name);
                    $('#track-album').text("Track album: " + jsonData.album.name);
                    $('#track-artist').text("Track artist: " + jsonData.artists[0].name);
                },
                error: function () {
                    console.log("could not load data");
                }
            });


            
        }

        

        $(this).on('change', update);
        update();


    });

    $('#submit-track').click(function () {
        var response = $.ajax({
            type: 'POST',
            url: locationsUri,
            dataType: "application/json",
            data:  {
                LocationId: currentLocation.PlaceId,
                TrackId: selectize.items,
                AccessToken: authenticationString
            }
        });
        console.log(response);
        $("#custom-spotify-player").attr("src", "https://embed.spotify.com/?uri=spotify:user:danielberkeley:playlist:" + currentLocation.PlaylistId + "&theme=white");

    });
   
    GMaps.geolocate({
        success: function (position) {
            myLatitude = position.coords.latitude;
            myLongitude = position.coords.longitude;
            map = new GMaps({
                div: '#map',
                zoom: 17,
                lat: myLatitude,
                lng: myLongitude
            });
            //map.setCenter(, );
        },
        error: function (error) {
            map = new GMaps({
                div: '#map',
                zoom: 25,
                lat: 37.870584,
                lng: -122.260577
            });
            console.log('Geolocation failed: '+error.message);
        },
        not_supported: function () {
            map = new GMaps({
                div: '#map',
                zoom: 25,
                lat: 37.870584,
                lng: -122.260577
            });
            console.log("Your browser does not support geolocation");
        },
        always: function() {
            //alert("Done!");
        }
    });

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(successFunction, errorFunction);
    } else {
        alert('It seems like Geolocation, which is required for this page, is not enabled in your browser. Please use a browser which supports it.');
    }
    //console.log(myLatitude);

    
    


});

function successFunction(position) {
    $.ajax({
        type: "GET",
        url: authenticationUri,
        async: false,
        success: function (text) {
            authenticationString = text;
            console.log(authenticationString);
        }
    });
    myLatitude = position.coords.latitude;
    myLongitude = position.coords.longitude;

    console.log('Your latitude is :' + myLatitude + ' and longitude is ' + myLongitude);
    
    ajaxHelper(locationsUri + '?lat=' + myLatitude + '&lon=' + myLongitude, addMultipleMarkers);
    
    
    
    
    $.ajax({
        type: "GET",
        url: locationsUri + '?lat=' + myLatitude + '&lon=' + myLongitude + '&accessToken=' + authenticationString,
        async: false,
        success: function (result) {
            currentLocation = result;
        }
    });
    
    $("#custom-spotify-player").attr("src", "https://embed.spotify.com/?uri=spotify:user:danielberkeley:playlist:" + currentLocation.PlaylistId + "&theme=white");
    console.log(currentLocation);
    $("#current-location-name").text(function () {
        return "YOU'RE CURRENTLY AT " + currentLocation.PlaceName;
    });
    
}

function errorFunction() {
    console.log("error");
}
