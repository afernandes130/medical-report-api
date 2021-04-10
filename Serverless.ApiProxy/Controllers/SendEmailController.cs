using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serverless.ApiProxy.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serverless.ApiProxy.Controllers
{
    [Route("modelos3d/api/[controller]")]
    public class SendEmailController : ControllerBase
    {
        private readonly ISimpleEmail simpleEmail;
        public SendEmailController(ISimpleEmail simpleEmail)
        {
            this.simpleEmail = simpleEmail;
            
        }


        public async Task<IActionResult> Send()
        {
            await simpleEmail.SendEmailAsync();
            return Ok();
        }

        [HttpPost("send-attachment")]
        public async Task<IActionResult> SendAttachment()
        {
            await simpleEmail.SendEmailAttachAsync();
            return Ok();
        }
    }
}
