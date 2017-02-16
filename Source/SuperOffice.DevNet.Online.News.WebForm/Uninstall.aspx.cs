using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.DevNet.Online.Login.Extensions;
using SuperOffice.DevNet.Online.Provisioning;

namespace SuperOffice.DevNet.Online.News.WebForm
{
    public partial class Uninstall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SuperOfficeAuthHelper.Authorize();

            Page.Title = "Company News Uninstall";

            _username.InnerText = SuperOfficeAuthHelper.Context.Name;
            _company.InnerText = SuperOfficeAuthHelper.Context.Company;

            var masterPage = Master as SiteMaster;
            if (masterPage != null)
            {
                masterPage.DescriptionText = "Uninstall Company News from your SuperOffice CRM online installation?";
                masterPage.ExtraHeaderText = "Uninstall";
            }

            // Only an admin can remove this app:
            UninstallButton.Enabled = SoContext.CurrentPrincipal.HasFunctionRight("admin-all");
        }

        protected void UninstallButton_Click(object sender, EventArgs e)
        {
            SuperOfficeAuthHelper.Authorize();


            if (SoContext.CurrentPrincipal.HasFunctionRight("admin-all"))
            {
                var wpHelper = new WebPanelHelper();
                wpHelper.DeleteAllWebPanelsInForeignKeyTables(Global.AppName);

                this.ProvideFeedbackToTheUser();
            }
        }
    }
}