using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonQuoteConnector
{
	/// <summary>
	/// String, int, decimal, image, url, etc. How should the value be interpreted.
	/// </summary>
	[DataContract( Name = "JsonExtraDataFieldType" )]
	public enum JsonExtraDataFieldType
	{
		/// <summary>
		/// 0: Anything to be shown as text, left aligned.
		/// The field can contain formatspecifiers (use this for displaying numbers correctly as the users computer is set up)
		/// </summary>
		[EnumMember(Value = "String")]
		String = 0,


		/// <summary>
		/// 1: A web address
		/// </summary>
		[EnumMember(Value = "Url")]
		Url = 1,

		/// <summary>
		/// 2: Contains the url of the image, or a base64 encoded version of it.
		/// </summary>
		[EnumMember(Value = "Image")]
		Image = 2,

	}

	/// <summary>
	/// A way to show some simple extra data on a product, typically to hep the user to identify the correct product. 
	/// Basically a bucket of additional info that the ERP system would like to store and show in the user interface. 
	/// Information placed here is shown in the GUI if the “provide-extra-data” capability is true.
	/// </summary>
	[DataContract]
	public class JsonProductExtraDataField
	{

		/// <summary>
		/// Label for the field
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Value for the field. If type is string, it can contain formatspecifiers (use this for displaying numbers correctly as the users computer is set up).
		/// </summary>
		[DataMember]
		public string Value { get; set; }

		/// <summary>
		/// String, image, url,. 
		/// How the value should be interpreted.
		/// </summary>
		[DataMember]
		public JsonExtraDataFieldType Type { get; set; }

		/// <summary>
		/// Creates a XML representation of an array of ProductExtraDataFieldInfo
		/// </summary>
		/// <param name="productExtraInfos">Extra information structures</param>
		/// <returns></returns>
		public static string GetXMLRepresentation( JsonProductExtraDataField[] productExtraInfos )
		{
			if( productExtraInfos == null || productExtraInfos.Length == 0 )
				return null;

			string retv = "<Fields>\n";
			foreach( var field in productExtraInfos )
			{
				retv += "<Field Name=\"{0}\" Type=\"{1}\"><![CDATA[{2}]]></Field>\n".FormatWith( field.Name
																						, field.Type.ToString()
																						, field.Value );
			}

			retv += "</Fields>";

			return retv;
		}
	}
}
