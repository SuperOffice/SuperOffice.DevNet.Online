using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SC = System.Collections;
using SCG = System.Collections.Generic;

public static class TypeHelper
	{
		/// <summary>
		/// Checks if the instance represents an enumerable type.
		/// <remarks>
		/// While <see cref="System.String"/> technically is an enumerable of char, it will
		/// not be reported as an enumerable type.
		/// </remarks>
		/// </summary>
		/// <param name="instance">The type to analyse</param>
		/// <returns>true if enumerable.</returns>
		public static bool IsEnumerable( this Type instance )
		{
			Type ignored;
			return IsEnumerable( instance, out ignored );
		}

		/// <summary>
		/// Checks if the instance represents an enumerable type and attempts to
		/// discover the type it enumerates over.
		/// <remarks>
		/// While <see cref="System.String"/> technically is an enumerable of char, it will
		/// not be reported as an enumerable type.
		/// </remarks>
		/// </summary>
		/// <param name="instance">The type to analyse</param>
		/// <param name="elementType">The type which the enumerable contains. Set to null if result type is unenumerable.</param>
		/// <returns>true if enumerable.</returns>
		public static bool IsEnumerable( this Type instance, out Type elementType )
		{
			if( instance == ( typeof( string ) ) )
			{
				// logically (but unintutitively) reported as enumerable of type char
				// this breaks lots of stuff, so override and say "no"
				elementType = null;
				return false;
			}
			else if( instance.IsArray )
			{
				elementType = instance.GetElementType();
				return true;
			}
			else if( IsGenericIList( instance, out elementType ) )
			{
				return true;
			}
			else if( IsGenericEnumerable( instance, out elementType ) )
			{
				return true;
			}
			else if( IsNongenericIList( instance ) )
			{
				elementType = typeof( object );
				return true;
			}
			else if( IsNongenericEnumerable( instance ) )
			{
				elementType = typeof( object );
				return true;
			}
			else
			{
				elementType = null;
				return false;
			}
		}

		/// <summary>
		/// Checks is the instance repesent an generic enemurable and attempts to discover the type it
		/// enumerates over.
		/// </summary>
		/// <param name="instance">The type to analyse</param>
		/// <param name="elementType">The type which the enumerable contains. Set to null if result type is unenumerable.</param>
		/// <returns>true if enumerable.</returns>
		public static bool IsGenericEnumerable( this Type instance, out Type elementType )
		{
			// we should NOT check if instance.IsGenericType.
			// among other things, this fails on C# methods using the yield-statement.
			// besides, we can implement generic interfaces without being a generic class.
			foreach( Type interfaceType in instance.GetAllInterfaces() )
			{
				if( false == interfaceType.IsGenericType )
				{
					continue;
				}

				Type genericType = interfaceType.GetGenericTypeDefinition();
				if( genericType == typeof( SCG.IEnumerable<> ) )
				{
					elementType = interfaceType.GetGenericArguments()[ 0 ];
					return true;
				}
			}

			elementType = null;
			return false;
		}

		/// <summary>
		/// Checks is the instance repesent an non-generic IEnumerable.
		/// <remarks>
		/// While <see cref="System.String"/> technically is an enumerable of char, it will
		/// not be reported as an enumerable type.
		/// </remarks>
		/// </summary>
		/// <param name="instance">The type to analyse</param>
		/// <returns>true if nongeneric IEnumerable.</returns>
		public static bool IsNongenericEnumerable( this Type instance )
		{
			if( instance == ( typeof( string ) ) )
			{
				// logically (but unintutitively) reported as enumerable of type char
				// this breaks lots of stuff, so override and say "no"
				return false;
			}
			else
			{
				return ( typeof( SC.IEnumerable ).IsAssignableFrom( instance ) );
			}
		}

		/// <summary>
		/// Checks is the instance repesent an generic List and attempts to discover the type it
		/// enumerates over.
		/// </summary>
		/// <param name="instance">The type to analyse</param>
		/// <param name="elementType">The type which the enumerable contains. Set to null if result type is unenumerable.</param>
		/// <returns>true if generic List.</returns>
		public static bool IsGenericIList( this Type instance, out Type elementType )
		{
			// we should NOT check if instance.IsGenericType.
			// among other things, this fails on C# methods using the yield-statement.
			// besides, we can implement generic interfaces without being a generic class.
			foreach( Type interfaceType in instance.GetAllInterfaces() )
			{
				if( false == interfaceType.IsGenericType )
				{
					continue;
				}

				Type genericType = interfaceType.GetGenericTypeDefinition();
				if( genericType == typeof( SCG.IList<> ) )
				{
					elementType = interfaceType.GetGenericArguments()[ 0 ];
					return true;
				}
			}

			elementType = null;
			return false;
		}

		/// <summary>
		/// Checks is the instance repesent an non-generic IList.
		/// </summary>
		/// <param name="instance">The type to analyse</param>
		/// <returns>true if nongeneric IList.</returns>
		public static bool IsNongenericIList( this Type instance )
		{
			return ( typeof( SC.IList ).IsAssignableFrom( instance ) );
		}

		/// <summary>
		/// Gets the public, readable instance-properties of a type.
		/// </summary>
		/// <param name="instance">The type to analyze.</param>
		/// <returns>Empty array if no properties found.</returns>
		public static SCG.IEnumerable<PropertyInfo> GetPublicReadableProperties( this Type instance )
		{
			var propertyInfos = instance
				.GetProperties( BindingFlags.Public | BindingFlags.Instance )
				.Where( pi => pi.CanRead );

			return propertyInfos;
		}

		/// <summary>
		/// Determines if the type represents a simple/primitive type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <returns>true if simple</returns>
		public static bool IsSimple( this Type type )
		{
			Type[] systemPrimitives = {
									 typeof (object), // TODO: include this or remove it?
                                     typeof (string),
									 typeof (long),
									 typeof (ulong),
									 typeof (int),
									 typeof (uint),
									 typeof (short),
									 typeof (ushort),
									 typeof (bool),
									 typeof (double),
									 typeof (float),
									 typeof (DateTime),
									 typeof (Decimal)
								 };

			return systemPrimitives.Contains( type );
		}

		/// <summary>
		/// Determins if the type represents a type which can represent decimal nubers.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <returns>true if decimal number</returns>
		public static bool IsDecimalNumber( this Type type )
		{
			Type[] decimalNumbers = {
									 typeof (double),
									 typeof (float),
									 typeof (Decimal)
								 };

			return decimalNumbers.Contains( type );
		}

		/// <summary>
		/// Determines if the type represents a complex type.
		/// </summary>
		/// <param name="type">The type to analyze.</param>
		/// <returns>true is complex.</returns>
		public static bool IsComplex( this Type type )
		{
			return !IsSimple( type );
		}

		/// <summary>
		/// Gets all interfaces implemented by the type.
		/// This may includes the type itself it if itself is an interface.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static Type[] GetAllInterfaces( this Type instance )
		{
			return GetAllInterfaces_Impl( instance ).ToArray();
		}

		private static IEnumerable<Type> GetAllInterfaces_Impl( Type instance )
		{
			if( instance.IsInterface )
			{
				yield return instance;
			}

			foreach( Type iinterface in instance.GetInterfaces() )
			{
				yield return iinterface;
			}
		}
	}
