using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class Welcome : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			Page.Title = "SuperOffice Maps example welcome";

		}

		protected void InstallButton_Click( object sender, EventArgs e )
		{
			Response.BufferOutput = true;
			Response.Redirect("Configuration.aspx");
		}
	}
}