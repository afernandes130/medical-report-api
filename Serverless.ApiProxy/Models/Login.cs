using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Models
{
    public class Login
    {
        public string Protocol { get; set; }
        public string Password { get; set; }
    }
}
