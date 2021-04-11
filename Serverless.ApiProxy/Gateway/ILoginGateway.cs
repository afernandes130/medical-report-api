using Serverless.ApiProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Gateway
{
    public interface ILoginGateway
    {
        public Task<string> Get(Login LoginData);

        public Task Insert(Login loginData, Guid idHospitalization);
    }
}
