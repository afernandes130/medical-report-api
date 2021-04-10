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

namespace Serverless.ApiProxy.Controllers
{
    [Route("api/[controller]")]
    public class HospitalizationsController : ControllerBase
    {
        private readonly IHospitalizationGateway hospitalizationGateway;
        private readonly IInteractionsGateway interactionsGateway;

        public HospitalizationsController(IHospitalizationGateway hospitalizationGateway, IInteractionsGateway interactionsGateway)
        {
            this.hospitalizationGateway = hospitalizationGateway;
            this.interactionsGateway = interactionsGateway;
        }

        [HttpGet("{idHospitalization}")]
        public async Task<IActionResult> GetHospitalization(Guid idHospitalization)
        {
            try
            {
                return Ok(await hospitalizationGateway.Get(idHospitalization));
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
                var result = await hospitalizationGateway.Insert(jsonHospitalization);
                return Ok(new {url = $"https://renan-saraiva.github.io/medical-report-app/{result}"});
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
                return Ok(await interactionsGateway.Get(idHospitalization));
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

    }
}
