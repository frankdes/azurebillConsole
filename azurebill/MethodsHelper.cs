

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;


namespace azurebill
{
public class MethodsHelper{


        public static Dictionary<string, object> DictAdd(Dictionary<string, object> obj, string key, object value)
        {
            if (obj.ContainsKey(key))
            {
                obj[key] = value;
            }
            else
            {
                obj.Add(key, value);
            }
            return obj;
        }


        public static Stream GetStream(string body)
        {
            byte[] fileContents = System.Text.Encoding.ASCII.GetBytes(body);
            MemoryStream mFile = new MemoryStream(fileContents);
            return mFile;
        }

        public static Stream GetStream(string body, Encoding encoding)
        {
            byte[] fileContents = encoding.GetBytes(body);
            MemoryStream mFile = new MemoryStream(fileContents);
            return mFile;
        }





        public static string GetString(Stream stream,Encoding encoding)
        {
            if (stream != null)
            {
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }






        public static Dictionary<string, object> GetDict(object o)
        {
            return JsonHelper.GetDict(JsonHelper.GetJson(o));
        }
        public static bool CompareDict(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            bool IsSame = true;
            foreach (string key in a.Keys)
            {
                if (b.ContainsKey(key))
                {
                    if (a[key].GetType().Name == b[key].GetType().Name)
                    {
                        switch (a[key].GetType().Name)
                        {
                            case "String":
                            case "Double":
                            case "Integer":
                            case "Boolean":
                                IsSame = a[key].ToString() == b[key].ToString();
                                break;
                            default:
                                IsSame = CompareDict(GetDict(a[key]), GetDict(b[key]));
                                break;
                        }
                    }
                }
                else
                {
                    IsSame = false;
                }
                if (!IsSame)
                {
                    break;
                }
            }
            return IsSame;
        }
      


}
}