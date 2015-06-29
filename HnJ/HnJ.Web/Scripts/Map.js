//
//  Map.js
//  HnJ Cinemas
//
//  Created by Hasan Nizamani on 02/11/2014.
//  Copyright (c) 2014 Hasan Nizamani. All rights reserved.
//

var map = null;
var marker = null;
var geocoder;
var zoom = 11;

var coordinateRoundTo = 5;

var myLatlng = new google.maps.LatLng(-35.28193, 149.12878);

function initialize() {

    // START - Put marker on the map if in edit mode
    var loadedLat = $("#Latitude").val();
    var loadedLng = $("#Longitude").val();

    if (loadedLat != null && loadedLng != null) {
        if (loadedLat != 0 && loadedLng != 0) {
            myLatlng = new google.maps.LatLng(loadedLat, loadedLng);
            zoom = 17;
        }
    }
    // END - Put marker on the map if in edit mode

    ////////////////////////////////////////////////////////////////////////////////////

    // START - Initialize Map
    var mapOptions = {
        center: { lat: myLatlng.lat(), lng: myLatlng.lng() },
        zoom: zoom,
        draggableCursor: 'crosshair'
    };

    map = new google.maps.Map(document.getElementById('map-canvas'),
        mapOptions);
    // END - Initialize Map

    google.maps.event.trigger(map, 'resize');

    ////////////////////////////////////////////////////////////////////////////////////

    // START - Put marker on the map if in edit mode
    if (loadedLat != null && loadedLng != null) {
        if (loadedLat != 0 && loadedLng != 0) {
            AddMarker(myLatlng);

            $("#mapLatitude").html("Latitude: <b>" + $("#Latitude").val() + "</b>");
            $("#mapLongitude").html("Longitude: <b>" + $("#Longitude").val() + "</b>");
            $("#mapAddress").html("<b>" + $("#Address").val() + "</b>");
        }
    }
    // END - Put marker on the map if in edit mode

    ////////////////////////////////////////////////////////////////////////////////////

    // START - Display coordinates on Mouse Move if there is no marker on the map

    google.maps.event.addListener(map, 'mousemove', function (event) {
        displayCoordinates(event.latLng);
    });

    function displayCoordinates(pnt) {

        if (marker != null)
            return;

        var lat = pnt.lat();
        lat = lat.toFixed(coordinateRoundTo);
        var lng = pnt.lng();
        lng = lng.toFixed(coordinateRoundTo);

        $("#Latitude").val(lat);
        $("#Longitude").val(lng);

        $("#mapLatitude").html("Latitude: <b>" + lat + "</b>");
        $("#mapLongitude").html("Longitude: <b>" + lng + "</b>");

        $("#mapAddress").html("<b style='color:yellow;'>[Drop marker to fetch address]</b>");
    }
    // END - Display coordinates on Mouse Move if there is no marker on the map

    ////////////////////////////////////////////////////////////////////////////////////

    // START - Add marker when clicked on the map
    google.maps.event.addListener(map, 'click', function (event) {
        AddMarker(event.latLng);

        var lat = event.latLng.lat().toFixed(coordinateRoundTo);
        var lng = event.latLng.lng().toFixed(coordinateRoundTo);

        $("#Latitude").val(lat);
        $("#Longitude").val(lng);

        $("#mapLatitude").html("Latitude: <b>" + lat + "</b>");
        $("#mapLongitude").html("Longitude: <b>" + lng + "</b>");

        GetAddress();
    });
    // END - Add marker when clicked on the map

    ////////////////////////////////////////////////////////////////////////////////////

    $("#Latitude").blur(function () {
        UpdateMarker();
    });

    $("#Longitude").blur(function () {
        UpdateMarker();
    });

    function UpdateMarker()
    {
        var lat = $("#Latitude").val();
        var lng = $("#Longitude").val();

        $("#mapLatitude").html("Latitude: <b>" + lat + "</b>");
        $("#mapLongitude").html("Longitude: <b>" + lng + "</b>");

        var latlng = new google.maps.LatLng(lat, lng);

        AddMarker(latlng);
        map.panTo(marker.getPosition());
        GetAddress();
    }
}

