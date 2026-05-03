using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Settings
{
    public class EmailSettings
    {
        public string FromEmail { get; set; }
        public string AppPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}
