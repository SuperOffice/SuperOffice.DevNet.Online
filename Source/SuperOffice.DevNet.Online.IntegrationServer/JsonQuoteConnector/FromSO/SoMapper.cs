using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonQuoteConnector
{
	public class SoMapper
	{
		/*
         * stateful API
         */

		// TODO: add explicit mapping capabilities
		//private Dictionary<Type,Dictionary<Type,List<Action<object,object>>>> _mapFunctions;

		/// <summary>
		/// Default constructor
		/// </summary>
		public SoMapper()
		{
			// TODO: add explicit mapping capabilities
			//_mapFunctions = new Dictionary<Type, Dictionary<Type, List<Action<object, object>>>>();
		}

		/*
         * static API
         */
		#region static

		private static readonly SoMapper _instance = new SoMapper();

		/// <summary>
		/// The default-instance of SoMapper for usage in cases where customization is not needed.
		/// </summary>
		public static SoMapper Instance
		{
			get { return _instance; }
		}

		#endregion static

		//public void AddMap<TInput,TOutput>(params Action<TInput,TOutput>[] mapFunctions)
		//{
		// // TODO: make it use expressions.
		//    foreach(Action<TInput,TOutput> mapFunction in mapFunctions)
		//    {
		//        Action<TInput, TOutput> currentMapFunction = mapFunction;
		//        Action<object, object> typeWrapper = (p1, p2) => currentMapFunction.DynamicInvoke(p1, p2);

		//        List<Action<object,object>> list = _mapFunctions[typeof (TInput)][typeof (TOutput)];
		//        list = list ?? new List<Action<object, object>>();
		//        list.Add(typeWrapper);
		//    }
		//}

		private const bool THROW_ON_MISMATCH = true;

		/// <summary>
		/// Convert the given input instance and return a instance of the specified output type
		/// using fuzzy Duck-typing and logic.
		/// <remarks>
		/// Will return same instance if the provided types are the same.
		/// </remarks>
		/// </summary>
		/// <typeparam name="TInput">Type of input-value.</typeparam>
		/// <typeparam name="TResult">Type of desired return-value.</typeparam>
		/// <param name="source">Value to convert.</param>
		/// <param name="throwOnMismatch">
		/// If true, throws when there is a mismatch between the types being converted.
		/// If false, silently ignores property with mismatch and moves along to the next one.
		/// </param>
		/// <returns>The converted value.</returns>
		public TResult Map<TInput, TResult>( TInput source, bool throwOnMismatch = THROW_ON_MISMATCH )
			where TResult : new()
		{
			if( typeof( TInput ) == typeof( TResult ) )
			{
				// we DO know better than the compiler
				return (TResult)(object)source;
			}

			return Copy_Impl<TInput, TResult>( source, throwOnMismatch );
		}

		/// <summary>
		/// Clones the given object-graph using a recursive copy of public properties.
		/// <remarks>
		/// If class being copied depends on private fields, the copy may not function as expected.
		/// </remarks>
		/// </summary>
		/// <typeparam name="TInput">Type of input-value.</typeparam>
		/// <param name="source">Instance to clone.</param>
		/// <returns>The converted value.</returns>
		public TInput Clone<TInput>( TInput source )
			where TInput : new()
		{
			return Copy_Impl<TInput, TInput>( source );
		}


		/// <summary>
		/// Convert the given input instance and return a instance of the specified output type
		/// using fuzzy Duck-typing and logic.
		/// <remarks>
		/// Will return same instance if the provided types are the same.
		/// </remarks>
		/// </summary>
		/// <typeparam name="TInput">Type of input-value.</typeparam>
		/// <typeparam name="TResult">Type of desired return-value.</typeparam>
		/// <param name="source">Value to convert.</param>
		/// <param name="throwOnMismatch">
		/// If true, throws when there is a mismatch between the types being converted.
		/// If false, silently ignores property with mismatch and moves along to the next one.
		/// </param>
		/// <returns>The converted value.</returns>
		private TResult Copy_Impl<TInput, TResult>( TInput source, bool throwOnMismatch = THROW_ON_MISMATCH )
			where TResult : new()
		{
			if( source == null )
			{
				return default( TResult );
			}

			Type sourceType = typeof( TInput );
			Type destinationType = typeof( TResult );

			// if types are enum, we dont want to iterate over properties and stuff like that.
			// (it's a waste of time, and besides: Enums are value-typed so it wont ever work)
			if( sourceType.IsEnum && destinationType.IsEnum )
			{
				object destination = CopyEnum( destinationType, source, throwOnMismatch );
				TResult enumResult = (TResult)destination;
				return enumResult;
			}

			TResult result = new TResult();
			CopyProperties( source, result, throwOnMismatch );

			return result;
		}



		/// <summary>
		/// Copies all properties of the provided source-instance onto the provided destination-instance.
		/// </summary>
		/// <typeparam name="TSource">Type of the source value.</typeparam>
		/// <typeparam name="TDestination">Type of the destination value.</typeparam>
		/// <param name="source">Source-value to copy.</param>
		/// <param name="destination">Target-value which properties will be copied onto.</param>
		/// <param name="throwOnMismatch">
		/// If true, throws when there is a mismatch between the types being converted.
		/// If false, silently ignores property with mismatch and moves along to the next one.
		/// </param>
		/// <param name="ignorePropertiesWithoutSetter">
		/// Will ignore properties that has only a get function
		/// </param>
		public void CopyProperties<TSource, TDestination>( TSource source, TDestination destination, bool throwOnMismatch, bool ignorePropertiesWithoutSetter = false )
		{
			Type sourceType = typeof( TSource );
			Type destinationType = typeof( TDestination );

			CopyProperties( sourceType, destinationType, source, destination, throwOnMismatch, ignorePropertiesWithoutSetter );
		}

		/// <summary>
		/// Copies all properties of the provided source-instance onto the provided destination-instance.
		/// </summary>
		/// <param name="sourceType">Type of the source value.</param>
		/// <param name="destinationType">Type of the destination value.</param>
		/// <param name="source">Source-value to copy.</param>
		/// <param name="destination">Target-value which properties will be copied onto.</param>
		/// <param name="throwOnMismatch">
		/// If true, throws when there is a mismatch between the types being converted.
		/// If false, silently ignores property with mismatch and moves along to the next one.
		/// </param>
		/// <param name="ignorePropertiesWithoutSetter">
		/// Will ignore properties that has only a get function
		/// </param>
		public void CopyProperties( Type sourceType, Type destinationType, object source, object destination, bool throwOnMismatch, bool ignorePropertiesWithoutSetter = false )
		{
			// TODO: future bugfix/optimization
			// - add "stack"/repository for copied instances, so that circuler references will not cause isssues.
			// - will probably also save CPU-cycles by not having to copy same instance several times.
			// (instance-repository needs to follow copy-process, not SoMapper instance.)
			/*
             * Person p;
             * Contact c = p.Contact;
             * Person[] ps = c.Persons // includes p, which includes c, etc.
             */

			Type type1 = sourceType;
			Type type2 = destinationType;

			// just get the properties we can work with, nothing more.
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
			PropertyInfo[] type1PropertyInfos = type1.GetProperties( flags ).Where( p => p.CanRead ).ToArray();
			PropertyInfo[] type2PropertyInfos = type2.GetProperties( flags ).Where( p => p.CanWrite ).ToArray();

			PropertyInfo[] type2PropertyInfosCanRead = type2.GetProperties( flags ).Where( p => p.CanRead ).ToArray();

			// process properties
			foreach( var type1PropertyInfo in type1PropertyInfos )
			{
				// do null check before other analysis
				object type1PropertyValue = type1PropertyInfo.GetValue( source, null );
				if( type1PropertyValue == null )
				{
					// no point processing null-values.
					continue;
				}

				// find target prop
				var type2PropertyInfo = type2PropertyInfos.FirstOrDefault( pi => pi.Name == type1PropertyInfo.Name );
				if( type2PropertyInfo == null )
				{
					if( throwOnMismatch )
					{
						var type2PropertyInfoCanRead =
							type2PropertyInfosCanRead.FirstOrDefault( pi => pi.Name == type1PropertyInfo.Name );
						if( ignorePropertiesWithoutSetter && type2PropertyInfoCanRead != null )
							continue;
						else
						{
							string message =
								string.Format(
									"Cannot transfer property '{0}' from type '{1}' to type '{2}': Property does not exist on target type.",
									type1PropertyInfo.Name, type1.FullName, type2.FullName );
							throw new InvalidOperationException( message );
						}
					}
					else
					{
						continue;
					}
				}

				// validate match between preoprties
				Type type1PropertyType = type1PropertyInfo.PropertyType;
				Type type2PropertyType = type2PropertyInfo.PropertyType;

				Type type1ElementType, type2ElementType;

				bool type1IsEnumerable = type1PropertyType.IsEnumerable( out type1ElementType );
				bool type2IsEnumerable = type2PropertyType.IsEnumerable( out type2ElementType );

				// both must be array or not
				bool isCoherent1 = type1IsEnumerable == type2IsEnumerable;
				if( !isCoherent1 )
				{
					if( throwOnMismatch )
					{
						string message =
							string.Format(
								"Cannot transfer property '{0}' from type '{1}' to type '{2}': Cannot transfer value between enumerable and non-enumerable types.",
								type1PropertyInfo.Name, type1.FullName, type2.FullName );
						throw new InvalidOperationException( message );
					}
					else
					{
						// next property
						continue;
					}
				}

				// validate array-rank only if array.
				if( type1IsEnumerable && type1.IsArray )
				{
					int rank = type1PropertyType.GetArrayRank();
					if( rank != 1 )
					{
						// throw. always. this is not a "mismatch"
						throw new NotSupportedException( "Duck-copying multidimensional arrays is not supported." );
					}
				}

				// validate type-conversion
				Type type1ValidationType, type2ValidationType;

				if( type1IsEnumerable )
				{
					type1ValidationType = type1ElementType;
					type2ValidationType = type2ElementType;
				}
				else
				{
					type1ValidationType = type1PropertyType;
					type2ValidationType = type2PropertyType;
				}

				bool type1IsPrimitive = type1ValidationType.IsSimple();
				bool type2IsPrimitive = type2ValidationType.IsSimple();

				bool type1IsDecimal = type1ValidationType.IsDecimalNumber();
				bool type2IsDecimal = type2ValidationType.IsDecimalNumber();

				bool type1IsEnum = type1ValidationType.IsEnum;
				bool type2IsEnum = type2ValidationType.IsEnum;

				bool bothAreSamePrimitive = ( type1IsPrimitive && type2IsPrimitive && ( type1ValidationType == type2ValidationType ) );
				bool bothAreNonPrimitives = ( !type1IsPrimitive && !type2IsPrimitive );
				bool bothAreDecimals = ( type1IsDecimal && type2IsDecimal );
				bool bothAreEnum = type1IsEnum && type2IsEnum;

				bool isCoherent2 = bothAreNonPrimitives || bothAreSamePrimitive || bothAreEnum || bothAreDecimals;

				if( !isCoherent2 )
				{
					if( throwOnMismatch )
					{
						string message =
							string.Format(
								"Cannot transfer property '{0}' from type '{1}' to type '{2}': Properties are not coherent across types.",
								type1PropertyInfo.Name, type1.FullName, type2.FullName );
						throw new InvalidOperationException( message );
					}
					else
					{
						// next property
						continue;
					}
				}

				// handle everything as Enumerables to simplify codebase
				System.Collections.IEnumerable sourceEnumerable;
				if( !type1IsEnumerable )
				{
					sourceEnumerable = new[] { type1PropertyValue };
					// fake a collection-type destination to have the activator create something to insert data into.
					type2PropertyType = typeof( IEnumerable<> ).MakeGenericType( type2ValidationType );
				}
				else
				{
					sourceEnumerable = (System.Collections.IEnumerable)type1PropertyValue;
				}

				// safely allocate target collection
				System.Collections.IEnumerable destinationEnumerable = (System.Collections.IEnumerable)CreateDefaultEntity( type2PropertyType );
				EnumerableWriter writer = EnumerableWriter.CreateFor( type2PropertyType );

				// do set-conversion
				foreach( object sourceValue in sourceEnumerable )
				{
					object destinationValue;

					if( type1IsPrimitive && bothAreSamePrimitive )
					{
						// primitive types can be copied directly.
						destinationValue = sourceValue;
					}
					else if( type1IsPrimitive && bothAreDecimals )
					{
						destinationValue = CopyDecimal( sourceValue, type2ValidationType );
					}
					else if( type1IsPrimitive )
					{
						throw new InvalidOperationException( "Unsupported property-copy between non-compatible primitive types." );
					}
					else if( type1ValidationType == type2ValidationType
						&& type1ValidationType.IsInterface )
					{
						// we cannot reliably create new instances of these, but we can safely copy instance.
						destinationValue = sourceValue;
					}
					else if( bothAreEnum )
					{
						destinationValue = Enum.ToObject( type2ValidationType, (int)sourceValue );
					}
					else
					{
						// complex types needs to be converted by doing a recursive self invocation.

						// NOTE: we specify validation-types to ensure it works for both enumerables and non-enumerables.
						destinationValue = CreateDefaultEntity( type2ValidationType );
						CopyProperties( type1ValidationType, type2ValidationType, sourceValue, destinationValue, throwOnMismatch );
					}

					destinationEnumerable = writer.Write( destinationEnumerable, destinationValue );
				}

				object result;
				if( type1IsEnumerable )
				{
					result = destinationEnumerable;
				}
				else
				{
					// we need to unwrap our value from the enumerator
					System.Collections.IEnumerator enumerator = destinationEnumerable.GetEnumerator();
					if( enumerator.MoveNext() )
					{
						result = enumerator.Current;
					}
					else
					{
						// this should never happen. so we throw.
						throw new Exception( "Internal error copying single value between types." );
					}
				}

				// all that remains is writing copy.
				type2PropertyInfo.SetValue( destination, result, null );
			}
		}

		private object CopyEnum( Type destinationType, object source, bool throwOnMismatch )
		{
			object destination = CreateDefaultEntity( destinationType );

			try
			{
				string sourceString = source.ToString();
				destination = Enum.Parse( destinationType, sourceString );
			}
			catch( Exception )
			{
				if( throwOnMismatch )
					throw;
			}

			return destination;
		}

		private object CopyDecimal( object sourceValue, Type targetType )
		{
			object destinationValue = null;
			string sourceValueText = sourceValue.ToString();

			if( targetType == typeof( decimal ) )
			{
				destinationValue = decimal.Parse( sourceValueText );
			}
			else if( targetType == typeof( float ) )
			{
				destinationValue = float.Parse( sourceValueText );
			}
			else if( targetType == typeof( double ) )
			{
				destinationValue = double.Parse( sourceValueText );
			}
			else
			{
				string msg = string.Format( "Unable to create decimal object of type {0} from value '{1}'.",
										   targetType.FullName, sourceValue );
				throw new InvalidOperationException( msg );
			}

			return destinationValue;
		}

		private static object CreateDefaultEntity( Type type )
		{
			object result = null;

			Type itemType;
			if( type.IsArray )
			{
				// don't double-create the array (ie create T[] for T[], not T[][])
				itemType = type.GetElementType();
				result = Array.CreateInstance( itemType, 0 );
			}
			else if( type.IsInterface && type.IsGenericEnumerable( out itemType ) )
			{
				// activator cannot create these. do some cover-up magic.
				// A List<T> should cover all enumerables and generics and woo.
				Type listType = typeof( List<> );
				Type typedListType = listType.MakeGenericType( itemType );
				ConstructorInfo typedListConstructor = typedListType.GetConstructor( new Type[] { } );
				result = typedListConstructor.Invoke( new object[] { } );
			}
			else if( type.IsInterface && type.IsNongenericEnumerable() )
			{
				// activator cannot create these. do some cover-up magic.
				// A ArrayList should cover all non-gengeric enumerables.
				result = new System.Collections.ArrayList();
			}
			else
			{
				result = Activator.CreateInstance( type );
			}
			return result;
		}
	}
}
