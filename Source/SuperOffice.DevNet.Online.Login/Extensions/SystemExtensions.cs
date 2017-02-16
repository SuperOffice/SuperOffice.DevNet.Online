using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperOffice.DevNet.Online.Login.Extensions
{
	public static class SystemExtensions
	{
		public static string ToStringInvariant( this int me )
		{
			return me.ToString( CultureInfo.InvariantCulture );
		}
	}
}
