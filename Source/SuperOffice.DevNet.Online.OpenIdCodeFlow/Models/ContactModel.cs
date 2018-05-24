using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTest.Models
{
    public class ContactModel
    {
        public int PrimaryKey { get; set; }
        public string nameDepartment { get; set; }
        public string category { get; set; }
        public string business { get; set; }
        public string number { get; set; }
        public DateTime registeredDate { get; set; }
    }
}