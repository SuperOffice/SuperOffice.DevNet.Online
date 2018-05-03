using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonQuoteConnector;
using Newtonsoft.Json;
using SuperOffice.CRM;

namespace SuperOffice
{
	[QuoteConnector( Name )]
	public class JsonQuoteConnector : QuoteConnectorBase
	{
		private string _foldername;
		public const string Name = "JsonQuoteConnector";
		protected InMemoryProductProvider ProductProvider { get; set; }

		private JsonDataReader Reader { get; set; }

		public JsonQuoteConnector()
		{
		}

		public override Dictionary<string, FieldMetadataInfo> GetConfigurationFields()
		{
			var res = new Dictionary<string, FieldMetadataInfo>
					   {
						   {
							   "#1", new FieldMetadataInfo()
										 {
											 Access = FieldAccessInfo.Mandatory,
											 DefaultValue = string.Empty,
											 DisplayName =
												 "US:\"Json files folder name\";NO:\"Json mappenavn\";GE:\"JJSON-Dateien Ordnernamen\";FR:\"Json fichiers nom du dossier\"",
											 DisplayDescription =
												 "US:\"The name of the folder where the Json files to load pricelists and products from are found\";"
												 + "NO:\"Navnet på mappen der JSON-filer å laste prislister og produkter fra er funnet\";"
												 + "GE:\"Der Name des Ordners, in dem die JSON-Dateien von Preislisten und Produkte zu laden gefunden\";"
												 + "FR:\"Le nom du dossier dans lequel les fichiers JSON pour charger les listes de prix et de produits à partir se trouvent\"",
											 FieldType = FieldMetadataTypeInfo.Text,
											 FieldKey = "#1",
											 Rank = 1,
										 }
						   },
						   //{
							  // "#2", new FieldMetadataInfo()
									//	 {
									//		 DisplayName = "[SR_ADMIN_IMPORT_ONLY_Json]",
									//		 DisplayDescription = "[SR_ADMIN_IMPORT_ERROR_XLS]",
									//		 FieldType = FieldMetadataTypeInfo.Label,
									//		 FieldKey = "#2",
									//		 Rank = 2,
									//	 }
						   //},
						   //{
							  // "#3", new FieldMetadataInfo()
									//	 {
									//		 DisplayName = "[SR_LABEL_CATEGORY]",
									//		 FieldType = FieldMetadataTypeInfo.List,
									//		 ListName = "productcategory",
									//		 FieldKey = "#3",
									//		 Rank = 3,
									//	 }
						   //}

					   };

			return res;
		}

		public override CRM.PluginResponseInfo InitializeConnection( CRM.QuoteConnectionInfo connectionData
																	, UserInfo user
																	, bool isOnTravel
																	, Dictionary<string, string> connectionConfigFields
																	, CRM.IProductRegisterCache productRegister )
		{
			var retv = CheckConnectionData( connectionConfigFields );

			if( connectionConfigFields == null || connectionConfigFields.Count == 0 )
				throw new Exception( "Tried to initialize JsonQuoteConnector with no connection config!" );

			ReadInData();

			return retv;
		}

		public override bool CanProvideCapability( string capabilityName )
		{
			bool retv = false;
			switch( capabilityName )
			{
				case CRMQuoteConnectorCapabilities.CanProvideCost:
				case CRMQuoteConnectorCapabilities.CanProvideMinimumPrice:
				case CRMQuoteConnectorCapabilities.CanProvidePicture:
				case CRMQuoteConnectorCapabilities.CanProvideExtraData:
				case CRMQuoteConnectorCapabilities.CanProvideStockData:
                case CRMQuoteConnectorCapabilities.CanPlaceOrder:
                    retv = true;
				break;

				case CRMQuoteConnectorCapabilities.CanProvideOrderState:
				case CRMQuoteConnectorCapabilities.CanSendOrderConfirmation:
				retv = false;
				break;

				case CRMQuoteConnectorCapabilities.CanProvideProductCategoryList:
				//retv = true;
				//break;

				case CRMQuoteConnectorCapabilities.CanProvideProductFamilyList:
				case CRMQuoteConnectorCapabilities.CanProvideProductTypeList:
				retv = false;
				break;

				case CRMQuoteConnectorCapabilities.CanProvidePaymentTermsList:
				case CRMQuoteConnectorCapabilities.CanProvidePaymentTypeList:
				case CRMQuoteConnectorCapabilities.CanProvideDeliveryTermsList:
				case CRMQuoteConnectorCapabilities.CanProvideDeliveryTypeList:
				//retv = true;
				//break;

				case CRMQuoteConnectorCapabilities.CanPerformComplexSearch:
				retv = false;
				break;

				case CRMQuoteConnectorCapabilities.CanProvideAddresses:
				//retv = true;
				retv = false;
				break;
			}
			return retv;
		}

