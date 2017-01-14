$.fn.select2.defaults.set('debug', true);
var locationsUri = 'http://localhost:61215/api/locations';
var authenticationUri = 'http://localhost:61215/api/authentication';
var authenticationString;
var map;
var myLatitude;
var myLongitude;

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
    //console.log(result);
    for (i = 0; i < result.length; i++) {
        //console.log(result[i].Id);
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
            console.log(result.PlaylistId);
            $("#custom-spotify-player").attr("src", "https://embed.spotify.com/?uri=spotify:user:danielberkeley:playlist:" + result.PlaylistId);
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
    ajaxHelper(locationsUri, 'GET').done(function (data) {
        self.locations(data);
    });
}

function formatTrack(track) {
    if (track.loading) return track.text;

    //var markup = "<i>" + track.id + "</i>";
    //console.log(track);
    var markup = "<div class='select2-result-track clearfix'>" +
        "<div class='select2-result-track_image'><img src='" + track.album.images[2].url + "' /></div>" +
        "<div class='select2-result-track_meta'>" +
        "<div class='select2-result-track_name'>" + track.text + "</div>" +
        "<div class='select2-result-track_meta'>" +
        "<div class='select2-result-track_artist'>" + track.album.artists[0].name + "</div>";
    return markup;
}

function formatTrackSelection(track, container) {
    console.log(track.id);
    return track.name || track.text;
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
    
    
    var $select = $('#select-track').selectize({
        valueField: 'id',
        labelField: 'name',
        searchField: 'name',
        create: false,
        options: [],
        render: {
            option: function (item, escape) {
                
                return '<div>' +
					'<img src="' + escape(item.album.images[2].url) + '" alt="">' +
                /*
                    '<span class="title">' +
						'<span class="name">' + escape(item.name) + '</span>' +
					'</span>' +
					//'<span class="description">' + escape(item.synopsis || 'No synopsis available at this time.') + '</span>' +
					'<span class="artists">' + escape(item.artists[0].name) + '</span>' +
				'</div>';
                */

                //'<div>' +
                    '<span class="title">' +
                        '<span class="name">' + escape(item.name) + '</span>' +
                        //'<span class="by">' + escape(item.username) + '</span>' +
                    '</span>' +
                    //'<span class="description">' + escape(item.description) + '</span>' +
                    '<ul class="meta">' +
                        //(item.language ? '<li class="language">' + escape(item.language) + '</li>' : '') +
                        '<li class="watchers"><span>' + escape(item.artists[0].name) + 
                        //'<li class="forks"><span>' + escape(item.forks) + '</span> forks</li>' +
                    '</ul>' +
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
                    $('#display-data').prepend('<img id="theImg" src="' + jsonData.album.images[1].url + '"/>');
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
                LocationId: "ChIJC4xmE1LdMIgRyQsQaH9Aawk",
                TrackId: selectize.items,
                AccessToken: authenticationString
            }
        });
        console.log(response);
        $("#spotify-player-goes-after").hide().fadeIn('fast');

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

   
    GMaps.geolocate({
        success: function (position) {
            map = new GMaps({
                div: '#map',
                zoom: 12,
                lat: position.coords.latitude,
                lng: position.coords.longitude
            });
            //map.setCenter(, );
        },
        error: function (error) {
            map = new GMaps({
                div: '#map',
                zoom: 50,
                lat: 37.870584,
                lng: -122.260577
            });
            console.log('Geolocation failed: '+error.message);
        },
        not_supported: function () {
            map = new GMaps({
                div: '#map',
                zoom: 50,
                lat: 37.870584,
                lng: -122.260577
            });
            console.log("Your browser does not support geolocation");
        },
        always: function() {
            //alert("Done!");
        }
    });



    ajaxHelper(locationsUri, addMultipleMarkers);


});

