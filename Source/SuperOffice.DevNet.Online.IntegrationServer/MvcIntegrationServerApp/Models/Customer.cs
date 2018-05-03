using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcIntegrationServerApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public DateTime Registered { get; set; }

        public int? RegisteredId { get; set; }
        public virtual User RegisteredBy { get; set; }


        public int? LastUsedById { get; set; }
        public virtual User LastUsedBy { get; set; }
        public DateTime LastUsed { get; set; }

        public string ContextIdentifier { get; set; }

        public string Name { get;  set; }

        public string SystemUserToken { get; set; }

        public string NetServerUrl { get;  set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
