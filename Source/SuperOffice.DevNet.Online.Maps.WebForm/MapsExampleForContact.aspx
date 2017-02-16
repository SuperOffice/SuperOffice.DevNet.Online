<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MapsExampleForContact.aspx.cs" Inherits="SuperOffice.DevNet.Online.Maps.WebForm.MapsExampleForContact" %>

<!DOCTYPE html>


<!-- NB! Is different because it shows just one address -->


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Maps example for SuperOffice CRM Online Contact</title>
    <style>
        html, body, #map-canvas {
            height: 100%;
            margin: 0px;
            padding: 0px;
        }
    </style>
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false"></script>
    <script>
        var markers = [];
        var map;


        function initializeMap() {
            var mapOptions = {
                backgroundColor: 'teal'
                , streetViewControl: true
                , mapTypeId: google.maps.MapTypeId.HYBRID
                , zoom: 17
            }
            map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

            initializeMarkers();    // Will be generated into the literal 'loadMarkersScript'

            map.setCenter(markers[0].getPosition());
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
    </script>

    <script>
        google.maps.event.addDomListener(window, 'load', initializeMap);
    </script>
</head>
    

<body>
    <div id="map-canvas">
        
    </div>
    <form id="form1" runat="server">
        <asp:Literal runat="server" ID="loadMarkersScript" />
    </form>
</body>
</html>
