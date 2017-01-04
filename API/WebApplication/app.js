$.fn.select2.defaults.set('debug', true);

$(document).ready(function () {

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
	        results: dataItems,
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

    
});

/* Scroll the background layers */



function formatTrack (track) {
  if (track.loading) return track.text;

  var markup = "<div class='select2-result-track clearfix'>" +
      "<div class='select2-result-track_image'><img src='" + track.album.images[2].url + "' /></div>" + 
      "<div class='select2-result-track_meta'>" +
      "<div class='select2-result-track_name'>" + track.name + "</div>" +
      "<div class='select2-result-track_meta'>" +
      "<div class='select2-result-track_artist'>" + track.album.artists[0].name + "</div>";
  return markup;
}

function formatTrackSelection (track) {
    return track.name || track.text;
}

