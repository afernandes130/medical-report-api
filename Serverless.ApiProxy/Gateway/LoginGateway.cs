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
    public class LoginGateway : ILoginGateway
    {
        private readonly string tableName = "Logins";
        private readonly string primaryPartitionKey = "logindata";
        private readonly string idHospitalizationIndex = "idhospitalization-index";
        private readonly IAmazonDynamoDB amazonDynamoDbClient;

        public LoginGateway(IAmazonDynamoDB amazonDynamoDbClient)
        {
            this.amazonDynamoDbClient = amazonDynamoDbClient;
        }

        public async Task<string> Get(Login loginData)
        {
            var idsHospitalization = new List<object>();
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
                                new AttributeValue { S =  JsonSerializer.Serialize(loginData) }
                            }
                        }
                    }
                }
            };
            var result = await amazonDynamoDbClient.QueryAsync(request);
            foreach (var item in result.Items)
            {
                var document = Document.FromAttributeMap(item);
                idsHospitalization.Add(document[idHospitalizationIndex].ToString());
            }

            var guidId = (idsHospitalization.FirstOrDefault() ?? string.Empty).ToString();
            return guidId;
        }

        public async Task Insert(Login loginData, Guid idHospitalization)
        {
            var putAttributes = new Dictionary<string, AttributeValue>
            {
                [primaryPartitionKey] = new AttributeValue { S = JsonSerializer.Serialize(loginData) },
                [idHospitalizationIndex] = new AttributeValue { S = idHospitalization.ToString()}
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
