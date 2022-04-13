using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using TodoFuncApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace TodoFuncApp.Functions
{
    public class GetTodoItemsFunc
    {

        private CosmosClient _cosmosClient;
        public GetTodoItemsFunc(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;
        }

        [FunctionName("GetTodoItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting all Todo Items...");

            QueryDefinition query = new QueryDefinition(
                "select * from TodoItems");

            var container = this._cosmosClient.GetContainer("TodoDB", "TodoItems");

            List<Todo> todoItemsList = new List<Todo>();
            using (FeedIterator<Todo> resultSet = container.GetItemQueryIterator<Todo>(query))
            {
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<Todo> response = await resultSet.ReadNextAsync();
                    Todo todoItem = response.First();
                    log.LogInformation($"Id: {todoItem.Id};");
                    if (response.Diagnostics != null)
                    {
                        Console.WriteLine($" Diagnostics {response.Diagnostics.ToString()}");
                    }

                    todoItemsList.AddRange(response);

                }
            }

            return new OkObjectResult(todoItemsList);
        }
    }
}
