using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azurebill
{
    public class TreeFlowContext
    {

        public async Task<bool> saveItem(ITreeFlowParam tf)
        {
            bool ok = false;
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, object> queuedata = (Dictionary<string, object>)tf.data["queuedata"];
            try
            {

                Stream session = (Stream)tf.data["session"];
               
                MethodsHelper.DictAdd(result, "valid", false);

                if (session != null)
                {
                    session.Position = 0;
                    string sessionstr = MethodsHelper.GetString(session, Encoding.UTF8);
                    Dictionary<string, string> dict = JsonHelper.GetObject<Dictionary<string, string>>(sessionstr);
                    if (dict != null)
                    {
                        ok = dict.ContainsKey("userid") && dict.ContainsKey("sessionid") && dict["userid"] == "user1" && dict["sessionid"] == "session1";
                    }

                }

                if (ok)
                { 
                List<Task> tList = new List<Task>();
                Microsoft.AspNetCore.Http.HttpRequest req = (Microsoft.AspNetCore.Http.HttpRequest)tf.data["req"];
                Stream Body = req.Body;


                Body.Position = 0;
                MemoryStream m1 = new MemoryStream();
                Body.Position = 0;
                await Body.CopyToAsync(m1);
                m1.Position = 0;
                Stream allfile = (Stream)tf.data["allfile"];
                tList.Add(m1.CopyToAsync(allfile));
                MemoryStream m2 = new MemoryStream();
                Body.Position = 0;
                await Body.CopyToAsync(m2);
                m2.Position = 0;
                Stream versionfile = (Stream)tf.data["versionfile"];
                tList.Add(m2.CopyToAsync(versionfile));
                MemoryStream m3 = new MemoryStream();
                Body.Position = 0;
                await Body.CopyToAsync(m3);
                m3.Position = 0;
                Stream parentfile = (Stream)tf.data["parentfile"];
                tList.Add(m3.CopyToAsync(parentfile));
                MemoryStream m4 = new MemoryStream();
                Body.Position = 0;
                await Body.CopyToAsync(m4);
                m4.Position = 0;
                Stream pagenofile = (Stream)tf.data["pagenofile"];
                tList.Add(m4.CopyToAsync(pagenofile));
                Dictionary<string, object> versionDict = new Dictionary<string, object>();
                versionDict.Add("id", tf.data["parentId"]);
                versionDict.Add("vid", tf.data["vid"]);
                MemoryStream m5 = (MemoryStream)MethodsHelper.GetStream(JsonHelper.GetJson(versionDict));
                m5.Position = 0;
                tList.Add(m5.CopyToAsync((Stream)tf.data["folderversion"]));
                MemoryStream m6 = (MemoryStream)MethodsHelper.GetStream(JsonHelper.GetJson(versionDict));
                m6.Position = 0;
                tList.Add(m6.CopyToAsync((Stream)tf.data["folderpageversion"]));
                MemoryStream m7 = new MemoryStream();
                Body.Position = 0;
                await Body.CopyToAsync(m7);
                m7.Position = 0;
                Stream objecttypefile = (Stream)tf.data["objecttypefile"];
                tList.Add(m7.CopyToAsync(objecttypefile));
                Dictionary<string, object> objVersionDict = new Dictionary<string, object>();
                objVersionDict.Add("id", tf.data["objectType"]);
                objVersionDict.Add("vid", tf.data["vid"]);
                MemoryStream m8 = (MemoryStream)MethodsHelper.GetStream(JsonHelper.GetJson(objVersionDict));
                m8.Position = 0;
                tList.Add(m8.CopyToAsync((Stream)tf.data["objecttypeversion"]));

                /////////////
                MemoryStream m9 = new MemoryStream();
                Body.Position = 0;
                await Body.CopyToAsync(m9);
                m9.Position = 0;
                Stream containertypefile = (Stream)tf.data["containertypefile"];
                tList.Add(m9.CopyToAsync(containertypefile));
                Dictionary<string, object> containerVersionDict = new Dictionary<string, object>();
                containerVersionDict.Add("id", tf.data["containerType"]);
                containerVersionDict.Add("vid", tf.data["vid"]);
                MemoryStream m10 = (MemoryStream)MethodsHelper.GetStream(JsonHelper.GetJson(containerVersionDict));
                m10.Position = 0;
                tList.Add(m10.CopyToAsync((Stream)tf.data["containertypeversion"]));


                try
                {
                    await Task.WhenAll(tList);
                    MethodsHelper.DictAdd(result, "valid", true);
                    MethodsHelper.DictAdd(result, "msg", "item saved");
                    MethodsHelper.DictAdd(queuedata, "valid", true);
                    MethodsHelper.DictAdd(queuedata, "state", "item saved");

                }
                catch (Exception e)
                {

                    result.Add("msg", e.Message);
                    MethodsHelper.DictAdd(result, "valid", false);
                    MethodsHelper.DictAdd(result, "msg", e.Message);
                    MethodsHelper.DictAdd(queuedata, "valid", false);
                    MethodsHelper.DictAdd(queuedata, "state", e.Message);
                }
            }
            else
                {
                    MethodsHelper.DictAdd(result, "valid", false);
                    MethodsHelper.DictAdd(result, "msg", "invalid session");
                    MethodsHelper.DictAdd(queuedata, "valid", false);
                    MethodsHelper.DictAdd(queuedata, "state", "invalid session");
                }

                tf.data.TryAdd("result", result);
               
              

            }
            catch (Exception e)
            {
                tf.AddException(e);
                MethodsHelper.DictAdd(result, "valid", false);
                MethodsHelper.DictAdd(result, "msg",e.Message);
                MethodsHelper.DictAdd(queuedata, "valid", false);
                MethodsHelper.DictAdd(queuedata, "state", e.Message);
            }
            return ok;
        }
    }
}
