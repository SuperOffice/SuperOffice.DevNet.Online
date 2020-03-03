using System;
using System.Collections.Generic;
using SuperOffice.CRM.Services;

namespace SuperOffice.DevNet.Online.Provisioning
{
	/// <summary>
	/// Helps store and clean up foreign keys for partners
	/// 
	/// NB! The foreign key system has a three dimensional structure, where the dimensions are 
	/// named "App", "Device" and "Key". 
	/// To store information we only need two, so to keep this a little simple 
	/// implementation-wise, we did something a little confusing: 
	/// We have one "App" for all Partners, the device level is named using the partner app name 
	/// and the key is the name. This is hidden in the implementation details, of course.
	/// </summary>
	public class ForeignKeyHelper
	{
		readonly ForeignSystemAgent fsa = new ForeignSystemAgent();

		private const string ForeignKeyAppName = "Partner.Apps";
		private const string TableName = "extapp";
		private const string TableFakeValue = "PartnerApp";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="keyName"></param>
		/// <param name="recordId"></param>
		public void AddKeyForId(string appName, string keyName, int recordId)
		{
			var app = GetApp();

			var key = fsa.GetKeyByValue( ForeignKeyAppName, appName, keyName, TableFakeValue, TableName );
			if( key == null )
			{
				key = new ForeignKey
				{
					Key = keyName,
					RecordId = recordId,
					TableName = TableName,
					Value = TableFakeValue,
				};

				key = fsa.SaveForeignKey( key, ForeignKeyAppName, appName, appName );
			}
		}

		public bool DoesKeyExist(string appName, string keyName)
		{
			var key = GetKey(appName, keyName);

			return key != null;
		}

		public ForeignKey GetKey(string appName, string key)
		{
			return fsa.GetKeyByValue(ForeignKeyAppName, appName, key, TableFakeValue, TableName);
		}

		private ForeignAppEntity GetApp()
		{
			var app = fsa.GetAppByName(ForeignKeyAppName);
			if (app.ForeignAppId == 0)
			{
				app = fsa.CreateDefaultForeignAppEntity();
				app.Name = ForeignKeyAppName;
				app = fsa.SaveForeignAppEntity(app);
			}
			return app;
		}

		/// <summary>
		/// Deletes the key and cleans up the structure if it can:
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="deviceName"></param>
		/// <param name="keyName"></param>
		/// <returns>The vaule of the key</returns>
		private int DeleteKey( string appName, string deviceName, string keyName, string value, string tableName )
		{
			var key = fsa.GetKeyByValue( appName, deviceName, keyName, value, tableName );
			if( key != null )
				fsa.DeleteForeignKey( key, appName, deviceName, "", tableName, key.RecordId );

			// Cleaning up:
			// Is there other keys on this device?
			var devs = fsa.GetDeviceKeys( appName, deviceName );
			if( devs == null || devs.Length == 0 )
			{
				// delete the device too:
				var dev = fsa.GetDeviceByName( appName, deviceName );
				if (dev != null)
					fsa.DeleteForeignDevice(dev, appName);
			}

			var app = fsa.GetAppByName( appName );
			if( app.ForeignAppId != 0 )
			{
				if( app.Devices == null || app.Devices.Length == 0 )
				{
					// delete the app too:
					fsa.DeleteForeignAppEntity( app.ForeignAppId );
				}
			}

			return ( key != null ? key.RecordId : 0 );
		}


		/// <summary>
		/// When developing, sometimes you just want to clean out your foreignkeys:
		/// </summary>
		/// <param name="appName"></param>
		private void DeleteAppAndAll( string appName )
		{
			var fsa = new ForeignSystemAgent();

			var app = fsa.GetAppByName( appName );
			if( app.ForeignAppId != 0 )
			{
				if( app.Devices != null )
				{
					foreach( var dev in app.Devices )
					{
						var keys = fsa.GetDeviceKeys( appName, dev.Name );
						if( keys != null )
							foreach( var key in keys )
								fsa.DeleteForeignKey( key, appName, dev.Name, dev.DeviceIdentifier, string.Empty, 0 );

						// delete the device too:
						fsa.DeleteForeignDevice( dev, appName );
					}
				}
				// delete the app too:
				fsa.DeleteForeignAppEntity( app.ForeignAppId );
			}
		}

		/// <summary>
		/// When developing, sometimes you just want to clean out your foreignkeys:
		/// </summary>
		/// <param name="appName"></param>
		public void DeleteAllKeysForPartnerApp( string appName )
		{
			var partnerApp = fsa.GetDeviceByName(ForeignKeyAppName, appName);
			if( partnerApp != null )
			{
				var keys = fsa.GetDeviceKeys( ForeignKeyAppName, partnerApp.Name );
				if( keys != null )
					foreach( var key in keys )
						fsa.DeleteForeignKey( key, ForeignKeyAppName, partnerApp.Name, partnerApp.DeviceIdentifier, key.TableName, key.RecordId );

				// delete the device too:
				fsa.DeleteForeignDevice( partnerApp, ForeignKeyAppName );
			}
		}

	}
}
