using System.Web.Mvc;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters( GlobalFilterCollection filters )
		{
			filters.Add( new HandleErrorAttribute() );
		}
	}
}
