using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.CRM.Services;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.Factory;
using SuperOffice.Util;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class MapsExampleForSelection : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();

			// First choose the correct installation:
			SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = SuperOfficeAuthHelper.Context.NetServerUrl;

			var selPar = Request.QueryString[ "SelectionId" ];
			int selId = Convert.ToInt32( selPar );

			using( var selAgent = new SelectionAgent() )
			{
				SelectionEntity sel = selAgent.GetSelectionEntity( selId );

				IAddressFetcher fetcher = null;
				switch( sel.TargetTableNumber )
				{
					case 5: // Contact
					fetcher = new AddressFetcherFromContactSelection();
					break;

					default:
					fetcher = new AddressFetcherFromOtherSelections(sel.TargetTableNumber);
					break;
				}

				Page.ClientScript.RegisterStartupScript( this.GetType(), "MapScript", fetcher.GetScript( selId ), true );
			}
		}
	}
}