using System;
using System.Web;
using SuperOffice;
using SuperOffice.Factory;
using SuperOffice.License;
using SuperOffice.Security.Principal;

namespace SuperOffice.DevNet.Online.Login
{
	/// <summary>
	/// Context storage provider based on the IIS HttpContext
	/// </summary>
	/// <remarks>
	/// In principle, this class will store an incoming SoContextContainer in a context variable
	/// connected to the current session. But there are some unpleasant details, as follows:
	/// <para/>
	/// NetServer has async operations, where the threads do not come from the IIS thread pool. This basically
	/// applies to the NetServer, not 6.web, async operations and so is a problem when running with NetServer in-process
	/// with the web server - or when providing Web Services, which means there's an IIS wrapped around NetServer anyway.
	/// Such threads to not have a valid session and so cannot access their context info through the HttpContext.Session.
	/// <br/>
	/// For such threads we have a fallback by using a threadstatic variable to store the context in. This variable
	/// is Set and Get'ed if and only if a valid HttpContext and session are not present.
	/// <para/>
	/// During execution of a request, IIS may switch threads under our feet - so setting the ThreadStatic fallback
	/// variable when a normal HttpContext is available (on the main thread serving the request) may be a waste and
	/// is not guaranteed to help any future async threads.
	/// <para/>
	/// The transfer of the context is done by the ThreadManager deep inside NetServer, when a new thread is spawned. The new thread
	/// receives a clone of the context, to guard against logouts coming from the root or other threads.
	/// <para/>
	/// During request processing in IIS, a number of events are fired. On the earliest of these, the HttpContext is not
	/// available. Later on the HttpContext is available, but there may still be thread switches ahead. Saving the context
	/// in the HttpContext makes us less vulnerable to such switches, at the cost of having to "wait" until it is
	/// established.
	/// <para/>
	/// To sum it up, the point is to find a safe place to store the SoContext. HttpContext is such a place,
	/// but is only established after the first few events have fired in IIS, and it's not available to threads
	/// that IIS does not know about; on the other hand, threadstatic variables are stable, but bound to a thread and at
	/// the mercy of IIS' behind-the-scenes thread switching,
	/// </remarks>
	[SoContextProvider( "PartnerHttpContext" )]
	public class HttpContextProvider : ISoContextProvider
	{
		/// <summary>
		/// Context for worker threads
		/// </summary>
		/// <remarks>
		/// The context provider will be asked to hold contexts from the processing of web 
		/// requests as well as threads being executed asyncroniously.  The <see cref="HttpContext"/> 
		/// (i.e. web session handling) is not available for these latter worker threads.  
		/// Hence; we are storing context wor worker threads in a <see cref="ThreadStatic"/> variable.
		/// </remarks>
		[ThreadStatic]
		static SoContextContainer _workerThreadContext = null;



		const string Provider = "SoHttpContextProvider";

		public HttpContextProvider()
		{
		}

		/// <summary>
		/// Get the context.
		/// </summary>
		/// <returns>Context obtained.</returns>
		/// <remarks>
		/// We first try to obtain the current <see cref="HttpContext"/> from ASP.Net.  We simply return the 
		/// stored <see cref="SoContextContainer"/> in the <see cref="HttpContext"/> if it is available, otherwise we just 
		/// fall back to returning the <see cref="SoContextContainer"/> helt in the thread-static member 
		/// variable designated for worker threads, assuming it is one.  
		/// <para>
		/// This relyes on the assomption that we are not trying to do anything with NetServer 
		/// before the <see cref="HttpContext"/> has been made available by ASP.Net.  This is something that 
		/// needs to be beared in mind when working with ASP.Net Http Modules.
		/// </para>
		/// </remarks>
		public SoContextContainer GetCurrenContext()
		{
            HttpContext currentContext = HttpContext.Current;
			if( currentContext != null && currentContext.Session != null )
				return (SoContextContainer)currentContext.Session[Provider];
			else
				return _workerThreadContext;
		}

		/// <summary>
		/// Set the context
		/// </summary>
		/// <remarks>
		/// See the remarks on <see cref="GetCurrenContext"/>.
		/// </remarks>
		/// <param name="context">Context to set.</param>
		public void SetCurrentContext( SoContextContainer context )
		{
			HttpContext currentContext = HttpContext.Current;
			bool nullingContext = context == null || string.IsNullOrEmpty(context.ToString());

			if( currentContext != null && currentContext.Session != null )
			{
				currentContext.Session[Provider] = context;
			}
			else
			{
				_workerThreadContext = context;
			}
		}

		/// <summary>
		/// Determine if there is a valid context stored in ASP.Net <see cref="HttpContext"/>
		/// </summary>
		/// <returns></returns>
		public static bool IsValid()
		{
			HttpContext currentContext = HttpContext.Current;
            if (currentContext != null && currentContext.Session != null)
            {
                var contextContainer = currentContext.Session[Provider] as SoContextContainer;


                // If this is a Anonymous user, we don't have a contextContainer since we actually don't need to store that data.
                if (contextContainer == null &&
                    SoContext.CurrentPrincipal != null &&
                    SoContext.CurrentPrincipal.UserType == UserType.AnonymousAssociate)
                    return true;                    
                
                return SuperOfficeAuthHelper.IsAuthenticatedWithNetServer();
            }
            else
                return false;
		}

	}
}
