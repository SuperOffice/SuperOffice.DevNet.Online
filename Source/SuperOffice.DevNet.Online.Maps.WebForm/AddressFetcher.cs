using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperOffice.CRM.Documents;
using SuperOffice.CRM.Globalization;
using SuperOffice.DevNet.Online.Login.Extensions;
using SuperOffice.Services75;
using SuperOffice.Util;

namespace SuperOffice.DevNet.Online.Maps.WebForm

{
	public interface IAddressFetcher
	{
		string GetScript( int selectionId );
	};

	abstract public class AddressFetcher : IAddressFetcher
	{
		protected string Archive = "ContactSelection";
		protected string Keyid = "selectionId";

		protected string Contactid = "contact/contactId";
		protected string Name = "contact/name";
		protected string Department = "contact/department";
		protected string Namedepartment = "contact/nameDepartment";
		protected string StreetaddressLine1 = "contact/streetAddress/line1";
		protected string StreetaddressLine2 = "contact/streetAddress/line2";
		protected string StreetaddressLine3 = "contact/streetAddress/line3";
		protected string StreetaddressCounty = "contact/streetAddress/county";
		protected string StreetaddressCity = "contact/streetAddress/city";
		protected string StreetaddressZip = "contact/streetAddress/zip";
		protected string PostalAddressLine1 = "contact/postAddress/line1";
		protected string PostalAddressLine2 = "contact/postAddress/line2";
		protected string PostalAddressLine3 = "contact/postAddress/line3";
		protected string PostalAddressCounty = "contact/postAddress/county";
		protected string PostalAddressCity = "contact/postAddress/city";
		protected string PostalAddressZip = "contact/postAddress/zip";
		protected string Country = "contact/country";

		public string GetScript( int mainId )
		{
			using (var newArcAgt = new ArchiveAgent())
			{
				//Setting the Parameters
				var archiveColumns = new string[]
				{
					Keyid, Contactid, Name, Department, Namedepartment
					, StreetaddressLine1, StreetaddressLine2, StreetaddressLine3
					, StreetaddressCounty, StreetaddressCity, StreetaddressZip
					, PostalAddressLine1, PostalAddressLine2, PostalAddressLine3
					, PostalAddressCounty, PostalAddressCity, PostalAddressZip
					, Country
				};
				//Parameter - restriction - Archive restrictions
				var archiveRest = GetArchiveRestrictionInfos(mainId);

				//Parameter - page - Page number, page 0 is the first page
				int page = 0;

				//Parameter - pageSize – Number of records displayed per page
				const int pageSize = 10;

				// Get a page of results for an archive list, explicitly specifying 
				// the restrictions, orderby and chosen columns
				ArchiveListItem[] arcLstItm = null;
				var script = "function initializeMarkers() {\n";

				var placedCompanies = new HashSet<int>();	// Because we only want to register a place once.

				do
				{

					arcLstItm = newArcAgt.GetArchiveListByColumns(
						Archive, archiveColumns, new ArchiveOrderByInfo[0],
						archiveRest, null, page++, pageSize);

					foreach (ArchiveListItem archiveRow in arcLstItm)
					{
						int contactId = CultureDataFormatter.ParseEncodedInt(archiveRow.ColumnData[Contactid].DisplayValue);
						if (!placedCompanies.Contains(contactId))
						{
							placedCompanies.Add( contactId );
							string address = string.Empty;

							address = address.Add(archiveRow.ColumnData[StreetaddressLine1].DisplayValue, ", ");
							address = address.Add(archiveRow.ColumnData[StreetaddressLine2].DisplayValue, ", ");
							address = address.Add(archiveRow.ColumnData[StreetaddressLine3].DisplayValue, ", ");
							address = address.Add( archiveRow.ColumnData[ StreetaddressZip ].DisplayValue, ", " );
							address = address.Add( archiveRow.ColumnData[ StreetaddressCity ].DisplayValue, ", " );

							if (address.IsNullOrEmpty())
							{
								address = address.Add( archiveRow.ColumnData[ PostalAddressLine1 ].DisplayValue, ", " );
								address = address.Add( archiveRow.ColumnData[ PostalAddressLine2 ].DisplayValue, ", " );
								address = address.Add( archiveRow.ColumnData[ PostalAddressLine3 ].DisplayValue, ", " );
								address = address.Add( archiveRow.ColumnData[ PostalAddressZip ].DisplayValue, ", " );
								address = address.Add( archiveRow.ColumnData[ PostalAddressCity ].DisplayValue, ", " );
							}

							address = address.Add(archiveRow.ColumnData[Country].DisplayValue, ", ");
							string lat, lng;
							GoogleMaps.GeocodeAddress(address,
								out lat,
								out lng
								);

							var tooltip = System.Web.HttpUtility.JavaScriptStringEncode( archiveRow.ColumnData[ Namedepartment ].DisplayValue );

							script += string.Format("AddMarker(map, '{0}', {1}, {2} );\n",tooltip,lat,lng);
						}
					}
				} while( arcLstItm.Length != 0 );

				script += "}";

				return script;
			}
		}

