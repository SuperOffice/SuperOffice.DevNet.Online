using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.CRM.Services;
using SuperOffice.Data;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.DevNet.Online.Provisioning;
using SuperOffice.Util;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public partial class Configuration : System.Web.UI.Page
	{
		private const string BuyAllName = "_buyAll";

		private Dictionary<EProduct, Product> _products = null;
		private bool _isAnUpdate = false;

		protected void Page_Load( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();

			var p = new Product();
			_products = p.GetProductsDic();

			// Is this the first time, or later?
			var helper = new WebPanelHelper();
			var installedWebPanels = helper.GetAllWebPanels();

			if (installedWebPanels != null && installedWebPanels.Length != 0)
			{
				_isAnUpdate = true;
				if( !IsPostBack )
				{
					// The system has been installed, now the user wants to change the configuration:
					foreach( var product in _products.Values )
					{
						var id = installedWebPanels.FirstOrDefault( w => w.Name == product.LinkName );
						if(id != null)
						{
							product.IsSelected = id.Name == product.LinkName && !id.Deleted;
						}
						else
						{
							product.IsSelected = false;
						}
					}
				}
			}

			Page.Title = "SuperOffice Maps example setup";
			var masterPage = Master as MapsExample;
			if( masterPage != null )
			{
				masterPage.ExtraHeaderText = "Configure";
			}

			if( !IsPostBack )
			{
				_repeater.DataSource = _products.Values;
				this.DataBind();
			}

			if( IsPostBack )
			{
				//set checked on the data
				var checkboxes = _repeater.FindDescendants<CheckBox>().ToArray();
				var buyAll = GetBuyAllCheckBox( checkboxes );
				foreach( var checkbox in checkboxes )
				{
					if( checkbox != buyAll )
						GetProduct( checkbox ).IsSelected = checkbox.Checked;
				}
			}

		}
		protected void Page_PreRender( object sender, EventArgs e )
		{
			if( _isAnUpdate )
			{
				UpdateDoItButtonText( Context );
			}
		}

		private void UpdateDoItButtonText( System.Web.HttpContext Context )
		{
			var buttons = _repeater.FindDescendants<Button>();

			var button = buttons.First( b => b.ID == "_doItButton" );

			button.Text = "Update";
		}


		protected void CheckChanged( object sender, EventArgs e )
		{
			var checkboxes = _repeater.FindDescendants<CheckBox>();
			var buyAll = GetBuyAllCheckBox( checkboxes );
			buyAll.Checked = false;
		}

		protected void AllCheckChanged( object sender, EventArgs e )
		{
			var allCheckbox = sender as CheckBox;
			if( allCheckbox != null )
			{
				var checkboxes = _repeater.FindDescendants<CheckBox>();

				foreach( var checkbox in checkboxes )
				{
					if( checkbox != allCheckbox )
						checkbox.Checked = allCheckbox.Checked;
				}
			}
		}

		private static CheckBox GetBuyAllCheckBox( IEnumerable<CheckBox> checkboxes )
		{
			var buyAll = checkboxes.First( checkbox => checkbox.ID == BuyAllName );
			return buyAll;
		}

		private Product GetProduct( CheckBox checkbox )
		{
			if( checkbox.ID == BuyAllName )
				return Product.GetBuyAllProduct();


			var idField = checkbox.Parent.FindControl( "hiddenID" ) as HiddenField;
			if( idField == null )
				throw new Exception( "Couldn't find the hidden Id field, cannot find price" );
			else
			{
				var id = (EProduct)Convert.ToInt32( idField.Value );
				var dic = _products;
				if( !dic.ContainsKey( id ) )
					throw new Exception( "Cannot find the id {0} in the webPanels dictionary".FormatWith( id ) );

				var panel = dic[ id ];
				return panel;
			}
		}





		protected void Install_Click( object sender, EventArgs e )
		{
            SuperOfficeAuthHelper.Authorize();
			if( _isAnUpdate )
			{
				var helper = new WebPanelHelper();
				var installedWebPanels = helper.GetAllWebPanels();

				// Four possibilities:
				// Installed	Selected	Result
				//		x		    x		Do nothing
				//		x					Delete
				//					x		Install
				//							Do nothing
				foreach( var product in _products.Values )
				{
					var isInstalled = installedWebPanels.Any( wp => wp.Name == product.LinkName );
					if( isInstalled ) // or exists..
					{
						var webPanelId = installedWebPanels.Where(wp => wp.Name == product.LinkName).FirstOrDefault();

						if ( !product.IsSelected && webPanelId != null) // -> Delete
						{
							if(webPanelId.WebPanelId > 0)
								helper.DeleteWebPanel(webPanelId.WebPanelId);
						}
						else  // -> Undelete
						{
							_ = helper.CreateWebPanel(webPanelId.Name, webPanelId.Url, webPanelId.VisibleIn);
						}
					}
					else
					{
						if( product.IsSelected ) // -> Install
							helper.CreateAndSaveWebPanel( Global.AppName, product.LinkName, product.Url, product.GetNavigation() );
					}
				}
			}
			else
				CreateWebPanelsInUsersInstallation( _products.Values.Where( pr => pr.IsSelected ) );

			ProvideFeedbackToTheUser();
		}

		private void ProvideFeedbackToTheUser()
		{
			// Go to user's installation:
			var url = SuperOfficeAuthHelper.Context.GetUsersInstallationUrl();

            using (var agent = new DiagnosticsAgent())
                agent.FlushCaches();


            using (ConfigurationAgent ca = new ConfigurationAgent())
            {
                ca.ClearConfigurationCache("WebClient", "Web", true);
            }




            if (!url.IsNullOrEmpty())
            {
                // flush the CRM.WEB caches:
                url += "/default.aspx?flush";
                Response.Redirect(url);
            }

            // shouldn't happen, but just in case:
            var labels = _repeater.FindDescendants<Label>();

			var label = labels.First( lb => lb.ID == "_installFeedback" );
			if( label != null )
			{
				label.Visible = true;
				label.Text = "The app has been installed, please go to your online installation to try it out!";
			}
		}

		private void CreateWebPanelsInUsersInstallation( IEnumerable<Product> selectedProducts )
		{
            SuperOfficeAuthHelper.Authorize();

			var helper = new WebPanelHelper();

			foreach( var selectedProduct in selectedProducts )
			{
				helper.CreateAndSaveWebPanel( Global.AppName, selectedProduct.LinkName, selectedProduct.Url,
					selectedProduct.GetNavigation() );
			}
		}
	}
}