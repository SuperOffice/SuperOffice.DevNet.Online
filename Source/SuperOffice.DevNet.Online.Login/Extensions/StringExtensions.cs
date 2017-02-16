using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SuperOffice.DevNet.Online.Login.Extensions
{
	public static class StringExt
	{
		public static bool IsNullOrEmpty( this string me )
		{
			return System.String.IsNullOrEmpty( me );
		}

		public static bool HasContent( this string me )
		{
			return !me.IsNullOrEmpty();
		}

		public static bool HasRoomFor( this string me, string toAdd, int maxLength = 254 )
		{
			return me != null &&
				   toAdd != null &&
				   ( me.Length + toAdd.Length ) <= maxLength;
		}

		public static string AddLine( this string me, string textToAdd, int maxLength = 254 )
		{
			return Add( me, textToAdd, "\n", maxLength );
		}

		public static string Add( this string me, string textToAdd, string divider, int maxLength = 2048 )
		{
			me = me ?? "";
			textToAdd = textToAdd ?? "";

			if( me.HasContent() && textToAdd.HasContent() )
				textToAdd = divider + textToAdd;

			if( me.HasContent() && me.HasRoomFor( textToAdd ) )
				me += textToAdd;
			else if( !me.HasContent() )
				me = textToAdd; // long first message gets trimmed - so be it. Something better than nothing

			return me;
		}
	}
}