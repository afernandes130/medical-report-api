using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serverless.ApiProxy.Models;

namespace Serverless.ApiProxy.Gateway
{
    public interface IEmailGateway
    {
        public Task SendEmailAsync(EmailModel model);

        public Task SendEmailAttachAsync();
    }
}
