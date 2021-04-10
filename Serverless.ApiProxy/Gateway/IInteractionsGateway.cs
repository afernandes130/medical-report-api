using Serverless.ApiProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Gateway
{
    public interface IInteractionsGateway
    {
        public Task<List<object>> Get(Guid guidId);

        public Task Insert(object jsonInteraction, Guid idHospitalization);
    }
}
