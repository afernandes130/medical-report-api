using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serverless.ApiProxy.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Runtime.CredentialManagement;
using Amazon.DynamoDBv2.DocumentModel;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json.Linq;

namespace Serverless.ApiProxy.Gateway
{
    public class InteractionsGateway : IInteractionsGateway
    {

        private readonly string tableName = "Interactions";
        private readonly string primaryPartitionKey = "id";
        private readonly string idHospitalizationIndex = "idhospitalization-index";
        private readonly string interactionIndex = "interaction-index";
        private readonly IAmazonDynamoDB amazonDynamoDbClient;

        public InteractionsGateway(IAmazonDynamoDB amazonDynamoDbClient)
        {
            this.amazonDynamoDbClient = amazonDynamoDbClient;
        }

        public async Task<List<object>> Get(Guid guidId)
        {
            var interactions = new List<JObject>();

            var request = new ScanRequest
            {
                TableName = tableName
            };

            ScanResponse  result = await amazonDynamoDbClient.ScanAsync(request);
            foreach (var item in result.Items)
            {
                var document = Document.FromAttributeMap(item);
                if (document[idHospitalizationIndex]== guidId.ToString())
                {
                    var interaction = JObject.Parse(document[interactionIndex].AsString());
                    interactions.Add(interaction);
                }
            }
            
            var orderInteractions = interactions
                                                .OrderByDescending(obj => DateTime.Parse(obj["interactionAt"].ToString()))
                                                .Select(item => JsonSerializer.Deserialize<object>(item.ToString()))
                                                .ToList();

            return orderInteractions;
        }

        public async Task Insert(object jsonInteraction, Guid idInteraction)
        {
            var putAttributes = new Dictionary<string, AttributeValue>
            {
                [primaryPartitionKey] = new AttributeValue { S = Guid.NewGuid().ToString() },
                [idHospitalizationIndex] = new AttributeValue { S = idInteraction.ToString() },
                [interactionIndex] = new AttributeValue { S = JsonSerializer.Serialize(jsonInteraction) }
            };

            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = putAttributes
            };

            await amazonDynamoDbClient.PutItemAsync(request);
        }

    }
}
