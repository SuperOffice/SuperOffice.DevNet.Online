using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public class GoogleMaps
	{
		public static void GeocodeAddress(string address, out string lat, out string lng)
		{
			XElement xLat, xLng;

			GeocodeAddress( address, out xLat, out xLng );

			lat = xLat.Value;
			lng = xLng.Value;
		}

		private static void GeocodeAddress( string address, out XElement lat, out XElement lng )
		{
			var requestUri = string.Format( "http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
				Uri.EscapeDataString( address ) );

			var request = WebRequest.Create( requestUri );
			var response = request.GetResponse();
			var xdoc = XDocument.Load( response.GetResponseStream() );
			try
			{
				var result = xdoc.Element( "GeocodeResponse" ).Element( "result" );
				if( result == null )
				{
					lat = null;
					lng = null;
					return;
				}
				var locationElement = result.Element( "geometry" ).Element( "location" );
				lat = locationElement.Element( "lat" );
				lng = locationElement.Element( "lng" );

			}
			catch (Exception e)
			{
				throw new Exception( "Cannot geocode address: " + e.Message );
			}
		}

	}
}