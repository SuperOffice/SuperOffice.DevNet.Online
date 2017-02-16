using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.DevNet.Online.Login.Extensions;
using SuperOffice.DevNet.Online.Provisioning;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	/// <summary>
	/// Got the button from http://www.cssbuttongenerator.com/
	/// </summary>
	public partial class Uninstall : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();

			Page.Title = "SuperOffice Maps example Uninstall";

			_username.InnerText = SuperOfficeAuthHelper.Context.Name;
			_company.InnerText = SuperOfficeAuthHelper.Context.Company;

			var masterPage = Master as MapsExample;
			if (masterPage != null)
			{
				masterPage.DescriptionText = "Uninstall SuperOffice Maps from your SuperOffice CRM online installation?";
				masterPage.ExtraHeaderText = "Uninstall";
			}

			InsertWebPanelListItems();
		}

		private void InsertWebPanelListItems()
		{
			_webPanelList.Items.Clear();

			var wpHelper = new WebPanelHelper();

			var webpanels = wpHelper.GetInstalledWebPanelIdentifiers( Global.AppName ).ToArray();

			if (webpanels.Length == 0)
			{
				_webPanelList.Items.Add( new ListItem
				{
					Text = "(No web panels found)",
				} );
				
			}
			else
				foreach (var webpanel in webpanels)
				{
					_webPanelList.Items.Add(new ListItem
					{
						Text = webpanel.Key,
						Value = webpanel.Value.ToString()
					});
				}

		}

		protected void UninstallButton_Click( object sender, EventArgs e )
		{

            SuperOfficeAuthHelper.Authorize();

			var wpHelper = new WebPanelHelper();
			wpHelper.DeleteAllWebPanelsInForeignKeyTables( Global.AppName );

			this.ProvideFeedbackToTheUser();
		}
	}
}