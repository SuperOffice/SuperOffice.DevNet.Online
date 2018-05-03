using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcIntegrationServerApp.Models
{
    public class User
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public int AssociateId { get; set; }

        [MaxLength(255)]
        public string UserPrincipalName { get; set; }

        [MaxLength(255)]
        public string Email { get; internal set; }
    }
}
