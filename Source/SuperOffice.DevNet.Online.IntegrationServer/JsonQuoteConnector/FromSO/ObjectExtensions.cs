using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonQuoteConnector;

public static class ObjectExtensions
	{
		/// <summary>
		/// Copies all public writable &amp; public properties from the source-instance to the provided
		/// target instance.
		/// This operation is applied recursively for all non-primitive types, meaning that the two instances
		/// should be 100% independent and unable to mutate each others members.
		/// <remarks>
		/// Fields and private members are not copied or assigned.
		/// If the type depends on private state to function, the copy may not work as expected.
		/// </remarks>
		/// </summary>
		/// <typeparam name="T">Type of object whose properties we want to copy.</typeparam>
		/// <param name="source">Source instance</param>
		/// <param name="destination">Targetinstance</param>
		public static void AssignByReflection<T>( this T source, T destination )
			// not actually a requirement, but less autocomplete polution for simple types where this
			// functionality is NEVER needed.
			where T : new()
		{
			// lets be defensive about accuracy
			const bool throwOnMismatch = true;

			// but we shall ignore properties with only get'ers.
			const bool ignoreWithoutSetters = true;

			SoMapper.Instance.CopyProperties( source, destination, throwOnMismatch, ignoreWithoutSetters );
		}

		/// <summary>
		/// Creates a copy of the provided object-graph by copying all public properties of the provided
		/// instance.
		/// This operation is applied recursively for all non-primitive types, meaning that the resulting instance
		/// should be 100% independent from the instance provided and that the two instances should be unable to
		/// mutate each others members.
		/// <remarks>
		/// Fields and private members are not copied or assigned.
		/// If the type depends on private state to function, the copy may not work as expected.
		/// </remarks>
		/// </summary>
		/// <typeparam name="T">Type of object whose properties we want to copy.</typeparam>
		/// <param name="source">Source instance</param>
		/// <returns>A 100% independent copy.</returns>
		public static T GraphCopy<T>( this T source )
			where T : new()
		{
			T result = SoMapper.Instance.Clone( source );
			return result;
		}
	}
