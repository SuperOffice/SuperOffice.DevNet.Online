using System;
using System.Linq;
using SuperOffice.CRM.Services;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.DevNet.Online.Login.Extensions;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class MapsExampleForContact : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();

			// First choose the correct installation:
			SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = SuperOfficeAuthHelper.Context.NetServerUrl;

			var contPar = Request.QueryString[ "ContactId" ];
			int contId = Convert.ToInt32( contPar );

			using (var contAgent = new ContactAgent())
			{
				var cont = contAgent.GetContactEntity(contId);
				var addr = cont.Address;
				string address = string.Empty;
				if( addr.LocalizedAddress != null && addr.LocalizedAddress.Length > 1)
				{
					if (addr.LocalizedAddress[0] != null)
						address = addr.LocalizedAddress[0].Aggregate(address, (current, addrLine) => current.Add(addrLine.Value, ", "));
					if( addr.LocalizedAddress[ 1 ] != null )
						address = addr.LocalizedAddress[ 1 ].Aggregate( address, ( current, addrLine ) => current.Add( addrLine.Value, ", " ) );
					address = address.Add( cont.Country.EnglishName, ", " );

					string lat, lng;
					GoogleMaps.GeocodeAddress(address,
						out lat,
						out lng
						);

					var contactName = System.Web.HttpUtility.JavaScriptStringEncode(cont.Name);

					var script = "<script>\nfunction initializeMarkers() {\n";
					script += string.Format("AddMarker(map, '{0}', '{1}', '{2}' );\n", contactName, lat, lng);
					script += "}\n</script>";


					loadMarkersScript.Text = script;
				}
			}
		}
	}
}