using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SuperOffice.DevNet.Online.SystemUser.PartnerDBLibrary.Models
{
    [Serializable]
    public class CustomerInfo
    {
        public int ID { get; set; }
        public int AssociateID { get; set; }
        public bool IsActive { get; set; }
        public string SystemUserToken { get; set; }
        public string ContextIdentifier { get; set; }
        public DateTime LastSync { get; set; }
    }

    public class CustomerDataSource
    {
        private const string _dataSource = @"C:\Temp\Customers.xml";

        public CustomerDataSource()
        {
            Deserialize();
        }

        public List<CustomerInfo> Customers = new List<CustomerInfo>();

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<CustomerInfo>));

            using (TextWriter writer = new StreamWriter( _dataSource))
            {
                serializer.Serialize(writer, Customers);
            }
        }

        private void Deserialize()
        {
            if (!File.Exists(_dataSource))
            {
                return;
            }

            XmlSerializer deserializer = new XmlSerializer(typeof(List<CustomerInfo>));
            using(TextReader reader = new StreamReader(_dataSource))
            {
                object obj = deserializer.Deserialize(reader);
                Customers = (List<CustomerInfo>)obj;
                reader.Close();
            }
            
        }
    }
}
