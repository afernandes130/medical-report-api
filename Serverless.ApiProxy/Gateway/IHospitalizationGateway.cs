using Serverless.ApiProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Gateway
{
    public interface IHospitalizationGateway
    {
        public Task<object> Get(Guid guidId);

        public Task<Guid> Insert(object jsonHospitalization);
    }
}
