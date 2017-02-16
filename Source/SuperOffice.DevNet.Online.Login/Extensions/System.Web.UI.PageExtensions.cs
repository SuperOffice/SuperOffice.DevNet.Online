using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace SuperOffice.DevNet.Online.Login.Extensions
{
	static public class PageExtensions
	{
		public static void ProvideFeedbackToTheUser( this Page page)
		{
			// Go to user's installation:
			var url = SuperOfficeAuthHelper.Context.GetUsersInstallationUrl();
			if( !url.IsNullOrEmpty() )
			{
				// flush the CRM.WEB caches:
				url += "/default.aspx?flush";
				page.Response.Redirect( url );
			}
		}
	}
}
