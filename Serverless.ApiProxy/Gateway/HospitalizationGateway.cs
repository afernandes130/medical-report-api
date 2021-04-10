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
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Serverless.ApiProxy.Gateway
{
    public class HospitalizationGateway : IHospitalizationGateway
    {
        private readonly string tableName = "Hospitalizations";
        private readonly string primaryPartitionKey = "id";
        private readonly string registrationIndex = "hospitalization";
        private readonly IAmazonDynamoDB amazonDynamoDbClient;

        public HospitalizationGateway(IAmazonDynamoDB amazonDynamoDbClient)
        {
            this.amazonDynamoDbClient = amazonDynamoDbClient;
        }

        public async Task<object> Get(Guid guidId)
        {
            var hospitalizations = new List<object>();
            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditions = new Dictionary<string, Condition>
                {
                    { primaryPartitionKey, new Condition()
                        {
                            ComparisonOperator = ComparisonOperator.EQ,
                            AttributeValueList = new List<AttributeValue>
                            {
                                new AttributeValue { S = guidId.ToString() }
                            }
                        }
                    }
                }
            };
            var result = await amazonDynamoDbClient.QueryAsync(request);
            foreach (var item in result.Items)
            {
                var document = Document.FromAttributeMap(item);
                //var hospitalization = JsonSerializer.Deserialize<JObject>(document[registrationIndex].AsString());
                var hospitalization = JObject.Parse(document[registrationIndex].AsString());
                hospitalization["id"] = guidId;
                hospitalizations.Add(JsonSerializer.Deserialize<object>(hospitalization.ToString()));
            }
            return hospitalizations.FirstOrDefault();
        }

        public async Task<Guid> Insert(object jsonHospitalization)
        {
            var idHospitalization = Guid.NewGuid();
            var putAttributes = new Dictionary<string, AttributeValue>
            {
                [primaryPartitionKey] = new AttributeValue { S = idHospitalization.ToString() },
                [registrationIndex] = new AttributeValue { S = JsonSerializer.Serialize(jsonHospitalization) }
            };

            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = putAttributes
            };

            await amazonDynamoDbClient.PutItemAsync(request);
            return idHospitalization;
        }

    }
}
