using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq;

namespace TodoFuncApp.Functions
{
    public static class DeleteTodoItemFunc
    {
        [FunctionName("DeleteTodoItemFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "TodoItem/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log, string id)
        {
            Uri documentUri = UriFactory.CreateDocumentUri("TodoDB", "TodoItems", id);
            
            await client.DeleteDocumentAsync(documentUri, new RequestOptions() { PartitionKey = new PartitionKey(id) });
            
            return new OkResult();
        }
    }
}
