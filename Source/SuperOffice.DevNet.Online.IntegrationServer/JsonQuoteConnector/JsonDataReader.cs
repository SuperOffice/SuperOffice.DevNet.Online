using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using JsonQuoteConnector.Data;
using SuperOffice.CRM;

namespace JsonQuoteConnector
{
	public class JsonDataReader
	{
		public string FolderName { get; set; }
		public InMemoryProductProvider ProductProvider { get; set; }

		public JsonDataReader()
		{
			ProductProvider = new InMemoryProductProvider();
			ProductProvider.Images = new Dictionary< string , string[]>();
		}
		public JsonDataReader(string foldername)
		: this()
		{
			FolderName = foldername;
		}

		public InMemoryProductProvider ReadInData()
		{
			if( FolderName.IsNullOrEmpty() )
				throw new ArgumentException( "We must have a Folder to load data from." );

			//Book book = ReadInBook();

			//ReadCapabilities( book );
			//ReadAddresses( book );

			ReadPricelists();
			ReadProducts();

			//ReadPaymentTerms( book );
			//ReadPaymentTypes( book );
			//ReadDeliveryTerms( book );
			//ReadDeliveryTypes( book );

			//ReadProductCategories( book );
			//ReadProductFamilies( book );
			//ReadProductTypes( book );

			return ProductProvider;
		}
		private List<TSo> ReadJson<TJson, TSo>( string filepath, Func<TJson, TSo, bool> extra ) where TSo : new()
		{
			DataContractJsonSerializer serializer =
				new DataContractJsonSerializer(typeof ( TJson[])
				, new DataContractJsonSerializerSettings
				{
					DateTimeFormat = new DateTimeFormat( "yyyy-mm-ddThh:MM:ss" )
				}
				);

			using (var stream = File.OpenRead( filepath ) )
			{
				var jsonlist = (TJson[]) serializer.ReadObject(stream);

				var soLists = new List<TSo>(jsonlist.Length);

				var mapper = SoMapper.Instance;
				foreach (var json in jsonlist)
				{
					var price = new TSo();

					mapper.CopyProperties<TJson, TSo>(json, price, false, true);

					extra(json, price);

					soLists.Add(price);
				}

				return soLists;
			}
		}

		private void ReadPricelists()
		{
			ProductProvider.PriceLists = ReadJson< JsonPriceList, PriceListInfo >( PriceListsPath, ExtraForPriceList );
		}

		private bool ExtraForPriceList(JsonPriceList jsonPriceList, PriceListInfo priceListInfo)
		{
			DateTime from;
			if (DateTime.TryParse(jsonPriceList.ValidFromString, out from))
				priceListInfo.ValidFrom = from;

			DateTime to;
			if( DateTime.TryParse( jsonPriceList.ValidToString, out to ) )
				priceListInfo.ValidTo = to;
			
			return true;
		}


		private void ReadProducts()
		{
			ProductProvider.Products = ReadJson<JsonProduct, ProductInfo>( ProductsPath, ExtraForProducts);
		}

		private bool ExtraForProducts( JsonProduct jsonProduct, ProductInfo productInfo)
		{
			ProductProvider.Images[ jsonProduct.ERPProductKey ] = new string[] { jsonProduct.Image };
			return true;
		}

		public string ProductsPath
		{
			get { return Path.Combine(FolderName, "Products.Json"); }
		}
		public string PriceListsPath
		{
			get { return Path.Combine( FolderName, "PriceLists.Json" ); }
		}

		public bool ValidatePath()
		{
			return System.IO.File.Exists(ProductsPath) && System.IO.File.Exists( PriceListsPath );
		}

		public List<ListItemInfo> DeliveryTerms { get; set; }
		public List<ListItemInfo> DeliveryTypes { get; set; }
		public List<ListItemInfo> PaymentTerms { get; set; }
		public List<ListItemInfo> PaymentTypes { get; set; }
		public List<ListItemInfo> ProductCategories { get; set; }

	}
}
