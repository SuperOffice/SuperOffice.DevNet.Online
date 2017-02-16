using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SuperOffice.DevNet.Online.Login.Repository
{
    /// <summary>
    /// Representation of a customer datasource that maps both onsite and online customers 
    /// towards a partners application.
    /// </summary>
    public sealed class CustomerDataSource
    {
        private static volatile CustomerDataSource instance;
        private static object syncRoot = new Object();

        private CustomerDataSource()
        {
            Deserialize();
        }

        public static CustomerDataSource Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CustomerDataSource();
                    }
                }

                return instance;
            }
        }

        private const string _dataSource = @"C:\Temp\Customers.xml";


        public List<SuperOfficeContext> Customers = new List<SuperOfficeContext>();

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<SuperOfficeContext>));

            using (TextWriter writer = new StreamWriter(_dataSource))
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

            XmlSerializer deserializer = new XmlSerializer(typeof(List<SuperOfficeContext>));
            using (TextReader reader = new StreamReader(_dataSource))
            {
                object obj = deserializer.Deserialize(reader);
                Customers = (List<SuperOfficeContext>)obj;
                reader.Close();
            }

        }
    }
}
