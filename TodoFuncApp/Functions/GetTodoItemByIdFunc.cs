using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using TodoFuncApp.Models;

namespace TodoFuncApp.Functions
{
    public static class GetTodoItemByIdFunc
    {
        [FunctionName("GetTodoItemByIdFunc")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "TodoItem/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "TodoDB",
                collectionName: "TodoItems",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{id}",
                PartitionKey = "{id}")]Todo toDoItem,
            ILogger log)
        {
            log.LogInformation("Getting Todo item with id...");

            
            return new OkObjectResult(toDoItem);
        }
    }
}