// START - Add marker
function AddMarker(pnt) {
    RemoveMarker();

    var lat = pnt.lat();
    lat = lat.toFixed(coordinateRoundTo);
    var lng = pnt.lng();
    lng = lng.toFixed(coordinateRoundTo);

    myLatlng = new google.maps.LatLng(lat, lng);

    var image = {
        url: '../../content/images/cinema.png',
    };

    marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        icon: image,
        //title: "Hello World!",
        draggable: true
    });

    google.maps.event.addListener(marker, 'dragend', function (event) {

        var lat = event.latLng.lat().toFixed(coordinateRoundTo);
        var lng = event.latLng.lng().toFixed(coordinateRoundTo);

        $("#Latitude").val(lat);
        $("#Longitude").val(lng);

        $("#mapLatitude").html("Latitude: <b>" + lat + "</b>");
        $("#mapLongitude").html("Longitude: <b>" + lng + "</b>");

        GetAddress();
    });

    google.maps.event.addListener(marker, 'dragstart', function (event) {
        // Use when required
    });

    google.maps.event.addListener(marker, 'drag', function (event) {
        var lat = event.latLng.lat().toFixed(coordinateRoundTo);
        var lng = event.latLng.lng().toFixed(coordinateRoundTo);

        $("#Latitude").val(lat);
        $("#Longitude").val(lng);

        $("#mapLatitude").html("Latitude: <b>" + lat + "</b>");
        $("#mapLongitude").html("Longitude: <b>" + lng + "</b>");

        $("#Address").val("");
        $("#mapAddress").html("<b style='color:yellow;'>[Drop marker to fetch address]</b>");
    });

    google.maps.event.addListener(marker, 'click', function (event) {
        RemoveMarker();
    });
}
// END - Add marker

////////////////////////////////////////////////////////////////////////////////////

// START - Remove marker
function RemoveMarker() {
    if (marker == null)
        return;

    marker.setMap(null);
    marker = null;

    $("#Address").val("");
    $("#mapAddress").html("");
}
// END - Remove marker

////////////////////////////////////////////////////////////////////////////////////

// START - Get Address from coordinates using GeoLocation API of Google Maps
function GetAddress() {

    var lat = $("#Latitude").val();
    var lng = $("#Longitude").val();

    var latlng = new google.maps.LatLng(lat, lng);

    geocoder = new google.maps.Geocoder();

    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[0]) {

                var address = results[0].formatted_address;
                $("#Address").val(address);
                $("#mapAddress").html("<b>" + address + "</b>");

            } else {
                //alert('No results found');
                $("#mapAddress").html("<b style='color:yellow;'>[Cannot get address]</b>");
            }
        } else {
            //alert('Geocoder failed due to: ' + status);
            $("#mapAddress").html("<b style='color:yellow;'>[Cannot get address]</b>");
        }
    });
}
// END - Get Address from coordinates using GeoLocation API of Google Maps

////////////////////////////////////////////////////////////////////////////////////

// START - Get Coordinates from address using GeoLocation API of Google Maps
function codeAddress() {

    var address = $("#txtSearchAddress").val();

    if (address == null)
        return;

    geocoder = new google.maps.Geocoder();

    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            map.setCenter(results[0].geometry.location);
            map.setZoom(14);

            var lat = results[0].geometry.location.lat().toFixed(coordinateRoundTo);
            var lng = results[0].geometry.location.lng().toFixed(coordinateRoundTo);
            var latlng = new google.maps.LatLng(lat, lng);

            AddMarker(latlng);

            var address = results[0].formatted_address;
            $("#Address").val(address);
            $("#mapAddress").html("<b>" + address + "</b>");

            $("#Latitude").val(lat);
            $("#Longitude").val(lng);

            $("#mapLatitude").html("Latitude: <b>" + lat + "</b>");
            $("#mapLongitude").html("Longitude: <b>" + lng + "</b>");

        }
        else {
            $("#mapAddress").html("<b style='color:yellow;'>[Cannot get address]</b>");
        }
    });
}
// END - Get Coordinates from address using GeoLocation API of Google Maps

google.maps.event.addDomListener(window, 'load', initialize);

