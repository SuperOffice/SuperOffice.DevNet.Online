using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SuperOffice.DevNet.Online.Login;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public class Global : HttpApplication
	{

		protected void Application_Start( object sender, EventArgs e )
		{
			ConfigManager.SoSessionCookieName = "MapsExample";

		}

		protected void Session_Start( object sender, EventArgs e )
		{

		}

		protected void Application_BeginRequest( object sender, EventArgs e )
		{

		}

		protected void Application_AuthenticateRequest( object sender, EventArgs e )
		{

		}

		protected void Application_Error( object sender, EventArgs e )
		{

		}

		protected void Session_End( object sender, EventArgs e )
		{

		}

		protected void Application_End( object sender, EventArgs e )
		{

		}

        public const string AppName = "SuperOffice.DevNet.Online.Maps.WebForm";
	}
}