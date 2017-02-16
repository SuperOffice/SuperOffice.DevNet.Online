using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class MapsExample : System.Web.UI.MasterPage
	{

		public string ExtraHeaderText
		{
			get { return _extraHeaderText.InnerText; }
			set { _extraHeaderText.InnerText = ": " + value; }
		}
		public string DescriptionText
		{
			get { return _description.InnerText; }
			set { _description.InnerText = value; }
		}

		protected void Page_Load( object sender, EventArgs e )
		{

		}
	}
}