		public override PluginResponseInfo TestConnection( Dictionary<string, string> connectionConfigFields )
		{
			return CheckConnectionData( connectionConfigFields );
		}

		private PluginResponseInfo CheckConnectionData( Dictionary<string, string> connectionConfigFields )
		{
			if( connectionConfigFields != null && connectionConfigFields.Count() >= 1 )
			{
                if (String.IsNullOrWhiteSpace(FolderName))
                    FolderName = connectionConfigFields.First().Value;

                if ( Reader.ValidatePath() )
				{
					return ProductProvider.ValidateData();
					//.Merge( ValidateList( Reader.DeliveryTerms, "DeliveryTerms" ) )
					//.Merge( ValidateList( Reader.DeliveryTypes, "DeliveryTypes" ) )
					//.Merge( ValidateList( Reader.PaymentTerms, "PaymentTerms" ) )
					//.Merge( ValidateList( Reader.PaymentTypes, "PaymentTypes" ) )
					//.Merge( ValidateList( Reader.ProductCategories, "ProductCategories" ) );
				}
				else
					return GetFileNotFoundResponse();
			}
			else
				return GetWrongConfigResponse();
		}

		private PluginResponseInfo ValidateList( List<ListItemInfo> list, string listname )
		{
			var uniqueItems = list.Select( item => item.ERPQuoteListItemKey ).Distinct().ToArray();
			if( uniqueItems.Length != list.Count() )
				return new PluginResponseInfo()
				{
					IsOk = false,
					State = ResponseState.Error,
					UserExplanation = "US:\"{0}\";NO:\"{1}\";GE:\"{2}\";FR:\"{3}\"".FormatWith(
									"All listitems in '{0}' list must have an unique 'ERPQuoteListItemKey'.".FormatWith( listname )
									, "Alle listeelementer i '{0}' listen må ha en unik 'ERPQuoteListItemKey'.".FormatWith( listname )
									, "Alle Listenelemente in '{0}' Liste muss über eine eindeutige' ERPQuoteListItemKey '.".FormatWith( listname )
									, "Tous les éléments de la liste dans '{0}' liste doit avoir un unique 'ERPQuoteListItemKey'.".FormatWith( listname )
									),
					TechExplanation = "Some items in {0} had the same ERPQuoteListItemKey.".FormatWith( listname )
				};
			else
				return new PluginResponseInfo() { State = ResponseState.Ok };
		}

		private static PluginResponseInfo GetFileNotFoundResponse()
		{
			return new PluginResponseInfo()
			{
				IsOk = false,
				State = ResponseState.Error
				,
				UserExplanation = "US:\"{0}\";NO:\"{1}\";GE:\"{2}\";FR:\"{3}\"".FormatWith(
								  "You must enter an absolute folder name to where the Json files resides in the file hierarchy."
								  , "Du må angi et absolutt mappenavn til der JSON filene ligger i filhierarkiet."
								  , "Sie müssen einen absoluten Ordnernamen eingeben, wo die JSON-Dateien in der Dateihierarchie befindet."
								  , "Vous devez entrer un nom de dossier absolu à l'endroit où les fichiers Json réside dans la hiérarchie des fichiers"
								  )
				,
				TechExplanation = "File not found :-("
			};
		}

		private static PluginResponseInfo GetWrongConfigResponse()
		{
			return new PluginResponseInfo()
			{
				IsOk = false,
				State = ResponseState.Error,
				UserExplanation = "Technical error: Config parameters were null or not exactly one element long",
				TechExplanation = "ConnectionData was null or had not excactly one element"
			};
		}


