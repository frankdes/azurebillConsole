
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
using System.Text;






//headerend
namespace azurebill
{
public static class saveItem{


[FunctionName("saveItem")]
          public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route =null)] HttpRequest req,
          [Blob("{headers.site}/all/{headers.id}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream allfile,
          [Blob("{headers.site}/versions/{headers.id}/{headers.vid}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream versionfile,
          [Blob("{headers.site}/{headers.parentid}/{headers.id}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream parentfile,
          [Blob("{headers.site}/pages/{headers.parentid}/{headers.pageno}/{headers.id}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream pagenofile,
          [Blob("{headers.site}/{headers.objtype}/{headers.id}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream objecttypefile,
          [Blob("{headers.site}/sys{headers.containertype}/{headers.id}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream containertypefile,
          [Blob("{headers.site}/childversions/{headers.parentid}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream folderversion,
          [Blob("{headers.site}/pageversions/{headers.parentid}/{headers.pageno}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream folderpageversion,
          [Blob("{headers.site}/objecttypeversions/{headers.objtype}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream objecttypeversion,
          [Blob("{headers.site}/containertypeversions/{headers.containertype}.json", FileAccess.Write, Connection = "storageAccountKey")] Stream containertypeversion,
          [Blob("systemsessions/{headers.userId}/{headers.sessionId}.json", FileAccess.Read, Connection = "storageAccountKey")] Stream session,
          [Queue("queuesystem"), StorageAccount("storageAccountKey")] ICollector<string> Qmsg    
          
            )
        {
            string requestid = Guid.NewGuid().ToString("N");
            DateTime start = DateTime.UtcNow;
            ITreeFlowParam tf = new TreeFlowParam();
            try
            { 
            
            tf.data.TryAdd("site", req.Headers["site"]);
            tf.data.TryAdd("vid", req.Headers["vid"]);
            tf.data.TryAdd("id", req.Headers["id"]);
            tf.data.TryAdd("objectType", req.Headers["objtype"]);
            tf.data.TryAdd("parentId", req.Headers["parentid"]);
            tf.data.TryAdd("pageNo", req.Headers["pageno"]);        
            tf.data.TryAdd("containerType", req.Headers["containertype"]);
            tf.data.TryAdd("type", "dict");
            tf.data.TryAdd("allfile", allfile);
            tf.data.TryAdd("versionfile", versionfile);
            tf.data.TryAdd("parentfile", parentfile);
            tf.data.TryAdd("pagenofile", pagenofile);
            tf.data.TryAdd("objecttypefile", objecttypefile);
            tf.data.TryAdd("containertypefile", containertypefile);
            tf.data.TryAdd("folderversion", folderversion);
            tf.data.TryAdd("folderpageversion", folderpageversion);
            tf.data.TryAdd("objecttypeversion", objecttypeversion);
            tf.data.TryAdd("containertypeversion", containertypeversion);
            tf.data.TryAdd("session",session); 
            tf.data.TryAdd("Qmsg",Qmsg); 
            tf.data.TryAdd("req",req);
            Dictionary<string,object> queuedata = new Dictionary<string,object>();
            queuedata.Add("site", req.Headers["site"]);
            queuedata.Add("parentId", req.Headers["parentid"]);
            queuedata.Add("pageNo", req.Headers["pageno"]);
            queuedata.Add("objectType", req.Headers["objtype"]);
            queuedata.Add("id", req.Headers["id"]);
            queuedata.Add("vid", req.Headers["vid"]);
            queuedata.Add("servermethod", "saveItem");
            queuedata.Add("state", "not saved");
            queuedata.Add("valid", false);
            queuedata.Add("requestid", requestid);
            tf.data.AddOrUpdate("queuedata", queuedata, (a,b)=>queuedata);
            TreeFlowContext ctx = new TreeFlowContext();
            await ctx.saveItem(tf);
                double totalTime = (DateTime.UtcNow - start).TotalMilliseconds;
                queuedata.Add("totaltime", totalTime);
                Qmsg.Add(JsonHelper.GetJson(queuedata));
            }
            catch(Exception e) { 
                tf.AddException(e); 
            }
            if(tf.IsException)
            {
                StringBuilder sb = new StringBuilder();
                foreach(Exception e in tf.exceptions)
                {
                    sb.AppendLine(e.Message);
                }

                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("valid", false);
                result.Add("requestid", requestid);
                result.Add("msg", sb.ToString());
                return new OkObjectResult(JsonHelper.GetJson(result));
            }
            else
            {
                Dictionary<string, object> result = (Dictionary<string, object>)tf.data["result"];
                result.Add("requestid", requestid);
                return new OkObjectResult(JsonHelper.GetJson(result));
            }           
        }





//methodend

}
}