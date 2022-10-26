using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace azurebillConsole
{
    public  class saveObject
    {

        public string url { get; set; }
        public bool hasInvalid { get; set; }

        public int maxtrycount { get; set; }
        public int trypause { get; set; }

        public Dictionary<string, object> data { get; set; }
        public List<Dictionary<string, object>> resultList { get; set; }

        public saveObject(string _url,int _maxtrycount,int _trypause, Dictionary<string, object> site, Dictionary<string, object> folder, Dictionary<string, object> file)
        {
            url = _url;
            maxtrycount = _maxtrycount;
            trypause = _trypause;
            data = new Dictionary<string, object>();
            data.Add("id", file["id"]);
            data.Add("parentId", folder["id"]);
            data.Add("site", site["id"]);
            data.Add("vid", Guid.NewGuid().ToString("N"));
            data.Add("title", file["title"]);
            data.Add("description", file["description"]);
            data.Add("containerType", "file");
            data.Add("objectType", "file");
            data.Add("pageId", 1);
            data.Add("modifiedOn", DateTime.UtcNow);
            data.Add("modifiedByUserId", Guid.NewGuid().ToString("N"));
            resultList = new List<Dictionary<string, object>>();
            hasInvalid = false;

        }
        public saveObject(string _url, int _maxtrycount, int _trypause, string site, string id) 
        {
            url = _url;
            maxtrycount = _maxtrycount;
            trypause = _trypause;
            data = new Dictionary<string, object>();
            data.Add("id",id);
            data.Add("parentId", Guid.NewGuid().ToString("N"));
            data.Add("site", site);
            data.Add("vid", Guid.NewGuid().ToString("N"));
            data.Add("title", "File " + id);
            data.Add("description", "File " + id);
            data.Add("containerType", "file");
            data.Add("objectType", "file");
            data.Add("pageId", 1);
            data.Add("modifiedOn", DateTime.UtcNow);
            data.Add("modifiedByUserId", Guid.NewGuid().ToString("N"));
            resultList = new List<Dictionary<string, object>>();
            hasInvalid = false;
        }
      
        async Task<string> PostMsg()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    httpContent.Headers.Add("userId", "user1");
                    httpContent.Headers.Add("sessionId", "session1");
                    httpContent.Headers.Add("id", data["id"].ToString());
                    httpContent.Headers.Add("vid", data["vid"].ToString());
                    httpContent.Headers.Add("parentid", data["parentId"].ToString());
                    httpContent.Headers.Add("site", data["site"].ToString());
                    httpContent.Headers.Add("pageno", data["pageId"].ToString());
                    httpContent.Headers.Add("objtype", data["objectType"].ToString());
                    httpContent.Headers.Add("containertype", data["containerType"].ToString());

                    var httpResponseMessage = await client.PostAsync(url, httpContent);

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        string resp = await httpResponseMessage.Content.ReadAsStringAsync();
                        return resp;
                    }
                    else
                    {
                        Dictionary<string, object> result = new Dictionary<string, object>();
                        result.Add("StatusCode", httpResponseMessage.StatusCode);
                        result.Add("ReasonPhrase", httpResponseMessage.ReasonPhrase);
                        result.Add("valid", false);
                        return JsonConvert.SerializeObject(result);
                    }
                }
            }
            catch(Exception ex)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("error", ex.Message);
                result.Add("valid", false);
                return JsonConvert.SerializeObject(result);
            }
          
        }
        public async Task save()
        {
            Console.WriteLine("saving :" + data["id"].ToString());
            bool valid = false;
            int i = 0;
            while(i< maxtrycount && !valid)
            {
                i++;
                string result = await PostMsg();
                if (!string.IsNullOrEmpty(result))
                {
                   
                    Dictionary<string, object> resObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    valid = resObject.ContainsKey("valid") && System.Convert.ToBoolean(resObject["valid"]);
                    resObject.Add("trycount", i);
                    resObject.Add("dataid", data["id"].ToString());
                    resObject.Add("logtime", DateTime.UtcNow);
                    resObject.Add("entrytype", "post");
                    //entrytype
                    resultList.Add(resObject);
                    if(!valid)
                    {
                        hasInvalid = true;
                    }
                }
                else
                {
                    Dictionary<string, object> resObject = new Dictionary<string, object>();
                    resObject.Add("trycount", i);
                    resObject.Add("msg", "null response");
                    resObject.Add("valid", false);
                    resObject.Add("dataid", data["id"].ToString());
                    resObject.Add("logtime", DateTime.UtcNow);
                    resObject.Add("entrytype", "post");
                    resultList.Add(resObject);
                    hasInvalid = true;
                }
                if (!valid)
                {
                    Thread.Sleep(trypause);
                }
            }

            Console.WriteLine("save end :" + data["id"].ToString());
        }
    }
}
