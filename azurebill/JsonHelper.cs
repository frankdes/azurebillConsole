//codefile {"site":"tfcf-4-code","parentId":"tfcf-4-code","title":"JsonHelper","isStatic":false,"namespace":"TreeFlowHelpers","project":"TreeFlowHelpers","jsLocation":"All","ext":"cs","vid":"08bb118b6f7b4013b60d3e356916b707","id":"43ad6c43e2ebc8cacad332426b1aa86e"}

//headerstart { "objectType":"header" }
using Newtonsoft.Json;
using System;
using System.Collections.Generic;








//headerend

namespace azurebill
{
public class JsonHelper{

//methodstart {"site":"tfcf-4-code","parentId":"43ad6c43e2ebc8cacad332426b1aa86e","id":"0bd3d828b66a3b9d926616d94a35b013","title":"GetObject"}
public static Cls GetObject<Cls>(string list) where Cls : new()
        {
            if (list != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Cls>(list);
                }
                catch (Exception exp)
                {
                    string msg = exp.Message;
                }
            }
            return new Cls();
        }








//methodend

//methodstart {"site":"tfcf-4-code","parentId":"43ad6c43e2ebc8cacad332426b1aa86e","id":"7fbbbe74c91431ea1cda19a2185e5ece","title":"GetJson"}
public static string GetJson(object obj)
        {
            if (obj != null)
            {
                try
                {
                    return JsonConvert.SerializeObject(obj);
                }
                catch (Exception exp)
                {
                    string msg = exp.Message;
                }
            }
            return "";
        }








//methodend

//methodstart {"site":"tfcf-4-code","parentId":"43ad6c43e2ebc8cacad332426b1aa86e","id":"fba1ca364e07b85acb7513b81a236cd6","title":"GetDict"}
public static Dictionary<string, object> GetDict(string list)
        {
            if (list != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(list);
                }
                catch (Exception exp)
                {
                    string msg = exp.Message;
                }
            }
            return new Dictionary<string, object>();
        }








//methodend

}
}