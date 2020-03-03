using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperOffice.CRM.Services;
using SuperOffice.Data;

namespace SuperOffice.DevNet.Online.Provisioning
{
    public class UserDefinedFieldHelper
    {
        /// <summary>
        /// Get a list of all user-defined fields for the specified entity type.
        /// </summary>
        /// <param name="owner">Entity type.</param>
        /// <returns>Array of <see cref="SuperOffice.CRM.Services.UserDefinedFieldInfo"/> fields</returns>
        public UserDefinedFieldInfo[] GetUserDefinedFieldList(SuperOffice.Data.UDefType owner)
        {
            using (UserDefinedFieldInfoAgent udfAgent = new UserDefinedFieldInfoAgent())
            {
                return udfAgent.GetUserDefinedFieldList(owner);
            }
        }

        /// <summary>
        /// Create user-defined field.
        /// </summary>
        /// <param name="owner">Main entity type this UDF belongs to.</param>
        /// <param name="type">Intrinsic data type for the field.</param>
        /// <param name="name">Label for the field.</param>
        /// <param name="progId">Optional. Defaults to SuperOffice:{0} where {0} is the next available number for the data type.</param>
        /// <param name="tooltip">Optional. Determines the fields tooltip.</param>
        /// <returns></returns>
        public UserDefinedFieldInfo CreateOrUpdateUserDefinedField(UDefType owner, UDefFieldType type, string name, string progId = "", string tooltip = "")
        {
            UserDefinedFieldInfo udField = null;

            using (UserDefinedFieldInfoAgent udfAgent = new UserDefinedFieldInfoAgent())
            {
                //Defaults...
                //if (fieldType == UDefFieldType.ShortText)
                //    info.TextLength = 20;
                //else if (fieldType == UDefFieldType.LongText)
                //    info.TextLength = 40;

                udField = udfAgent.CreateUserDefinedFieldInfo(owner, type);
                udField.Type = owner;
                udField.FieldType = type;
                udField.FieldLabel = name;

                if (!string.IsNullOrEmpty(progId))
                    udField.ProgId = progId;

                if (!string.IsNullOrEmpty(tooltip))
                    udField.Tooltip = tooltip;

                udField = udfAgent.SaveUserDefinedFieldInfo(udField);
            }

            return udField;
        }

        public void DeleteUserDefinedField(UDefType owner, string labelOrProgId)
        {
            var udfList = GetUserDefinedFieldList(owner);

            var listItem = udfList.FirstOrDefault(
                l => l.FieldLabel.Equals(labelOrProgId, StringComparison.InvariantCultureIgnoreCase)
                  || l.ProgId.Equals(labelOrProgId, StringComparison.InvariantCultureIgnoreCase));

            if (listItem != null)
            {
                using (UserDefinedFieldInfoAgent udfAgent = new UserDefinedFieldInfoAgent())
                {
                    udfAgent.DeleteUserDefinedFieldInfo(listItem.UDefFieldId);
                }
            }
        }

        public void PublishUserDefinedFields(UDefType owner)
        {
            using (UserDefinedFieldInfoAgent udfAgent = new UserDefinedFieldInfoAgent())
            {
                udfAgent.SetPublishStartSystemEvent(owner);
                udfAgent.Publish(owner);
            }

        }
    }
}