		protected virtual ArchiveRestrictionInfo[] GetArchiveRestrictionInfos(int mainId)
		{
			var archiveRest = new ArchiveRestrictionInfo[1];
			archiveRest[0] = new ArchiveRestrictionInfo
			{
				Name = Keyid,
				Operator = "=",
				Values = new string[] {mainId.ToString()},
				IsActive = true
			};
			return archiveRest;
		}
	}

	public class AddressFetcherFromContactSelection : AddressFetcher
	{
		public AddressFetcherFromContactSelection()
		{
			Archive = "ContactSelection";
			Keyid = "selectionId";

			StreetaddressLine1 = "streetAddress/line1";
			StreetaddressLine2 = "streetAddress/line2";
			StreetaddressLine3 = "streetAddress/line3";
			StreetaddressCounty = "streetAddress/county";
			StreetaddressCity = "streetAddress/city";
			StreetaddressZip = "streetAddress/zip";
			PostalAddressLine1 = "postAddress/line1";
			PostalAddressLine2 = "postAddress/line2";
			PostalAddressLine3 = "postAddress/line3";
			PostalAddressCounty = "postAddress/county";
			PostalAddressCity = "postAddress/city";
			PostalAddressZip = "postAddress/zip";
			Country = "country";
			Keyid = "selectionId";
			Contactid = "contactId";
			Name = "name";
			Department = "department";
			Namedepartment = "nameDepartment";
		}
	}

	public class AddressFetcherFromOtherSelections : AddressFetcher
	{
		protected string GetSelectionArchiveFromType(int type)
		{
			switch (type)
			{
					case 5:
						return "ContactSelection";
					case 9:
						return "AppointmentShadowSelection";
					case 10:
						return "DocumentShadowSelection";
					case 11:
						return "ProjectShadowSelection";
					case 13:
						return "SaleShadowSelection";
					default:
						return string.Empty;
			}
		}

		public AddressFetcherFromOtherSelections( int selectionTableType)
		{
			Archive = GetSelectionArchiveFromType( selectionTableType );
			Keyid = "selectionId";

			//streetaddressLine1 = "contact/streetAddress/line1";
			//streetaddressLine2 = "contact/streetAddress/line2";
			//streetaddressLine3 = "contact/streetAddress/line3";
			//streetaddressCounty = "contact/streetAddress/county";
			//streetaddressCity = "contact/streetAddress/city";
			//streetaddressZip = "contact/streetAddress/zip";
			//country = "contact/country";
			//keyid = "selectionId";
			//contactid = "contactId";
			//name = "contact/name";
			//department = "contact/department";
			//namedepartment = "contact/nameDepartment";
		}
	}
	public class AddressFetcherFromProjectMember : AddressFetcher
	{
		public AddressFetcherFromProjectMember()
		{
			Archive = "ProjectMember";
			Keyid = "projectId";
		}
	}

	public class AddressFetcherFromDiary : AddressFetcher
	{
		public AddressFetcherFromDiary()
		{
			Archive = "DiaryAppointment";
			Keyid = "associateId";
		}



		/// <summary>
		/// Should only show not done appointments from today (actually, current day)
		/// </summary>
		/// <param name="mainId"></param>
		/// <returns></returns>
		protected override ArchiveRestrictionInfo[] GetArchiveRestrictionInfos(int mainId)
		{
			var archiveRest = new ArchiveRestrictionInfo[]
			{
				new ArchiveRestrictionInfo
				{
					Name = Keyid,
					Operator = "=",
					Values = new string[]
					{
						CultureDataFormatter.EncodeInt(mainId)
					},
					IsActive = true,
					InterOperator = InterRestrictionOperator.And
				}, 
				new ArchiveRestrictionInfo
				{
					Name = "date",
					Operator = "today",
					Values = new string[] { "" },
					//Values = new string[]
					//{
					//	CultureDataFormatter.EncodeDateTime( DateTime.Today ),
					//	CultureDataFormatter.EncodeDateTime( DateTime.Today.AddDays( 2 )/*.AddDays( 1 ).AddSeconds( -1 )*/ ),
					//},
					IsActive = true,
					InterOperator = InterRestrictionOperator.And
				},
				new ArchiveRestrictionInfo
				{
					Name = "completed",
					Operator = "=",
					Values = new string[] { CultureDataFormatter.EncodeInt( 0 ) },
					IsActive = true
				}
			};
			return archiveRest;
		}
	}

}