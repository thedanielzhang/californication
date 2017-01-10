$.fn.select2.defaults.set('debug', true);
var locationsUri = 'http://localhost:61215/api/locations';
var authenticationUri = 'http://localhost:61215/api/authentication';
var authenticationString;

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
/*
function ajaxHelper(uri, method, data) {
    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null
    });
}
*/
function addMultipleMarkers(result) {
    console.log(result);
    for (i = 0; i < result.length; i++) {
        console.log(result[i].Id);
        ajaxHelper(locationsUri + '?id=' + result[i].Id + '&accessToken=' + authenticationString, addMarker);
    }
}

function addMarker(result) {
    $("#map").addMarker({
        coords: [result.PlaceLatitude, result.PlaceLongitude], // GPS coords
        //url: 'http://www.tiloweb.com', // Link to redirect onclick (optional)
        id: 'marker' + result.Id, // Unique ID for your marker
        title: result.PlaylistName,
        text: result.PlaceName
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
            console.log(json);
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
    ajaxHelper(locationsUri, 'GET').done(function (data) {
        self.locations(data);
    });
}

function formatTrack(track) {
    if (track.loading) return track.text;

    //var markup = "<i>" + track.id + "</i>";

    var markup = "<div class='select2-result-track clearfix'>" +
        "<div class='select2-result-track_image'><img src='" + track.data.album.images[2].url + "' /></div>" +
        "<div class='select2-result-track_meta'>" +
        "<div class='select2-result-track_name'>" + track.text + "</div>" +
        "<div class='select2-result-track_meta'>" +
        "<div class='select2-result-track_artist'>" + track.data.album.artists[0].name + "</div>";
    return markup;
}

function formatTrackSelection(track) {
    return track.text;
}

/* Scroll the background layers */
function parallaxScroll() {
    var scrolled = $(window).scrollTop();
    $('#parallax-bg1').css('top', (0 - (scrolled * .25)) + 'px');
    $('#parallax-bg2').css('top', (0 - (scrolled * .5)) + 'px');
    $('#parallax-bg3').css('top', (0 - (scrolled * .75)) + 'px');
}

/* Set navigation dots to an active state as the user scrolls */
function redrawDotNav() {
    var section1Top = 0;
    // The top of each section is offset by half the distance to the previous section.
    var section2Top = $('#frameless-parachute').offset().top - (($('#english-channel').offset().top - $('#frameless-parachute').offset().top) / 2);
    var section3Top = $('#english-channel').offset().top - (($('#about').offset().top - $('#english-channel').offset().top) / 2);
    var section4Top = $('#about').offset().top - (($(document).height() - $('#about').offset().top) / 2);;
    $('nav#primary a').removeClass('active');
    if ($(document).scrollTop() >= section1Top && $(document).scrollTop() < section2Top) {
        $('nav#primary a.manned-flight').addClass('active');
    } else if ($(document).scrollTop() >= section2Top && $(document).scrollTop() < section3Top) {
        $('nav#primary a.frameless-parachute').addClass('active');
    } else if ($(document).scrollTop() >= section3Top && $(document).scrollTop() < section4Top) {
        $('nav#primary a.english-channel').addClass('active');
    } else if ($(document).scrollTop() >= section4Top) {
        $('nav#primary a.about').addClass('active');
    }

}


$(document).ready(function () {


    $.ajax({
        type: "GET",
        url: authenticationUri,
        async: false,
        success: function (text) {
            authenticationString = text;
        }
    });
    
        
    $('.spotify-api-data').select2({
        ajax: {
            url: "https://api.spotify.com/v1/search",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term, // search term
                    page: params.page,
                    type: 'track'
                };
            },
            processResults: function (data, params) {
                // parse the results into the format expected by Select2
                // since we are using custom formatting functions we do not need to
                // alter the remote JSON data, except to indicate that infinite
                // scrolling can be used
                params.page = params.page || 1;
                //parsedJason = JSON.parse(data);
                dataItems = data.tracks.items;

                return {
                    results: $.map(dataItems, function (item) {
                        return {
                            id: item.id,
                            text: item.name,
                            data: item
                        };
                    }),
                    pagination: {
                        more: (params.page * 30) < data.total
                    }
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 1,
        templateResult: formatTrack, // omitted for brevity, see the source of this page
        templateSelection: formatTrackSelection // omitted for brevity, see the source of this page
    });


    redrawDotNav();

    /* Scroll event handler */
    $(window).bind('scroll', function (e) {
        parallaxScroll();
        redrawDotNav();
    });

    /* Next/prev and primary nav btn click handlers */
    $('a.manned-flight').click(function () {
        $('html, body').animate({
            scrollTop: 0
        }, 1000, function () {
            parallaxScroll(); // Callback is required for iOS
        });
        return false;
    });
    $('a.frameless-parachute').click(function () {
        $('html, body').animate({
            scrollTop: $('#frameless-parachute').offset().top
        }, 1000, function () {
            parallaxScroll(); // Callback is required for iOS
        });
        return false;
    });
    $('a.english-channel').click(function () {
        $('html, body').animate({
            scrollTop: $('#english-channel').offset().top
        }, 1000, function () {
            parallaxScroll(); // Callback is required for iOS
        });
        return false;
    });
    $('a.about').click(function () {
        $('html, body').animate({
            scrollTop: $('#about').offset().top
        }, 1000, function () {
            parallaxScroll(); // Callback is required for iOS
        });
        return false;
    });

    /* Show/hide dot lav labels on hover */
    $('nav#primary a').hover(
    	function () {
    	    $(this).prev('h1').show();
    	},
		function () {
		    $(this).prev('h1').hide();
		}
    );

    /*
    var basicLocations = [];
    console.log(dataStore(locationsUri).getJson());
    basicLocations = dataStore(locationsUri).getJson();
    console.log(basicLocations.length);
    */


    /*
    for (var b in $(dataStore(locationsUri).getJson())) {
        basicLocations.push({
            Id: b.Id,
            PlaceId: b.Place,
            PlaylistId: b.PlaylistId
        });
    }
    */

    //alert(basicLocations);

    $("#map").googleMap({
        zoom: 10, // Initial zoom level (optional)
        //coords: [48.895651, 2.290569], // Map center (optional)
        type: "ROADMAP" // Map type (optional)
    });

    ajaxHelper(locationsUri, addMultipleMarkers);






});

