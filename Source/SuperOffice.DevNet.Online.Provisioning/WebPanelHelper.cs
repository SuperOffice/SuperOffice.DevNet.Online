using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SuperOffice;
using SuperOffice.CRM.Services;
using SuperOffice.Data;

namespace SuperOffice.DevNet.Online.Provisioning
{
    public class WebPanelHelper
    {
        private const string _webPanelDescription = "Web panel created by partner application.";
        private const string _namePrefixFormat = "{0}Panel";
        private const string _windowPrefixFormat = "{0}Window";
        private const string _buttonPrefixFormat = "{0}Button";
        private const string _buttonWindowFormat = "{0}ButtonWindow";
        private const string _locationPrefixFormat = "soprotocol:browser.{0}";

        public WebPanelHelper()
        {

        }

        /// <summary>
        /// Create a web panel in the logged in users' installation of SuperOffice.
        /// </summary>
        /// <param name="name">The string that uniquely identifies the web panel. This name will be stripped of spacing and special characters, and appended with "Panel", so it becomes namePanel.</param>
        /// <param name="appearLocation">Where in SuperOffice the panel will be shown</param>
        /// <param name="url">The url to show in the web panel</param>
        /// <param name="windowName">Window Name </param>
        /// <param name="urlEncoding"></param>
        public WebPanelEntity CreateWebPanel(
            string name, 
            string url, 
            Navigation appearLocation, 
            string windowName = null,
            UrlEncoding urlEncoding = UrlEncoding.None,
            bool showInAddressBar = false,
            bool showInMenuBar = false,
            bool showInStatusBar = false,
            bool showInToolBar = false)
        {
            WebPanelEntity webPanel;

            string locWindowName = ParseSafeName(windowName ?? name, _windowPrefixFormat);

            if (DoesWebPanelExist(name, locWindowName, out webPanel))
            {
                if (webPanel.Deleted)
                {
                    // this will happen when the user reinstalls a webpanel
                    WebPanelEntity webPanelEntity = UpdateWebPanel(webPanel.WebPanelId, name, locWindowName, url, _webPanelDescription, appearLocation
                                                    , urlEncoding
                                                    , showInAddressBar, showInMenuBar, showInStatusBar, showInToolBar
                                                    , false);	// NB!
                    ClearCache();
                    return webPanelEntity;
                }
                else
                    throw new WebPanelNameNotUniqueException(
                        "WebPanelHelper: The name isn't unique, and it isn't marked as deleted. Did you already install this? You could try again with a different name...");
            }
            else
            {
                WebPanelEntity webPanelEntity = CreateBasicWebPanel(name, locWindowName, url, appearLocation, urlEncoding, showInAddressBar, showInMenuBar, showInStatusBar, showInToolBar);
                ClearCache();
                return webPanelEntity;
            }
        }

        public bool DoesWebPanelExist(string name, string windowName, out WebPanelEntity webPanel)
        {
            using (var listAgent = new SuperOffice.CRM.Services.ListAgent())
            {
                var webPanels = listAgent.GetWebPanelList();
                //var englName = SuperOffice.CRM.Globalization.CultureDataFormatter.ParseMultiLanguageString(name, "EN");
                webPanel = webPanels.FirstOrDefault(wp => wp.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase) || wp.WindowName.StartsWith(windowName, StringComparison.InvariantCultureIgnoreCase));
                return webPanel != null;
            }

        }

        /// <summary>
        /// Creates a web panel that appears in the browser panel in superoffice with a button to open it.
        /// </summary>
        /// <param name="name">A string that represents the name of this panel. Cannot contain spaces. Spaces will be removed.</param>
        /// <param name="url">The URL the web panel will open to. </param>
        /// <param name="windowName">A string that represents the window name.</param>
        /// <param name="urlEncoding">URL encoding.</param>
        /// <param name="showInAddressBar">Determines whether the web panel displays the address bar.</param>
        /// <param name="showInMenuBar">Determines whether the web panel displays the browser menu.</param>
        /// <param name="showInStatusBar">Determines whether the web panel displays the browser status bar.</param>
        /// <param name="showInToolBar">Determines whether the web panel displays the toolbar.</param>
        /// <returns>Two web panel entities: one that represents the panel and one that represents the button.</returns>
        public WebPanelEntity[] CreateBrowserPanelWithButton(string name, string url, string windowName = null,
            UrlEncoding urlEncoding = UrlEncoding.None,
            bool showInAddressBar = false,
            bool showInMenuBar = false,
            bool showInStatusBar = false,
            bool showInToolBar = false)
        {
            WebPanelEntity webPanelEntity;
            WebPanelEntity webButtonEntity;

            //Create the Panel
            string locWindowName = ParseSafeName(windowName ?? name, _windowPrefixFormat);
            if (!DoesWebPanelExist(name, locWindowName, out webPanelEntity))
            {
                webPanelEntity = CreateBasicWebPanel(name, locWindowName, url, Navigation.BrowserPanel, urlEncoding, showInAddressBar, showInMenuBar, showInStatusBar, showInToolBar);
            }

            string buttonName = ParseSafeName(name, _buttonPrefixFormat);
            string buttonWindowName = ParseSafeName(windowName, _buttonWindowFormat);
            string buttonUrl = string.Format(_locationPrefixFormat, buttonWindowName);

            //Create the button
            if (!DoesWebPanelExist(buttonName, buttonWindowName, out webButtonEntity))
            {
                webButtonEntity = CreateBasicWebPanel(buttonName, buttonWindowName, buttonUrl, Navigation.NavigatorButton, urlEncoding, showInAddressBar, showInMenuBar, showInStatusBar, showInToolBar);    
            }

            ClearCache();

            return new WebPanelEntity[] { webPanelEntity, webButtonEntity };
        }

