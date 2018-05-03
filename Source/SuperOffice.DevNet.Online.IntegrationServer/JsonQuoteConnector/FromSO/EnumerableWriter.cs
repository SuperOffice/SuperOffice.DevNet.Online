using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SC = System.Collections;
using SCG = System.Collections.Generic;

	/// <summary>
	/// Class which provides the ability to write to a provided enumerable of many kinds.
	/// Instances should be created through <see cref="EnumerableWriter.CreateFor"/>.
	/// </summary>
	public abstract class EnumerableWriter
	{
		/// <summary>
		/// Creates an EnumerableWriter for the specified type and attempts to detect the type it enumerates over.
		/// </summary>
		/// <param name="enumerableType">Type for the EnumuerableWriter to work with. Must be a recognized Enumerable, or method throws.</param>
		/// <returns>Will always return an instance of <see cref="EnumerableWriter"/></returns>
		public static EnumerableWriter CreateFor( Type enumerableType )
		{
			Type itemType;
			bool isEnumerable = enumerableType.IsEnumerable( out itemType );
			if( false == isEnumerable )
			{
				throw new ArgumentException( "Unable to produce EnumerableWriter for non-enumerable type " + enumerableType.FullName );
			}

			MethodInfo methodInfo = typeof( EnumerableWriter ).GetMethod( "CreateFor", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof( Type ) }, null );
			MethodInfo genericInvocatable = methodInfo.MakeGenericMethod( itemType );

			object[] parameters = new object[] { enumerableType };
			object result = genericInvocatable.Invoke( null, parameters );
			return (EnumerableWriter)result;
		}

		/// <summary>
		/// Creates an EnumerableWriter for the specified type.
		/// </summary>
		/// <typeparam name="TItem">The type of item which the enumerable provides.</typeparam>
		/// <param name="enumerableType">Type for the EnumuerableWriter to work with. Must be a recognized Enumerable, or method throws.</param>
		/// <returns>Will always return an instance of <see cref="EnumerableWriter"/></returns>
		private static EnumerableWriter CreateFor<TItem>( Type enumerableType )
		{
			Type ignored = null;

			EnumerableWriter result = null;

			if( enumerableType.IsArray )
			{
				result = new ArrayWriter<TItem>();
			}
			else if( enumerableType.IsGenericIList( out ignored ) )
			{
				result = new GenericListWriter<TItem>();
			}
			else if( enumerableType.IsNongenericIList() )
			{
				result = new NongenericListWriter();
			}
			else if( enumerableType.IsGenericEnumerable( out ignored ) )
			{
				result = new GenericEnumerableRewriter<TItem>();
			}
			else if( enumerableType.IsNongenericEnumerable() )
			{
				result = new NongenericEnumerableRewriter();
			}
			else
			{
				throw new ArgumentException( "Unable to produce EnumerableWriter for type " + enumerableType.FullName );
			}

			return result;
		}


		/// <summary>
		/// Takes an instance of an enumerable and adds data to it.
		/// The returned value may be the same instance modified, or it may be a new intance.
		/// </summary>
		/// <param name="enumerableInstance">Enumerable instance to modify</param>
		/// <param name="value">Value to add.</param>
		/// <returns>A enumerable instance guaranteed to contain the provided value.</returns>
		public abstract SC.IEnumerable Write( SC.IEnumerable enumerableInstance, object value );


		private class ArrayWriter<T> : EnumerableWriter
		{
			public override SC.IEnumerable Write( SC.IEnumerable enumerableInstance, object value )
			{
				T[] array = (T[])enumerableInstance;
				int elements = array.Length;
				Array.Resize( ref array, elements + 1 );
				array[ elements ] = (T)value;
				return array;
			}
		}

		private class GenericListWriter<T> : EnumerableWriter
		{
			public override SC.IEnumerable Write( SC.IEnumerable enumerableInstance, object value )
			{
				SCG.IList<T> list = (SCG.IList<T>)enumerableInstance;
				T typedValue = (T)value;
				list.Add( typedValue );
				return list;
			}
		}

		private class NongenericListWriter : EnumerableWriter
		{
			public override SC.IEnumerable Write( SC.IEnumerable enumerableInstance, object value )
			{
				SC.IList list = (SC.IList)enumerableInstance;
				list.Add( value );
				return list;
			}
		}

		private class GenericEnumerableRewriter<T> : EnumerableWriter
		{
			public override SC.IEnumerable Write( SC.IEnumerable enumerableInstance, object value )
			{
				// pre-cast value to force type-check early on. writing invalid values SHOULD fail.
				T typedValue = (T)value;
				// make sure our non-generic generated enumerable originates from a generic enumerable.
				return (SC.IEnumerable)WriteTyped( enumerableInstance, typedValue );
			}

			private SCG.IEnumerable<T> WriteTyped( SC.IEnumerable enumerableInstance, T value )
			{
				foreach( T item in enumerableInstance )
				{
					yield return item;
				}
				yield return value;
			}
		}

		private class NongenericEnumerableRewriter : EnumerableWriter
		{
			public override SC.IEnumerable Write( SC.IEnumerable enumerableInstance, object value )
			{
				foreach( object item in enumerableInstance )
				{
					yield return item;
				}
				yield return value;
			}
		}
	}
