using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperOffice.DevNet.Online.Provisioning
{
    public class WebPanelNameNotUniqueException : Exception
    {
        public WebPanelNameNotUniqueException(string message)
            : base(message)
        {
        }
    }
}