        private string ParseSafeName(string name, string formatString)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;


            var cleanName = Regex.Replace(name, "[^0-9a-zA-Z]+", "");
            return string.Format(formatString, cleanName);
        }

        

        private WebPanelEntity CreateBasicWebPanel(string name, string windowName, string url, Navigation appearLocation,
            UrlEncoding urlEncoding = UrlEncoding.None,
            bool showInAddressBar = false,
            bool showInMenuBar = false,
            bool showInStatusBar = false,
            bool showInToolBar = false)
        {
            WebPanelEntity mainWebPanelEntity;

            try
            {
                using (var listAgent = new SuperOffice.CRM.Services.ListAgent())
                {
                    mainWebPanelEntity = listAgent.CreateDefaultWebPanelEntity();
                    mainWebPanelEntity.Tooltip = _webPanelDescription;
                    mainWebPanelEntity.Name = name;
                    mainWebPanelEntity.WindowName = windowName;

                    mainWebPanelEntity.Url = url;
                    mainWebPanelEntity.UrlEncoding = urlEncoding;
                    mainWebPanelEntity.VisibleIn = appearLocation;
                    mainWebPanelEntity.ShowInAddressBar = showInAddressBar;
                    mainWebPanelEntity.ShowInMenuBar = showInMenuBar;
                    mainWebPanelEntity.ShowInStatusBar = showInStatusBar;
                    mainWebPanelEntity.ShowInToolBar = showInToolBar;
                    //mainWebPanelEntity.OnCentral = true;
                    mainWebPanelEntity.OnSalesMarketingWeb = true;
                    mainWebPanelEntity.Rank = 100;

                    mainWebPanelEntity = listAgent.SaveWebPanelEntity(mainWebPanelEntity);
                }
            }
            catch (Exception)
            {
                throw;
            }


            return mainWebPanelEntity;
        }

        public void ClearCache(string cacheName = null)
        {
            using (var diagAgent = new DiagnosticsAgent())
            {
                diagAgent.FlushCachesByName(new string[] { "ExtAppCache" });
            }
        }

        public WebPanelEntity UpdateWebPanel(int id, string name, string windowName, string url, string description, Navigation appearLocation,
            UrlEncoding urlEncoding = UrlEncoding.None,
            bool showInAddressBar = false,
            bool showInMenuBar = false,
            bool showInStatusBar = false,
            bool showInToolBar = false,
            bool deleted = false)
        {
            WebPanelEntity webPanel;

            using (var listAgent = new ListAgent())
            {
                webPanel = listAgent.GetWebPanelEntity(id);
                webPanel.Name = name;
                webPanel.Tooltip = description;
                webPanel.WindowName = windowName;
                webPanel.Url = url;
                webPanel.UrlEncoding = urlEncoding;
                webPanel.VisibleIn = appearLocation;
                webPanel.ShowInAddressBar = showInAddressBar;
                webPanel.ShowInMenuBar = showInMenuBar;
                webPanel.ShowInStatusBar = showInStatusBar;
                webPanel.ShowInToolBar = showInToolBar;
                webPanel.OnCentral = true;
                webPanel.OnSalesMarketingWeb = true;
                webPanel.Deleted = deleted;
                webPanel = listAgent.SaveWebPanelEntity(webPanel);
            }

            return webPanel;
        }


        public void DeleteWebPanel(string appName, string webPanelName)
        {
            var fkHelper = new ForeignKeyHelper();

            int key = fkHelper.DeleteWebPanel(appName, webPanelName);
        }

        public void DeleteWebPanelOnly(int soIdentity)
        {
            using (var listAgent = new SuperOffice.CRM.Services.ListAgent())
            {
                var webPanel = listAgent.GetWebPanelEntity(soIdentity);
                webPanel.Deleted = true;
                listAgent.SaveWebPanelEntity(webPanel);
            }
            ClearCache();
        }

        public void DeleteAllWebPanelsInForeignKeyTables(string appName)
        {
            var fkHelper = new ForeignKeyHelper();
            var webPanels = fkHelper.GetWebPanelIdentifiers(appName);

            foreach (var webPanel in webPanels)
                fkHelper.DeleteWebPanel(appName, webPanel.Key);

            ClearCache();
        }


        public void CreateAndSaveWebPanel(string name, string webPanelName, string url, Navigation navigation)
        {
            try
            {
                var fkHelper = new ForeignKeyHelper();
                if (!fkHelper.DoesKeyForWebPanelExist(name, webPanelName))
                {
                    var panel = CreateWebPanel(webPanelName, url + "&ctx=" + SoDatabaseContext.GetCurrent().ContextIdentifier, navigation);

                    // Now we need to remember this panel so that we can remove it later should the user stop paying, or end the contract:
                    // We shall use the foreign keys system in the customers database to do this. We could make a database and hold the information there; 
                    // if you take a monthly fee for the app you should do this anyway, but in this example we assume a free license or one-time fee:
                    if (panel != null)
                        fkHelper.AddKeyForWebPanel(name, webPanelName, panel.WebPanelId);
                }
            }
            catch (WebPanelNameNotUniqueException ex)
            {
                // If the panel already exists, I guess that is OK, then
            }
        }

        public Dictionary<string, int> GetInstalledWebPanelIdentifiers(string appName)
        {
            var fkHelper = new ForeignKeyHelper();

            return fkHelper.GetWebPanelIdentifiers(appName);
        }

        /// <summary>
        /// Gets a list of <list type="WebPanelEntity">WebPanelEntity</list>.
        /// </summary>
        /// <returns>List of <list type="WebPanelEntity">WebPanelEntity</list>.</returns>
        public WebPanelEntity[] GetAllWebPanels()
        {
            using (ListAgent listAgent = new ListAgent())
            {
                return listAgent.GetWebPanelList();
            }
        }

        /// <summary>
        /// Gets the available VisibleForUserGroup list as an array of <see cref="SelectableMDOListItem"/>.
        /// </summary>
        /// <param name="webPanelId">The id of the web panel.</param>
        /// <param name="listId">Outs the web panel list id which is used when setting the list of VisibleForUserGroups.</param>
        /// <returns></returns>
        public SelectableMDOListItem[] GetVisibleForUserGroupsList(int webPanelId, out int webPanelListId)
        {
            SelectableMDOListItem[] result = null;

            //Get the list id for managing web panels

            webPanelListId = GetWebPanelListId();

            if (webPanelListId > 0)
            {
                //Get the VisibleFor Lists Items

                using (ListAgent listAgent = new ListAgent())
                {
                    try
                    {
                        result = listAgent.GetVisibleForUserGroups(webPanelListId, webPanelId);
                    }
                    catch (Exception)
                    {
                        //optionally throw new/different exception.
                        throw;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Selects or de-selects a given list of VisibleForUserGroup items for a given web panel.
        /// </summary>
        /// <param name="webPanelListId">Id of web panel list. Use the out parameter from the GetVisibleForUserGroupsList method.</param>
        /// <param name="webPanelId">Id of the web panel item.</param>
        /// <param name="visibleForUserGroups">UserGroups that should be enabled or disabled.</param>
        /// <param name="enable">Set true to enable, or false to disable UserGroups.</param>
        public void SetVisibleForUserGroups(int webPanelListId, int webPanelId, bool enable, params SelectableMDOListItem[] visibleForUserGroups)
        {
            if (webPanelListId == 0 || webPanelId == 0)
                throw new NullReferenceException("Both webPanelListId and webPanelId must be specified!");

            if (visibleForUserGroups != null && visibleForUserGroups.Length > 0)
            {
                using (ListAgent listAgent = new ListAgent())
                {
                    listAgent.SetVisibleForUserGroup(webPanelListId, webPanelId,
                        visibleForUserGroups.Select(i => i.Id).ToArray(), enable);
                }
            }
            else
            {
                throw new NullReferenceException("visibleForUserGroups parameter must contain at least on item!");
            }
        }

        /// <summary>
        /// Use to obtain the list id for web panels.
        /// </summary>
        /// <returns>List Id for managing web panels. Less than zero if there was an error.</returns>
        private int GetWebPanelListId()
        {
            var webPanelListId = -1;

            using (var mdoAgent = new MDOAgent())
            {
                //Get All Lists
                var lists = mdoAgent.GetList("Lists", true, string.Empty, false);

                //Find the list item for web panels
                var listId = lists.FirstOrDefault(l => l.Type == "webpanel");

                if (listId != null)
                {
                    webPanelListId = listId.Id;
                }
            }

            return webPanelListId;
        }
    }
}
