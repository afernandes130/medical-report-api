using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Serverless.ApiProxy.Models;
using Serverless.ApiProxy.Gateway;
using Newtonsoft.Json.Linq;

namespace Serverless.ApiProxy.Controllers
{
    [Route("api/[controller]")]
    public class HospitalizationsController : ControllerBase
    {
        private readonly IHospitalizationGateway hospitalizationGateway;
        private readonly IInteractionsGateway interactionsGateway;
        private readonly ILoginGateway loginGateway;

        private readonly IEmailGateway emailGateway;


        public HospitalizationsController(
            IHospitalizationGateway hospitalizationGateway, 
            IInteractionsGateway interactionsGateway,
            ILoginGateway loginGateway,
            IEmailGateway emailGateway)
        {
            this.hospitalizationGateway = hospitalizationGateway;
            this.interactionsGateway = interactionsGateway;
            this.loginGateway = loginGateway;
            this.emailGateway = emailGateway;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromBody] Login loginData)
        {
            try
            {
                var result = await loginGateway.Get(loginData);
                if (!string.IsNullOrEmpty(result))
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idHospitalization}")]
        public async Task<IActionResult> GetHospitalization(Guid idHospitalization)
        {
            try
            {
                var result = await hospitalizationGateway.Get(idHospitalization);
                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertHospitalization([FromBody] object jsonHospitalization)
        {
            try
            {
                //var result = await hospitalizationGateway.Insert(jsonHospitalization);
                var result = await SendEmailAndInsertHospitalization(jsonHospitalization);
               
                return Ok(new {url = $"https://renan-saraiva.github.io/medical-report-app/hospitalizations/{result}"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("{idHospitalization}/interactions")]
        public async Task<IActionResult> GetInteractions(Guid idHospitalization)
        {
            try
            {
                var result = await interactionsGateway.Get(idHospitalization);
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("{idHospitalization}/interactions")]
        public async Task<IActionResult> InsertInteraction([FromBody] object jsonInteraction, Guid idHospitalization)
        {
            try
            {
                await interactionsGateway.Insert(jsonInteraction, idHospitalization);
                return Created("{idHospitalization}/interactions", idHospitalization);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public async Task<Guid> SendEmailAndInsertHospitalization(object jsonHospitalization)
        {
            var hospitalization = JObject.Parse(jsonHospitalization.ToString() ?? string.Empty);

            var patient = hospitalization["patient"];

            var login = new Login()
            {
                Protocol = GeneratorTypes(Type.Protocol),
                Password = GeneratorTypes(Type.Password)
            };

            var model = new EmailModel()
            {
                To = hospitalization["contact"].ToString(),
                NamePatient = patient["name"].ToString(),
                LoginData = login
            };

            
            var result =  await hospitalizationGateway.Insert(jsonHospitalization);
            await loginGateway.Insert(login, result);
            model.Url = $"https://renan-saraiva.github.io/medical-report-app/login";
            await emailGateway.SendEmailAsync(model);

            return result;
        }

        private string GeneratorTypes(Type type )
        {
            string chars = string.Empty;
            if (type == Type.Password)
            {
                chars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_ - ";
            }
            if (type == Type.Protocol)
            {
                chars = "0123456789";
            }

            var pass = string.Empty;
            Random random = new Random();
            for (int f = 0; f < 8; f++)
            {
                pass = pass + chars.Substring(random.Next(0, chars.Length - 1), 1);
            }

            return pass; 
        }

        enum Type
        {
            Protocol,
            Password
        }
    }
}
