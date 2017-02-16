https://sod.superoffice.com/apps/GoogleMapsTrunk/MapsExampleForContact.aspx?ContactId=1

https://dev-tfn-d30/SuperOffice.MapsExample/MapsExampleForContact.aspx?ContactId=1


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




        function CenterMapOnMarker() {
            var bounds = new google.maps.LatLngBounds();

            for (var i = 0; i < markers.length; i++) {
                // extending bounds to contain this visible marker position
                bounds.extend(markers[i].getPosition());
            }

            // setting new bounds to visible markers
            map.fitBounds(bounds);

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




