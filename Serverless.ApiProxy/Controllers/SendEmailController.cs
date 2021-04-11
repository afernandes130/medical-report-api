using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serverless.ApiProxy.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serverless.ApiProxy.Models;

namespace Serverless.ApiProxy.Controllers
{
    [Route("modelos3d/api/[controller]")]
    public class SendEmailController : ControllerBase
    {
        private readonly IEmailGateway emailGateway;
        public SendEmailController(IEmailGateway emailGateway)
        {
            this.emailGateway = emailGateway;
            
        }


        public async Task<IActionResult> Send()
        {
            await emailGateway.SendEmailAsync(new EmailModel());
            return Ok();
        }

        [HttpPost("send-attachment")]
        public async Task<IActionResult> SendAttachment()
        {
            await emailGateway.SendEmailAttachAsync();
            return Ok();
        }
    }
}
