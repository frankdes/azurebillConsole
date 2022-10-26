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

namespace azurebill
{
    public static class saveItemTest1
    {
        [FunctionName("saveItemTest1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Blob("{headers.site}/{headers.id}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream file
            )
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("valid", true);
            result.Add("msg", "ok");
            try
            {
                req.Body.Position = 0;
                await req.Body.CopyToAsync(file);
            }
            catch(Exception e)
            {
                result["valid"] = false;
                result["msg"] = e.Message;
            }
          

            return new OkObjectResult(JsonConvert.SerializeObject(result));
        }
    }
}
