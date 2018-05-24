using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTest.Models
{
    public class AppModel
    {
        public TimeSpan TimeLeft { get; set; }

        public string BaseUrl { get; set; }
        public string AccessToken { get; set; }

        public string Error { get; set; }

        public ContactModel[] Contacts { get; set; }
    }
}