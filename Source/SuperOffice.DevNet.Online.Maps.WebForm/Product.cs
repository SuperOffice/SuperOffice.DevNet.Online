using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
//using SuperOffice.Online.PartnerLogin;
using SuperOffice.Util;

namespace SuperOffice.DevNet.Online.Maps.WebForm
{
	public enum EProduct
	{
		Contact = 1,
		Project = 2,
		Diary = 3,
		Selection = 4,

		BuyAll = int.MaxValue

	}
	public class Product
	{
		public int ID { get; set; }

		public EProduct ProductId
		{
			get { return (EProduct) ID; }
			set { ID = (int)value; }
		}

		public string Name { get; set; }

		public decimal Price { get; set; }

		public string Url { get; set; }

		/// <remarks>
		/// Should be changed if we want to support several currencies
		/// Should then not be static either
		/// </remarks>
		public static string CurrentCurrencyString
		{
			get { return "€"; }
		}

		/// <remarks>
		/// Should be changed to place currency marker correctly
		/// </remarks>
		public string PriceCurrency
		{
			get
			{
				return "{0} {1}".FormatWith( CurrentCurrencyString, Price );
				
			}
		}

		/// <summary>
		/// The linkname in SuperOffice
		/// </summary>
		public string LinkName { get; set; }


		public string Description { get; set; }

		public bool IsSelected { get; set; }


		public ObservableCollection<Product> Products { get; set; }
		public ObservableCollection< Product> GetProducts()
		{
			return Products ?? (Products = new ObservableCollection<Product>
			{
				new Product
				{
					ProductId = EProduct.Contact,
					Name = "Company screen",
					LinkName = "Location",
					Description =
						"Shows the location of the company, based on the streetaddress (or the postal address if no street address is registered).",
					Price = 1.9m,
					IsSelected = true,
					Url = ConfigurationManager.AppSettings[ "BaseURL" ] + "MapsExampleForContact.aspx?ContactId=<cuid>",
				},
				new Product
				{
					ProductId = EProduct.Project,
					Name = "Project screen",
					LinkName = "ProjectMembers Location",
					Description = "Shows the location of the projectmembers.",
					Price = 1.9m,
					Url = ConfigurationManager.AppSettings[ "BaseURL" ] + "MapsExampleForProject.aspx?ProjectId=<prid>",
				},
				new Product
				{
					ProductId = EProduct.Diary,
					Name = "Diary screen",
					LinkName = "Todays Locations",
					Description = "Shows the location of the companies you are having meetings with today.",
					Price = 1.9m,
					Url = ConfigurationManager.AppSettings[ "BaseURL" ] + "MapsExampleForDiary.aspx?UserId=<usid>",
				},
				new Product
				{
					ProductId = EProduct.Selection,
					LinkName = "Selection Locations",
					Name = "Selection screen",
					Description =
						"Shows the location of the selection members. If, for instance, it is an appointment selection, it shows the location of the companies the appointments are with.",
					Price = 1.9m,
					Url = ConfigurationManager.AppSettings[ "BaseURL" ] + "MapsExampleForSelection.aspx?SelectionId=<slid>",
				},
				//new WebPanel
				//{
				//	ID = 5,
				//	Name = "Buy all",
				//	Description =
				//		"Gives you all of the views listed above, to a nicer price!",
				//	Price = 5.9m,
				//	IsAll = true
				//},
			});
		}

		public Dictionary<EProduct, Product> GetProductsDic()
		{
			var webPanels = GetProducts();

			return webPanels.ToDictionary( wp => wp.ProductId, wp => wp );			
		}

		public static Product GetBuyAllProduct()
		{
			return new Product
			{
				ProductId = EProduct.BuyAll,
				Name = "Buy all",
				Description =
					"Gives you all of the views listed above, to a nicer price!",
				Price = 5.9m,
			};

		}

		internal Data.Navigation GetNavigation()
		{
			switch( ProductId )
			{
				case EProduct.Contact:
				return Data.Navigation.ContactCard;
				case EProduct.Project:
				return Data.Navigation.ProjectCard;
				case EProduct.Diary:
				return Data.Navigation.ButtonPanelTask;//DiaryMinicard;
				case EProduct.Selection:
				return Data.Navigation.ButtonPanelTask;//SelectionCard;

				default:
					throw new Exception( "Unknown ProductId, can't know where in SuperOffice to create the web panel");
			}
		}
	}
}