using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TodoFuncApp.Models;

namespace TodoFuncApp.Functions
{
    public class AddTodoItemFunc
    {

        private CosmosClient _cosmosClient;
        public AddTodoItemFunc(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;
        }

        [FunctionName("AddTodoItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new Todo Item...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic data = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (data == null)
            {
                return new BadRequestObjectResult($"Cannot parse body.");
            }

            if (string.IsNullOrEmpty(data.Id))
            {
                data.CreatedDate = DateTime.Now;
                data.Id = Guid.NewGuid().ToString();
            }

            var container = this._cosmosClient.GetContainer("TodoDB", "TodoItems");

            try
            {
                var result = await container.CreateItemAsync<Todo>(data, new PartitionKey(data.Id));
                return new OkObjectResult(result.Resource.Id);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("Creating item failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to create item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }
            
        }
    }
}
