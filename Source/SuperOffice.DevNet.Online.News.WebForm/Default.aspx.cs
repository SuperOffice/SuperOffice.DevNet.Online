using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.CRM.Services;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.Data;

namespace SuperOffice.DevNet.Online.News.WebForm
{
	public partial class Welcome : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void InstallButton_Click(object sender, EventArgs e)
		{
           
            this.Response.Redirect("~/Install.aspx");

		}


	}
}