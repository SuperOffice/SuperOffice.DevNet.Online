using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.Data;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.DevNet.Online.Login.Extensions;
using SuperOffice.DevNet.Online.Provisioning;

namespace SuperOffice.DevNet.Online.News.WebForm
{
    public partial class Install : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SuperOfficeAuthHelper.Authorize();

            CreateWebPanelInUsersInstallation();
            this.ProvideFeedbackToTheUser();
        }

        private void CreateWebPanelInUsersInstallation()
        {
            var helper = new WebPanelHelper();
            helper.CreateAndSaveWebPanel(Global.AppName, ConfigurationManager.AppSettings["NewsProviderName"], ConfigurationManager.AppSettings["NewsProviderURL"], Navigation.ContactArchive);
        }
    }
}