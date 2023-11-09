using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Helper
{
    public class EmailSendConfigure
    {
        public string[] TOs { get; set; }
        public string[] CCs { get; set; }
        public string From { get; set; }
        public string FromDisplayName { get; set; }
        public string Subject { get; set; }
        public MailPriority Priority { get; set; }
        public string ClientCredentialUserName { get; set; }
        public string ClientCredentialPassword { get; set; }
        public EmailSendConfigure()
        {
        }
    }
}
