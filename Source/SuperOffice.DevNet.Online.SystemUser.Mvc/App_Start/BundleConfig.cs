using System.Web.Optimization;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles( BundleCollection bundles )
		{
			bundles.Add( new StyleBundle( "~/Content/css" ).Include(
					  "~/Content/site.css" ) );

			BundleTable.EnableOptimizations = true;
		}
	}
}
