var markers = [];
var map;

function initializeMap() {
    var mapOptions = {
        backgroundColor: 'teal'
        , streetViewControl: true
    }
    //var mapOptions = {
    //    center: new google.maps.LatLng(45.4555729, 9.169236),
    //    zoom: 20,
    //    mapTypeId: google.maps.MapTypeId.ROADMAP,

    //    panControl: true,
    //    mapTypeControl: false,
    //    panControlOptions: {
    //        position: google.maps.ControlPosition.RIGHT_CENTER
    //    },
    //    zoomControl: true,
    //    zoomControlOptions: {
    //        style: google.maps.ZoomControlStyle.LARGE,
    //        position: google.maps.ControlPosition.RIGHT_CENTER
    //    },
    //    scaleControl: false,
    //    streetViewControl: false,
    //    streetViewControlOptions: {
    //        position: google.maps.ControlPosition.RIGHT_CENTER
    //    }
    //};
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

    initializeMarkers();    // Will be generated into the literal 'loadMarkersScript'
    CenterMapOnMarkers();
}
function AddMarker(map, name, lat, long) {
    var myLatlng = new google.maps.LatLng(lat, long);
    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: name,
    });
    markers.push(marker);
}

function CenterMapOnMarkers() {
    var bounds = new google.maps.LatLngBounds();

    for (var i = 0; i < markers.length; i++) {
        // extending bounds to contain this visible marker position
        bounds.extend(markers[i].getPosition());
    }

    // setting new bounds to visible markers
    map.fitBounds(bounds);

}

google.maps.event.addDomListener(window, 'load', initializeMap);