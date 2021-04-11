using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Models
{
    public class EmailModel
    {
        public string To { get; set; }
        public string NamePatient { get; set; }
        public Login LoginData { get; set; }
        public string Url { get; set; }
    }
}
