using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonQuoteConnector.Data
{
	[DataContract]
	public class JsonPriceList
	{
		/// <summary>
		/// Reference to the pricelist in the product supplier system.
		/// </summary>
		[DataMember]
		public string ERPPriceListKey
		{
			get;
			set;
		}

		/// <summary>
		/// The connection in SuperOffice this pricelist comes from.
		/// </summary>
		[DataMember]
		public int QuoteConnectionId
		{
			get;
			set;
		}

		/// <summary>
		/// Name of this pricelist to use in the user interface.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Description of this pricelist , will be used as tool-tip in the user interface.
		/// </summary>
		[DataMember]
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// The iso currency code, like 'USD' or 'NOK'.
		/// </summary>
		[DataMember]
		public string Currency
		{
			get;
			set;
		}

		/// <summary>
		/// The name to use in the user interface, like perhaps 'US dollar' or '$'
		/// </summary>
		[DataMember]
		public string CurrencyName
		{
			get;
			set;
		}

		/// <summary>
		/// The date (inclusive) the pricelist start to be valid. 
		/// This can be DateTime.MinValue to signal that it doesn't have a specific start date.
		/// </summary>
		[DataMember]
		public string ValidFromString
		{
			get;
			set;
		}

		/// <summary>
		/// The date (inclusive) the pricelist ends to be valid. 
		/// This can be DateTime.MaxValue to signal that it doesn't have a specific end date.
		/// </summary>
		[DataMember]
		public string ValidToString
		{
			get;
			set;
		}

		///// <summary>
		///// Checks if the pricelist is valid according to the current date
		///// </summary>
		///// <returns>True if valid according to current date</returns>
		///// <remarks>
		///// If ValidTo is set to DateTime.MinValue, it will be interpreted as valid
		///// </remarks>
		//public bool IsValid()
		//{
		//	return ( ValidFrom < DateTime.Now ) &&
		//		   ( ValidTo == DateTime.MinValue || ValidTo > DateTime.Now );
		//}

		/// <summary>
		/// Is the list active (as opposed to being worked on, suddenly canceled, etc.
		/// </summary>
		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}

	}
}
