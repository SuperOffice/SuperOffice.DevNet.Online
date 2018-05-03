using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonQuoteConnector
{
	[DataContract]
	public class JsonProductData
	{
		/// <summary>
		/// Reference/foreign key to the product in the product supplier system, if it exists there.
		/// </summary>
		[DataMember]
		public string ERPProductKey { get; set; }

		/// <summary>
		/// The name to use in the user interface
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// The description to use, with potentially several lines.
		/// Will be used as tool-tip to use in the list user interface too.
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// The product code / article number in the product supplier system.
		/// </summary>
		[DataMember]
		public string Code { get; set; }

		/// <summary>
		/// Unit for setting the sales quantity: 5-Pack, 10-cm, meter, ton, bushel, microsecond, gradus, τρυβλίον, 五合枡, دونم or whatever
		/// </summary>
		[DataMember]
		public string QuantityUnit { get; set; }

		/// <summary>
		/// Unit for determining the price: 5-Pack, 10-cm, meter, ton, bushel, microsecond, gradus, τρυβλίον, 五合枡, دونم or whatever
		/// </summary>
		[DataMember]
		public string PriceUnit { get; set; }

		/// <summary>
		/// Is this a running subscription, rather than a one-time buy
		/// </summary>
		[DataMember]
		public bool IsSubscription { get; set; }

		/// <summary>
		/// Either a List id to an id from a connector provided list, 
		/// or, if the connection doesn’t support lists, a text with the actual subscription unit (year, month, ... or some kind of volume unit).
		/// </summary>
		[DataMember]
		public string SubscriptionUnit { get; set; }

		/// <summary>
		/// Line item number, NOR: «Postnummer». Specific numbers from some hierarchy, for instance “1.4.3.2P”. 
		/// Typically used to sort the items in the quote by some standard way.
		/// </summary>
		[DataMember]
		public string ItemNumber { get; set; }

		/// <summary>
		/// URL to product information web page
		/// </summary>
		[DataMember]
		public string Url { get; set; }

		/// <summary>
		/// Either a List id to an id from a connector provided list, 
		/// or, if the connection doesn’t support lists, a text with the actual product category.
		/// </summary>
		[DataMember]
		public string ProductCategoryKey { get; set; }

		/// <summary>
		/// Either a List id to an id from a connector provided list, 
		/// or, if the connection doesn't support lists, a text with the actual product family.
		/// </summary>
		[DataMember]
		public string ProductFamilyKey { get; set; }

		/// <summary>
		/// Either a List id to an id from a connector provided list, 
		/// or, if the connection doesn’t support lists, a text with the actual product type.
		/// </summary>
		[DataMember]
		public string ProductTypeKey { get; set; }

		/// <summary>
		/// Name of the supplier of the product
		/// </summary>
		[DataMember]
		public string Supplier { get; set; }

		/// <summary>
		/// Suppliers part code/number or other key-like field
		/// </summary>
		[DataMember]
		public string SupplierCode { get; set; }

		/// <summary>
		/// The image of the product, if it exists. Base64 encoded string, or a valid URI that resolves to an image.
		/// Requires the “Provide-Image” capability.
		/// </summary>
		[DataMember]
		public string Image { get; set; }

		/// <summary>
		/// The thumbnail of the product, if it exists. Base64 encoded string, or a valid URI that resolves to an image.
		/// Requires the “Provide-Thumbnail” capability.
		/// </summary>
		[DataMember]
		public string Thumbnail { get; set; }

		/// <summary>
		/// A field for putting VATInfo you need to show in the final quoteDocument, like the VAT type that is used.
		/// Not used in any business logic in SuperOffice; available to document templates.
		/// </summary>
		[DataMember]
		public string VATInfo { get; set; }

		/// <summary>
		/// Tax/VAT if available from ERP system. 
		/// Could be either the percentage or the actual value.
		/// This is just to help out the layout of the quote in a document
		/// , but SuperOffice will not try to calculate this value.
		/// </summary>
		[DataMember]
		public double VAT { get; set; }

		/// <summary>
		/// The cost price. 
		/// Might not be given, use Decimal.MinValue to signal this.
		/// </summary>
		[DataMember]
		public double UnitCost { get; set; }

		/// <summary>
		/// The minimum price this salesman can offer to his customer. This might be cost price if there is no policy.
		/// Might not be given, use Decimal.MinValue to signal this.
		/// </summary>
		[DataMember]
		public double UnitMinimumPrice { get; set; }

		/// <summary>
		/// (Basic Price, normal price, standard price.)
		/// This is the basic price from which the discount is computed from. 
		/// The ListPrice will stay the same even when a larger amount is ordered.
		/// </summary>
		[DataMember]
		public double UnitListPrice { get; set; }

		/// <summary>
		/// Extra data (fields with labels). Shall be shown in the quoteline dialog.
		/// Bucket of additional info that the ERP system would like to store and show in the user interface. 
		/// <para />
		/// Information placed here is shown in the GUI if the “provide-extra-data” capability is true.
		/// Different products can have different fields.
		/// It will not be possible to directly put info here into the quote document.
		/// </summary>
		/// <example>
		/// &lt;Fields&gt;
		/// &lt;Field Name="Weight" Type="Double" Value="16.6" /&gt;
		///  &lt;Field Name="Height" Type="Double" Value="4.0" /&gt;
		///  &lt;Field Name="Arms" Type="Integer" Value="2" /&gt;
		///  &lt;Field Name="Certification" Type="String" Value="AB-ICE"  /&gt;
		///  &lt;Field Name="Weight" Type="String" Value="40°C" /&gt;
		/// &lt;/Fields&gt;
		/// </example>
		[DataMember]
		public JsonProductExtraDataField[] ExtraInfo { get; set; }

		/// <summary>
		/// Extra data (fields with labels) as string. Shall be shown in the quoteline dialog.
		/// Bucket of additional info that the ERP system would like to store and show in the user interface. 
		/// <para />
		/// Information placed here is shown in the GUI if the “provide-extra-data” capability is true.
		/// Different products can have different fields.
		/// It will not be possible to directly put info here into the quote document.
		/// </summary>
		/// <example>
		/// &lt;Fields&gt;
		/// &lt;Field Name="Weight" Type="Double" Value="16.6" /&gt;
		///  &lt;Field Name="Height" Type="Double" Value="4.0" /&gt;
		///  &lt;Field Name="Arms" Type="Integer" Value="2" /&gt;
		///  &lt;Field Name="Certification" Type="String" Value="AB-ICE"  /&gt;
		///  &lt;Field Name="Weight" Type="String" Value="40°C" /&gt;
		/// &lt;/Fields&gt;
		/// </example>
		[DataMember]
		public string RawExtraInfo { get; set; }

		/// <summary>
		/// name=right&amp;... of any fields that have non-standard field access rights
		/// Will be used by SuperOffice to control the user interface when showing the record.
		/// </summary>
		Dictionary<string, string> fieldRights;
		private string _rights;

		/// <summary>
		/// Rights attribute
		/// </summary>
		[DataMember]
		public string Rights
		{
			get { return _rights; }
			set
			{
				_rights = value;
				fieldRights = null;
			}
		}

		/// <summary>
		/// Returns the special product or quoteline right this product have
		/// </summary>
		/// <param name="fieldname"></param>
		/// <returns></returns>
		public string GetFieldRight( string fieldname )
		{
			InitializeFieldRightsDictionary();

			if( fieldRights.ContainsKey( fieldname ) )
				return fieldRights[ fieldname ];

			return string.Empty;
		}

		private void InitializeFieldRightsDictionary()
		{
			if( fieldRights == null )
			{
				fieldRights = new Dictionary<string, string>();

				if( Rights.HasContent() )
				{
					var fields = Rights.Split( new[] { '&' }, StringSplitOptions.RemoveEmptyEntries )
									   .Select( s => s.Split( new[] { '=' } ) );

					fieldRights = fields.ToDictionary( field => field[ 0 ], field => field[ 1 ] );
				}
			}
		}

		/// <summary>
		/// Does the name field have any right override?
		/// </summary>
		/// <param name="fieldname"></param>
		/// <returns></returns>
		public bool HasFieldRight( string fieldname )
		{
			return !String.IsNullOrEmpty( GetFieldRight( fieldname ) );
		}

		/// <summary>
		/// Does the named field has any special right overrides in this product or Quoteline?
		/// </summary>
		/// <param name="fieldname"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public bool HasFieldRight( string fieldname, char right )
		{
			string fieldRight = GetFieldRight( fieldname );

			return fieldRight.ToUpper().Contains( right );
		}

		/// <summary>
		/// Sets the right on the named field
		/// </summary>
		/// <param name="fieldname">case-sensitive name of QuoteLine field without tablename</param>
		/// <param name="fieldRight">N=None/hidden, R=Read-only, W=Write/Editable, M=Editable+Mandatory</param>
		/// <example>
		/// Rights field = ""
		/// fieldname="Quantity"
		/// fieldRight="W"
		/// 
		/// Result: Rights field = "Quanity=W"
		/// </example>
		/// <example>
		/// Rights field = "Quanity=M;SomeOtherField=W"
		/// fieldname="Quantity"
		/// fieldRight="W"
		/// 
		/// Result: Rights field = "Quanity=W;SomeOtherField=W"
		/// </example>
		public void SetRight( string fieldname, string fieldRight )
		{
			if( Rights.IsNullOrEmpty() )
			{
				Rights = "{0}={1}".FormatWith( fieldname, fieldRight );

				InitializeFieldRightsDictionary();
			}
			else
			{
				InitializeFieldRightsDictionary();
				fieldRights[ fieldname ] = fieldRight;

				Rights = string.Join( "&", fieldRights.Select( kv => kv.Key + "=" + kv.Value ).ToArray() );
			}
		}



		/// <summary>
		/// The names of one or more calculation rules that are in effect for this line, comma-separated case-insensitive
		/// Will NOT be used by SuperOffice.
		/// </summary>
		[DataMember]
		public string Rule { get; set; }

		/// <summary>
		/// This a simple field for adding information that the Connector can provide
		/// , and that the quote document need to display.
		/// </summary>
		[DataMember]
		public string ExtraField1 { get; set; }
		/// <summary>
		/// This a simple field for adding information that the Connector can provide
		/// , and that the quote document need to display.
		/// </summary>
		[DataMember]
		public string ExtraField2 { get; set; }
		/// <summary>
		/// This a simple field for adding information that the Connector can provide
		/// , and that the quote document need to display.
		/// </summary>
		[DataMember]
		public string ExtraField3 { get; set; }
		/// <summary>
		/// This a simple field for adding information that the Connector can provide
		/// , and that the quote document need to display.
		/// </summary>
		[DataMember]
		public string ExtraField4 { get; set; }
		/// <summary>
		/// This a simple field for adding information that the Connector can provide
		/// , and that the quote document need to display.
		/// </summary>
		[DataMember]
		public string ExtraField5 { get; set; }
	}



	/// <summary>
	/// A product is some thing or service that can be sold or leased to a customer.
	/// </summary>
	public class JsonProduct : JsonProductData
	{
		/// <summary>
		/// Helper for simplified debugging.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format( "{0}/{1}: {2} [{3}] {4}", ERPProductKey, ERPPriceListKey, Code, QuantityUnit, Name );
		}

		/// <summary>
		/// Foreign key to the price list that this quoteline is a part of.
		/// </summary>
		[DataMember]
		public string ERPPriceListKey { get; set; }

		/// <summary>
		/// True for products that should currently be offered, false when the product is discontinued and should not ordinarily be offered.
		/// 
		/// When false the product no longer appears in search results.
		/// </summary>
		[DataMember]
		public bool InAssortment { get; set; }

		/// <summary>
		/// Negative numbers will be interpreted as how many is ordered. 
		/// 
		/// Might not be available.
		/// Requires the “Provide-Stock-data” capability, and that the ERP system is available.
		/// </summary>
		[DataMember]
		public double InStock { get; set; }

		/// <summary>
		/// If <see cref="JsonProductData.IsSubscription"/>, then this field specifies the default number of <see cref="JsonProductData.SubscriptionUnit"/> offered
		/// </summary>
		[DataMember]
		public double DefaultSubscriptionQuantity { get; set; }

		///// <summary>
		///// Returns a QuoteLineInfo from the current ProductDataInfo object
		///// </summary>
		///// <param name="context">Context to perform a conversion in</param>
		///// <returns></returns>
		//public QuoteLineInfo GetQuoteLineFromProduct( QuoteAlternativeContextInfo context )
		//{
		//	var line = new QuoteLineInfo()
		//	{
		//		Code = this.Code,
		//		DeliveredQuantity = 0,
		//		Description = this.Description,
		//		ERPProductKey = this.ERPProductKey,
		//		ItemNumber = this.ItemNumber,
		//		Name = this.Name,
		//		Quantity = 1,
		//		QuoteAlternativeId = context.CRMAlternativeWithLines.CRMAlternative.QuoteAlternativeId,
		//		Rights = this.Rights,
		//		Rule = this.Rule,
		//		Supplier = this.Supplier,
		//		SupplierCode = this.SupplierCode,
		//		Thumbnail = this.Thumbnail,
		//		QuantityUnit = this.QuantityUnit,
		//		PriceUnit = this.PriceUnit,
		//		UnitCost = this.UnitCost,
		//		UnitListPrice = this.UnitListPrice,
		//		UnitMinimumPrice = this.UnitMinimumPrice,
		//		VAT = this.VAT,
		//		VATInfo = this.VATInfo,
		//		ProductCategoryKey = this.ProductCategoryKey,
		//		ProductFamilyKey = this.ProductFamilyKey,
		//		ProductTypeKey = this.ProductTypeKey,
		//		ExtraField1 = this.ExtraField1,
		//		ExtraField2 = this.ExtraField2,
		//		ExtraField3 = this.ExtraField3,
		//		ExtraField4 = this.ExtraField4,
		//		ExtraField5 = this.ExtraField5,
		//		ExtraInfo = this.ExtraInfo,
		//		RawExtraInfo = this.RawExtraInfo,
		//		Url = this.Url,

		//		//SubTotal = this.UnitListPrice,
		//		//TotalPrice = this.UnitListPrice,

		//	};

		//	return line;
		//}
	}
}
