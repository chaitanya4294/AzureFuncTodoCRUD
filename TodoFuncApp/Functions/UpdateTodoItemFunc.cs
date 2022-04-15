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
using TodoFuncApp.Models;
using System.Linq;

namespace TodoFuncApp.Functions
{
    public static class UpdateTodoItemFunc
    {
        [FunctionName("UpdateTodoItemFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "TodoItem/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log, string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<Todo>(requestBody);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("TodoDB", "TodoItems");
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                            .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return new NotFoundResult();
            }

            if (!string.IsNullOrEmpty(updated.Title))
            {
                document.SetPropertyValue("Title", updated.Title);
            }

            if (!string.IsNullOrEmpty(updated.Description))
            {
                document.SetPropertyValue("Description", updated.Description);
            }

            await client.ReplaceDocumentAsync(document);

            /* var todo = new Todo()
            {
                Id = document.GetPropertyValue<string>("id"),
                CreatedTime = document.GetPropertyValue<DateTime>("CreatedTime"),
                TaskDescription = document.GetPropertyValue<string>("TaskDescription"),
                IsCompleted = document.GetPropertyValue<bool>("IsCompleted")
            };*/

            // an easier way to deserialize a Document
            Todo todo = (dynamic)document;


            return new OkObjectResult(todo);
        }
    }
}