		public string FolderName
		{
			get { return _foldername; }
			set
			{
				_foldername = value;

				ReadInData();
			}
		}

		private void ReadInData()
		{
			if( FolderName == null )
				return;

			Reader = new JsonDataReader( FolderName );
			ProductProvider = Reader.ReadInData();
		}


		public override int GetNumberOfActivePriceLists( string isoCurrencyCode )
		{
			ReadInData();
			return ProductProvider.GetNumberOfActivePriceLists( isoCurrencyCode );
		}

		public override PriceListInfo[] GetActivePriceLists( string isoCurrencyCode )
		{
			ReadInData();
			return ProductProvider.GetActivePriceLists( isoCurrencyCode );
		}

		public override PriceListInfo[] GetAllPriceLists( string isoCurrencyCode )
		{
			ReadInData();
			return ProductProvider.GetAllPriceLists( isoCurrencyCode );
		}





		public override ProductInfo[] FindProduct(QuoteAlternativeContextInfo context, string currencyCode, string userinput,
			string priceListKey)
		{
			ReadInData();

			var res = ProductProvider.FindProduct( context, currencyCode, userinput, priceListKey );
			return res;
		}

		public override ProductInfo GetProduct(QuoteAlternativeContextInfo context, string erpProductKey)
		{
			ReadInData();
			var product = ProductProvider.GetProduct( context, erpProductKey );
			if( !CanProvideCapability( CRMQuoteConnectorCapabilities.CanProvideCost ) )
				product.UnitCost = 0;
			if( !CanProvideCapability( CRMQuoteConnectorCapabilities.CanProvideMinimumPrice ) )
				product.UnitMinimumPrice = 0;
			return product;
		}

		public override ProductInfo[] GetProducts(QuoteAlternativeContextInfo context, string[] erpProductKeys)
		{
			ReadInData();
			return ProductProvider.GetProducts( context, erpProductKeys );
		}

		public override QuoteLineInfo[] GetQuoteLinesFromProduct(QuoteAlternativeContextInfo context, string erpProductKey)
		{
			ReadInData();
			var products = ProductProvider.GetQuoteLinesFromProduct( context, erpProductKey );
			foreach( var product in products )
			{
				if( !CanProvideCapability( CRMQuoteConnectorCapabilities.CanProvideCost ) )
					product.UnitCost = 0;
				if( !CanProvideCapability( CRMQuoteConnectorCapabilities.CanProvideMinimumPrice ) )
					product.UnitMinimumPrice = 0;
				// fill in random data for ERP field
				if( string.IsNullOrEmpty( product.ERPQuoteLineKey ) )
					product.ERPQuoteLineKey = GenKey();
			}
			return products;
		}
		static Random rnd = new Random();
		private string GenKey()
		{
			int x = rnd.Next();
			string fmt = x.ToString( "X" ) + "000000";
			return fmt.Substring( 0, 6 );
		}


		public override int GetNumberOfProductImages( string erpProductKey )
		{
			ReadInData();
			if( !CanProvideCapability( CRMQuoteConnectorCapabilities.CanProvidePicture ) )
				return 0;
			return ProductProvider.GetNumberOfProductImages( erpProductKey );
		}

		public override string GetProductImage( string erpProductKey, int rank )
		{
			ReadInData();
			if( !CanProvideCapability( CRMQuoteConnectorCapabilities.CanProvidePicture ) )
				return null;
			return ProductProvider.GetProductImage( erpProductKey, rank );
		}






		public override void OnAfterSaveQuote(QuoteAlternativeContextInfo context)
		{
			//ReadInData();
		}

		public override void OnBeforeDeleteQuote(QuoteInfo quote, ISaleInfo sale, IContactInfo contact)
		{
			//ReadInData();
		}

        public override PlaceOrderResponseInfo PlaceOrder(QuoteAlternativeContextInfo context)
        {
            var placeOrderPath = Path.Combine(_foldername, "placeorder_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture) + ".json");

            File.WriteAllText(placeOrderPath, JsonConvert.SerializeObject(context, Formatting.Indented));

            return new PlaceOrderResponseInfo(context)
            {
                IsOk = true,
                State = ResponseState.Ok
            };
        }


    }
}
