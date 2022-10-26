using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azurebillConsole
{
    public class saveObjectDirect
    {

        public int siteCount { get; set; }

        public int updateCount { get; set; }
        public int maxtryCount { get; set; }
        public async Task<bool> SaveFile(string site, string connection, string filename, string ContentType, Stream data)
        {
            bool valid = true;
            try
            {
                BlobContainerClient siteClient = new BlobContainerClient(connection, site);
                await siteClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);
                BlobClient bc = siteClient.GetBlobClient(filename);
                BlobHttpHeaders Conditions = new BlobHttpHeaders()
                {
                    ContentType = ContentType
                };

                data.Position = 0;
                await bc.UploadAsync(data, Conditions);
            }
            catch (Exception e)
            {
                valid = false;
            }
            return valid;
        }
        public static Stream GetStream(string body, Encoding encoding)
        {
            byte[] fileContents = encoding.GetBytes(body);
            MemoryStream mFile = new MemoryStream(fileContents);
            return mFile;
        }
        public async Task<List<string>> SaveObjectDirectTest1(string site, string connection)
        {
         
            List<string> errorList = new List<string>();
            try
            {
                BlobContainerClient siteClient = new BlobContainerClient(connection, site);
                await siteClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);
                string filename =  Guid.NewGuid().ToString("N");
                for (int i = 0; i < updateCount; ++i)
                {
                    bool done = false;
                    int trycount = 0;
                    while (!done && trycount < maxtryCount)
                    {
                        try
                        {
                            Console.WriteLine("save " + i.ToString() + " try " + trycount.ToString());
                            ++trycount;
                            Dictionary<string, object> file = new Dictionary<string, object>();
                            file.Add("id", filename);
                            file.Add("title", "file" + i.ToString());
                            file.Add("description", "File" + i.ToString());
                            file.Add("vid", Guid.NewGuid().ToString("N"));
                            file.Add("parentId", Guid.NewGuid().ToString("N"));
                            file.Add("site", "test");
                            file.Add("containerType", "file");
                            file.Add("objectType", "file");
                            file.Add("pageId", 1);
                            file.Add("modifiedOn", DateTime.UtcNow);
                            file.Add("modifiedByUserId", Guid.NewGuid().ToString("N"));
                            BlobClient bc = siteClient.GetBlobClient(filename);
                            BlobHttpHeaders Conditions = new BlobHttpHeaders()
                            {
                                ContentType = "application/json"
                            };

                            Stream data = GetStream(JsonConvert.SerializeObject(file), System.Text.Encoding.UTF8);

                            data.Position = 0;
                            await bc.UploadAsync(data, Conditions);
                            done = true;
                        }
                        catch (Exception e)
                        {
                          
                            errorList.Add(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
               
                errorList.Add(e.Message);
            }
            return errorList;
        }

        public async Task<List<string>> SaveObjectDirectTest2(string connection,int sitecount,int updatecount,int maxtrycount)
        {

            List<string> errorList = new List<string>();
            try
            {
                for (int s = 0; s < sitecount; ++s)
                {
                    string site = "azurebill-2-test2-" + s.ToString();
                    BlobContainerClient siteClient = new BlobContainerClient(connection, site);
                    await siteClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);
                   
                    for (int i = 0; i < updatecount; ++i)
                    {
                        bool done = false;
                        int trycount = 0;
                        string filename = Guid.NewGuid().ToString("N");
                        while (!done && trycount < maxtrycount)
                        {

                            try
                            {
                                Console.WriteLine("save " + site + " file " + i.ToString() + " try " + trycount.ToString());
                                ++trycount;
                                Dictionary<string, object> file = new Dictionary<string, object>();
                                file.Add("id", filename);
                                file.Add("title", "file" + i.ToString());
                                file.Add("description", "File" + i.ToString());
                                file.Add("vid", Guid.NewGuid().ToString("N"));
                                file.Add("parentId", Guid.NewGuid().ToString("N"));
                                file.Add("site", site);
                                file.Add("containerType", "file");
                                file.Add("objectType", "file");
                                file.Add("pageId", 1);
                                file.Add("modifiedOn", DateTime.UtcNow);
                                file.Add("modifiedByUserId", Guid.NewGuid().ToString("N"));
                                BlobClient bc = siteClient.GetBlobClient(filename);
                                BlobHttpHeaders Conditions = new BlobHttpHeaders()
                                {
                                    ContentType = "application/json"
                                };

                                Stream data = GetStream(JsonConvert.SerializeObject(file), System.Text.Encoding.UTF8);

                                data.Position = 0;
                                await bc.UploadAsync(data, Conditions);
                                done = true;
                            }
                            catch (Exception e)
                            {
                                done = false;
                                errorList.Add(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                errorList.Add(e.Message);
            }
            return errorList;
        }


        public async Task<bool> createSession(string connection)
        {
            bool valid = true;
            try
            {
                
                BlobContainerClient siteClient = new BlobContainerClient(connection, "systemsessions");
                await siteClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

                BlobClient bc = siteClient.GetBlobClient("user1/session1.json");
                BlobHttpHeaders Conditions = new BlobHttpHeaders()
                {
                    ContentType = "application/json"
                };
                Dictionary<string, object> file = new Dictionary<string, object>();
                file.Add("userid", "user1");
                file.Add("sessionid", "session1");
                Stream data = GetStream(JsonConvert.SerializeObject(file), System.Text.Encoding.UTF8);
                data.Position = 0;
                await bc.UploadAsync(data, Conditions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                valid = false;
            }
            return valid;


        }


    }
}
