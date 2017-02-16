using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.CRM.Globalization;
using SuperOffice.DevNet.Online.Login;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class MapsExampleForProject : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();

			// First choose the correct installation:
			SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = SuperOfficeAuthHelper.Context.NetServerUrl;

			var par = Request.QueryString[ "ProjectId" ];
			int id = Convert.ToInt32( par );

			var fetcher = new AddressFetcherFromProjectMember();

			Page.ClientScript.RegisterStartupScript( this.GetType(), "MapScript", fetcher.GetScript( id ), true );
		}
	}
}