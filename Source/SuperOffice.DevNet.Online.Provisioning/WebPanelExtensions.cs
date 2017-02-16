using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperOffice.CRM.Services;

namespace SuperOffice.DevNet.Online.Provisioning
{
    public static class WebPanelEntityExtensions
    {
        public static void SaveInForeignKey(this WebPanelEntity entity, string appName)
        {
            var fkHelper = new ForeignKeyHelper();

            fkHelper.AddKeyForWebPanel(appName, entity.Name, entity.WebPanelId);
        }

        public static void Delete(this WebPanelEntity entity, string appName)
        {
            var fkHelper = new ForeignKeyHelper();

            fkHelper.DeleteWebPanel(appName, entity.Name);

            var wpHelper = new WebPanelHelper();

            wpHelper.DeleteWebPanelOnly(entity.WebPanelId);
        }
    }
}
