using SuperOffice.CRM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperOffice.DevNet.Online.Provisioning
{
    public class ListHelper
    {
        private MDOListItem[] _allLists;

        public MDOListItem[] GetAllLists()
        {
            if (_allLists != null)
                return _allLists;

            using (MDOAgent mdoAgent = new MDOAgent())
            {
                _allLists = mdoAgent.GetList("Lists", true, string.Empty, false);
            }

            return _allLists;
        }

        public MDOListItem GetListDefinition(string listName)
        {
            if (_allLists == null)
                GetAllLists();
            return _allLists.FirstOrDefault(l => l.Type.Equals(listName, StringComparison.InvariantCultureIgnoreCase));
        }

        public MDOListItem[] GetSimpleList(string listName)
        {
            using (MDOAgent mdoAgent = new MDOAgent())
            {
                return mdoAgent.GetSimpleList(listName);
            }
        }

        public ListEntity GetListEntity(string listName)
        {
            ListEntity listEntity = null;

            var saleSourceListDefinition = GetListDefinition(Constants.Lists.SaleSource);

            if (saleSourceListDefinition != null)
            {
                using (ListAgent listAgent = new ListAgent())
                {
                    listEntity = listAgent.GetListEntity(saleSourceListDefinition.Id);
                }
            }

            return listEntity;
        }

        public ListItemEntity GetListEntityItem(string listName, int listItem)
        {
            ListItemEntity listEntityItem = null;

            var listDefinition = GetListDefinition(listName);

            if (listDefinition != null)
            {
                using (ListAgent listAgent = new ListAgent())
                {
                    listEntityItem = listAgent.GetFromListDefinition(listItem, listDefinition.Id);
                }
            }

            return listEntityItem;
        }

        public ListItemEntity GetListEntityItem(string listName, string listItem)
        {
            ListItemEntity listItemEntity = null;

            var listEntityItem = GetListDefinition(listName);

            if (listEntityItem != null)
            {
                var listItems = GetSimpleList(listName);

                if (listItems != null)
                {
                    var foundListItem = listItems.FirstOrDefault(l => l.Name.Equals(listItem, StringComparison.InvariantCultureIgnoreCase));

                    if (foundListItem != null)
                    {
                        using (ListAgent listAgent = new ListAgent())
                        {
                            listItemEntity = listAgent.GetFromListDefinition(listEntityItem.Id, foundListItem.Id);
                        }
                    }

                }
            }
            return listItemEntity;
        }

        public ListItemEntity CreateListItem(string listName, string listItemName, string listItemTooltip)
        {
            ListItemEntity listItem = null;

            //Get the root list definition
            var saleSourceListDefinition = GetListDefinition(listName);

            if (saleSourceListDefinition != null)
            {
                using (ListAgent listAgent = new ListAgent())
                {
                    //Create the listItem
                    listItem = listAgent.CreateDefaultListItemEntity();

                    listItem.UdListDefinitionId = saleSourceListDefinition.Id;
                    listItem.Name               = listItemName;
                    listItem.Tooltip            = listItemTooltip;

                    listItem = listAgent.SaveListItemEntity(listItem);
                }
            }

            return listItem;
        }

        public ListItemEntity CreateSaleSourceListItem(string name, string tooltip)
        {
            return CreateListItem(Constants.Lists.SaleSource, "Transmissions to Outer Space", "Beaming signals to other worlds to generate galactic sales!");
        }

        /// <summary>
        /// Creates a User-Defined List
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="listTooltip"></param>
        /// <returns></returns>
        public ListEntity CreateList(string listName, string listTooltip)
        {
            ListEntity listEntity = null;

            using (ListAgent listAgent = new ListAgent())
            {
                listEntity         = listAgent.CreateDefaultListEntity();
                listEntity.Name    = listName;
                listEntity.Tooltip = listTooltip;
                listEntity         = listAgent.SaveListEntity(listEntity);
            }

            return listEntity;
        }

        private static bool CanBeVisible(ListEntity listEntity)
        {
            switch (listEntity.ListType)
            {
                case Constants.Lists.SaleStage:
                case Constants.Lists.SaleTypeCategory:
                case Constants.Lists.ProjectStatus:
                case Constants.Lists.ProjectTypeStatus:
                case Constants.Lists.ChatProtocols:
                case Constants.Lists.QuoteDeliveryTerms:
                case Constants.Lists.QuoteDeliveryType:
                case Constants.Lists.QuotePaymentTerms:
                case Constants.Lists.QuotePaymentType:
                case Constants.Lists.ProductCategory:
                case Constants.Lists.ProductFamily:
                case Constants.Lists.ProductType:
                case Constants.Lists.ProductSubscriptionUnit:
                case Constants.Lists.ProductPriceUnit:
                    return false;
                default:
                    return listEntity.IsMDOList;
            }
        }
    }

    public static class Constants
    {
        public static class Lists
        {
            //                  Name                      = Type
            public const string ChatProtocols             = "chat";
            public const string DocumentTemplate          = "doctmpl";
            public const string GeneralResource           = "associate";
            public const string CompanyCategory           = "category";
            public const string CompanyBusiness           = "business";
            public const string GeneralUserGroup          = "usergroup";
            public const string GeneralRelation           = "relationdefinition";
            public const string FollowUpType              = "task";
            public const string FollowUpPriority          = "priority";
            public const string CompanyInterest           = "contint";
            public const string ContactInterest           = "persint";
            public const string ContactPosition           = "perspos";
            public const string ContactMrMrs              = "mrmrs";
            public const string ProjectTypeStatus         = "projtype";
            public const string ProjectStatus             = "projstatus";
            public const string ProjectMemberFunction     = "pmembtype";
            public const string SelectionType             = "searchcat";
            public const string SaleSource                = "source";
            public const string SaleReasonLost            = "reason";
            public const string SaleCompetitor            = "comptr";
            public const string SaleStage                 = "prob";
            public const string SaleCredited              = "credited";
            public const string GeneralCurrency           = "currency";
            public const string GUIApplication            = "extapp";
            public const string GeneralCountry            = "country";
            public const string FollowUpIntention         = "intent";
            public const string ContactAcademicTitle      = "salutation";
            public const string SaleAmountClass           = "amountclass";
            public const string GUIWebPanel               = "webpanel";
            public const string InvitationRejectionReason = "rejectreason";
            public const string SaleTypeStagesQuote       = "saletype";
            public const string SaleReasonStalled         = "reasonstalled";
            public const string SaleReasonSold            = "reasonsold";
            public const string SaleTypeCategory          = "saletypecat";
            public const string SaleStakeholderRole       = "stakeholderrole";
            public const string ProductType               = "producttype";
            public const string ProductCategory           = "productcategory";
            public const string ProductFamily             = "productfamily";
            public const string QuotePaymentTerms         = "paymentterms";
            public const string QuotePaymentType          = "paymenttype";
            public const string QuoteDeliveryTerms        = "deliveryterms";
            public const string QuoteDeliveryType         = "deliverytype";
            public const string ProductPriceUnit          = "priceunit";
            public const string ProductSubscriptionUnit   = "subscriptionunit";
        }
    }
}
