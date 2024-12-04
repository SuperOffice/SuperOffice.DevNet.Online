using System;
using SuperOffice.DevNet.Online.Login;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class MapsExampleForDiary : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();

			// First choose the correct installation:
			SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = SuperOfficeAuthHelper.Context.NetServerUrl;

			var par = Request.QueryString[ "UserId" ];
			int id = Convert.ToInt32( par );

			var fetcher = new AddressFetcherFromDiary();

			Page.ClientScript.RegisterStartupScript( this.GetType(), "MapScript", fetcher.GetScript( id ), true );
		}
	}
}