using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace azurebillConsole
{
    public class azurebilldatapost
    {
        public int siteCount { get; set; }
        public int folderCount { get; set; }
        public int fileCount { get; set; }
        public int NumberOfUpdates { get; set; }
        public int maxtrycount { get; set; }
        public int trypause { get; set; }
        public string runmode { get; set; }
        public string fulllog { get; set; }
        public string errorlog { get; set; }
        public string sitedirectlink { get; set; }
        public string saveItem { get; set; }
        public string saveItemTest1 { get; set; }

        public string ErrLogFile
        {
            get { return errorlog + @"/" + runmode + "Errors" + DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");  }
        }

        public string LogFile
        {
            get { return errorlog + @"/" + runmode + "_" + DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss"); }
        }

        public List<Dictionary<string,object>> sites { get; set; }
        public List<Dictionary<string, object>> folders { get; set; }
        public List<Dictionary<string, object>> files { get; set; }
    

        public void CreateTestData()
        {
            CreateContainers();
            CreateFolders();
            CreateFiles();
        }
        public void CreateTestData(string siteprefix)
        {
            CreateContainers(siteprefix);
            CreateFolders();
            CreateFiles();
        }

        public void CreateContainers()
        {
            sites = new List<Dictionary<string, object>>();
            for (int i = 0; i < siteCount; i++)
            {
                Dictionary<string, object> site = new Dictionary<string, object>();
                site.Add("id", "azurebill-1-site" + i.ToString());
                site.Add("title", "site" + i.ToString());
                sites.Add(site);
            }


        }
        public void CreateContainers(string siteprefix)
        {
            sites = new List<Dictionary<string, object>>();
            for (int i = 0; i < siteCount; i++)
            {
                Dictionary<string, object> site = new Dictionary<string, object>();
                site.Add("id", siteprefix + i.ToString());
                site.Add("title", "site" + i.ToString());
                sites.Add(site);
            }


        }
        public void CreateFolders()
        {

            folders = new List<Dictionary<string, object>>();
            for (int i = 0; i < folderCount; i++)
            {
                Dictionary<string, object> folder = new Dictionary<string, object>();
                folder.Add("id", Guid.NewGuid().ToString("N"));
                folder.Add("title", "Folder" + i.ToString());
                folder.Add("description", "Folder" + i.ToString());
                folder.Add("vid", Guid.NewGuid().ToString("N"));
                folders.Add(folder);
            }

        }

        public void CreateFiles()
        {
            files = new List<Dictionary<string, object>>();
            for (int i = 0; i < folderCount; i++)
            {
                Dictionary<string, object> file = new Dictionary<string, object>();
                file.Add("id", Guid.NewGuid().ToString("N"));
                file.Add("title", "File" + i.ToString());
                file.Add("description", "File" + i.ToString());
                file.Add("vid", Guid.NewGuid().ToString("N"));
                files.Add(file);
            }
        }

        public async Task RunTest()
        {
            switch(runmode)
            {
                case "test1":
                    await test1();
                    break;
                case "test2":
                    await test2();
                    break;
                case "test3":
                    await test3();
                    break;
                case "test4":
                    await RunSerialTest();
                    break;
                case "test5":
                    await RunParallelTest();
                    break;

            }

        }
        public async Task RunParallelTest()
        {
            if (await CreateSession())
            {
                StringBuilder sbfulllog = new StringBuilder();
                StringBuilder sberrorlog = new StringBuilder();
                DateTime start = DateTime.UtcNow;
                CreateTestData("azurebill-5-site");
                List<saveObject> svList = new List<saveObject>();
                for (int i = 0; i < NumberOfUpdates; ++i)
                {
                    Console.WriteLine("Update :" + i.ToString());
                    try
                    {
                        List<Task> tList = new List<Task>();
                        foreach (Dictionary<string, object> site in sites)
                        {
                            foreach (Dictionary<string, object> folder in folders)
                            {
                                foreach (Dictionary<string, object> file in files)
                                {
                                    saveObject sv = new saveObject(saveItem, maxtrycount, trypause, site, folder, file);
                                    svList.Add(sv);
                                    tList.Add(sv.save());
                                }
                            }
                        }
                        Console.WriteLine("start save");
                        await Task.WhenAll(tList);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        sberrorlog.AppendLine(e.StackTrace);
                    }
                }
                DateTime end = DateTime.UtcNow;
                double totalTime = (end - start).TotalMilliseconds;
                int cnt = 0;
                int fileNumber = 0;
                Dictionary<string, object> timing = null;
                foreach (saveObject sv in svList)
                {
                    cnt++;
                    foreach (Dictionary<string, object> result in sv.resultList)
                    {
                        sbfulllog.AppendLine(JsonConvert.SerializeObject(result));
                        if (sv.hasInvalid)
                        {
                            sberrorlog.AppendLine(JsonConvert.SerializeObject(result));
                        }
                    }
                    if (cnt > 1000)
                    {
                        fileNumber++;
                        timing = new Dictionary<string, object>();
                        timing.Add("entrytype", "timing");
                        timing.Add("start", start);
                        timing.Add("end", end);
                        timing.Add("duration", totalTime);
                        timing.Add("siteCount", siteCount);
                        timing.Add("folderCount", folderCount);
                        timing.Add("fileCount", fileCount);
                        timing.Add("NumberOfUpdates", NumberOfUpdates);
                        timing.Add("maxtrycount", maxtrycount);
                        timing.Add("trypause", trypause);
                        timing.Add("runmode", runmode);
                        timing.Add("fulllog", fulllog);
                        timing.Add("errorlog", errorlog);
                        timing.Add("url", saveItemTest1);
                        sbfulllog.AppendLine(JsonConvert.SerializeObject(timing));
                        sberrorlog.AppendLine(JsonConvert.SerializeObject(timing));
                        File.WriteAllText(LogFile + fileNumber.ToString() + ".txt", sbfulllog.ToString());
                        File.WriteAllText(ErrLogFile + fileNumber.ToString() + ".txt", sberrorlog.ToString());
                        cnt = 0;
                        sbfulllog = new StringBuilder();
                        sberrorlog = new StringBuilder();
                    }
                }
                if (cnt > 0)
                {
                    fileNumber++;
                    timing = new Dictionary<string, object>();
                    timing.Add("entrytype", "timing");
                    timing.Add("start", start);
                    timing.Add("end", end);
                    timing.Add("duration", totalTime);
                    timing.Add("siteCount", siteCount);
                    timing.Add("folderCount", folderCount);
                    timing.Add("fileCount", fileCount);
                    timing.Add("NumberOfUpdates", NumberOfUpdates);
                    timing.Add("maxtrycount", maxtrycount);
                    timing.Add("trypause", trypause);
                    timing.Add("runmode", runmode);
                    timing.Add("fulllog", fulllog);
                    timing.Add("errorlog", errorlog);
                    timing.Add("url", saveItemTest1);
                    sbfulllog.AppendLine(JsonConvert.SerializeObject(timing));
                    sberrorlog.AppendLine(JsonConvert.SerializeObject(timing));
                    File.WriteAllText(LogFile + fileNumber.ToString() + ".txt", sbfulllog.ToString());
                    File.WriteAllText(ErrLogFile + fileNumber.ToString() + ".txt", sberrorlog.ToString());
                    cnt = 0;
                    sbfulllog = new StringBuilder();
                    sberrorlog = new StringBuilder();
                }

            }
        }
        public async Task RunSerialTest()
        {
            if (await CreateSession())
            {
                StringBuilder sbfulllog = new StringBuilder();
                StringBuilder sberrorlog = new StringBuilder();
                DateTime start = DateTime.UtcNow;
                CreateTestData("azurebill-4-site");
                List<saveObject> svList = new List<saveObject>();
                for (int i = 0; i < NumberOfUpdates; ++i)
                {
                    Console.WriteLine("Update :" + i.ToString());
                    try
                    {
                        List<Task> tList = new List<Task>();

                        foreach (Dictionary<string, object> site in sites)
                        {
                            foreach (Dictionary<string, object> folder in folders)
                            {
                                foreach (Dictionary<string, object> file in files)
                                {
                                    saveObject sv = new saveObject(saveItem, maxtrycount, trypause, site, folder, file);
                                    svList.Add(sv);
                                    await sv.save();
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        sberrorlog.AppendLine(e.StackTrace);
                    }


                }
                DateTime end = DateTime.UtcNow;
                double totalTime = (end - start).TotalMilliseconds;
                int cnt = 0;
                int fileNumber = 0;
                Dictionary<string, object> timing = null;
                foreach (saveObject sv in svList)
                {
                    cnt++;
                    foreach (Dictionary<string, object> result in sv.resultList)
                    {
                        sbfulllog.AppendLine(JsonConvert.SerializeObject(result));
                        if (sv.hasInvalid)
                        {
                            sberrorlog.AppendLine(JsonConvert.SerializeObject(result));
                        }
                    }
                    if (cnt > 1000)
                    {
                        fileNumber++;
                         timing = new Dictionary<string, object>();
                        timing.Add("entrytype", "timing");
                        timing.Add("start", start);
                        timing.Add("end", end);
                        timing.Add("duration", totalTime);
                        timing.Add("siteCount", siteCount);
                        timing.Add("folderCount", folderCount);
                        timing.Add("fileCount", fileCount);
                        timing.Add("NumberOfUpdates", NumberOfUpdates);
                        timing.Add("maxtrycount", maxtrycount);
                        timing.Add("trypause", trypause);
                        timing.Add("runmode", runmode);
                        timing.Add("fulllog", fulllog);
                        timing.Add("errorlog", errorlog);
                        timing.Add("url", saveItemTest1);
                        sbfulllog.AppendLine(JsonConvert.SerializeObject(timing));
                        sberrorlog.AppendLine(JsonConvert.SerializeObject(timing));
                        File.WriteAllText(LogFile + fileNumber.ToString() + ".txt", sbfulllog.ToString());
                        File.WriteAllText(ErrLogFile + fileNumber.ToString() + ".txt", sberrorlog.ToString());
                        cnt = 0;
                        sbfulllog = new StringBuilder();
                        sberrorlog = new StringBuilder();
                    }
                }


                if (cnt > 0)
                {
                    fileNumber++;
                    timing = new Dictionary<string, object>();
                    timing.Add("entrytype", "timing");
                    timing.Add("start", start);
                    timing.Add("end", end);
                    timing.Add("duration", totalTime);
                    timing.Add("siteCount", siteCount);
                    timing.Add("folderCount", folderCount);
                    timing.Add("fileCount", fileCount);
                    timing.Add("NumberOfUpdates", NumberOfUpdates);
                    timing.Add("maxtrycount", maxtrycount);
                    timing.Add("trypause", trypause);
                    timing.Add("runmode", runmode);
                    timing.Add("fulllog", fulllog);
                    timing.Add("errorlog", errorlog);
                    timing.Add("url", saveItemTest1);
                    sbfulllog.AppendLine(JsonConvert.SerializeObject(timing));
                    sberrorlog.AppendLine(JsonConvert.SerializeObject(timing));
                    File.WriteAllText(LogFile + fileNumber.ToString() + ".txt", sbfulllog.ToString());
                    File.WriteAllText(ErrLogFile + fileNumber.ToString() + ".txt", sberrorlog.ToString());
                    cnt = 0;
                    sbfulllog = new StringBuilder();
                    sberrorlog = new StringBuilder();
                }
            }
        }

        string GetFileHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("test:" + runmode);
            sb.AppendLine("time:" + DateTime.UtcNow.ToString("yyyy-MM-dddd : HH:mm:ss"));
            sb.AppendLine("sites:" + siteCount.ToString());
            sb.AppendLine("folders:" + folderCount.ToString());
            sb.AppendLine("files:" + fileCount.ToString());
            sb.AppendLine("NumberOfUpdates:" + NumberOfUpdates.ToString());
            sb.AppendLine("maxtrycount:" + maxtrycount.ToString());
            sb.AppendLine("trypause:" + trypause.ToString());
            sb.AppendLine("fulllog:" + fulllog);
            sb.AppendLine("errorlog:" + errorlog);
            sb.AppendLine("saveItem:" + saveItem);
            sb.AppendLine("saveItemTest1:" + saveItemTest1);
            sb.AppendLine("sitedirectlink:" + sitedirectlink);
            return sb.ToString();
        }

        public async Task  test1()
        {
            saveObjectDirect sd = new saveObjectDirect();
            sd.updateCount = siteCount * folderCount * fileCount * NumberOfUpdates * 10;
            sd.maxtryCount = maxtrycount;
            List<string> data =  await sd.SaveObjectDirectTest1("azurebill-1-test1", sitedirectlink);
            File.WriteAllText(LogFile + ".txt", GetFileHeader());
            if(data.Count > 0)
            {
                File.WriteAllText(ErrLogFile + ".txt", JsonConvert.SerializeObject(data));
            }
           
        }
        public async Task test2()
        {
            saveObjectDirect sd = new saveObjectDirect();

            List<string> data = await sd.SaveObjectDirectTest2(sitedirectlink,siteCount, folderCount * fileCount * NumberOfUpdates * 10,maxtrycount);
            File.WriteAllText(LogFile + ".txt", GetFileHeader());
            if (data.Count > 0)
            {
                File.WriteAllText(ErrLogFile + ".txt", JsonConvert.SerializeObject(data));
            }
        }

        public async Task test3()
        {
            int number = siteCount * folderCount * fileCount * NumberOfUpdates * 10;
            StringBuilder sbfulllog = new StringBuilder();
            StringBuilder sberrorlog = new StringBuilder();
            DateTime start = DateTime.UtcNow;
            string id = Guid.NewGuid().ToString("N");
            List<saveObject> svList = new List<saveObject>();
            for (int i = 0; i < number; ++i)
            {
                Console.WriteLine("Update :" + i.ToString());
                try
                {
                    List<Task> tList = new List<Task>();
                    saveObject sv = new saveObject(saveItemTest1, maxtrycount, trypause,"azurebill-3-test3",id);
                    svList.Add(sv);
                    await sv.save();                
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    sberrorlog.AppendLine(e.StackTrace);
                }
            }
            DateTime end = DateTime.UtcNow;
            double totalTime = (end - start).TotalMilliseconds;
            int cnt = 0;
            int fileNumber = 0;
            Dictionary<string, object> timing = null;
            foreach (saveObject sv in svList)
            {
                cnt++;
                foreach (Dictionary<string, object> result in sv.resultList)
                {
                    sbfulllog.AppendLine(JsonConvert.SerializeObject(result));
                    if (sv.hasInvalid)
                    {
                        sberrorlog.AppendLine(JsonConvert.SerializeObject(result));
                    }
                }
                if(cnt > 1000)
                {
                    fileNumber++;
                    timing = new Dictionary<string, object>();
                    timing.Add("entrytype", "timing");
                    timing.Add("start", start);
                    timing.Add("end", end);
                    timing.Add("duration", totalTime);
                    timing.Add("siteCount", siteCount);
                    timing.Add("folderCount", folderCount);
                    timing.Add("fileCount", fileCount);
                    timing.Add("NumberOfUpdates", NumberOfUpdates);
                    timing.Add("maxtrycount", maxtrycount);
                    timing.Add("trypause", trypause);
                    timing.Add("runmode", runmode);
                    timing.Add("fulllog", fulllog);
                    timing.Add("errorlog", errorlog);
                    timing.Add("url", saveItemTest1);
                    sbfulllog.AppendLine(JsonConvert.SerializeObject(timing));
                    sberrorlog.AppendLine(JsonConvert.SerializeObject(timing));
                    File.WriteAllText(LogFile + fileNumber.ToString() + ".txt", sbfulllog.ToString());
                    File.WriteAllText(ErrLogFile + fileNumber.ToString() + ".txt", sberrorlog.ToString());
                    cnt = 0;
                    sbfulllog = new StringBuilder();
                    sberrorlog = new StringBuilder();
                }
            }

            if(cnt > 0)
            {
                fileNumber++;
                 timing = new Dictionary<string, object>();
                timing.Add("entrytype", "timing");
                timing.Add("start", start);
                timing.Add("end", end);
                timing.Add("duration", totalTime);
                timing.Add("siteCount", siteCount);
                timing.Add("folderCount", folderCount);
                timing.Add("fileCount", fileCount);
                timing.Add("NumberOfUpdates", NumberOfUpdates);
                timing.Add("maxtrycount", maxtrycount);
                timing.Add("trypause", trypause);
                timing.Add("runmode", runmode);
                timing.Add("fulllog", fulllog);
                timing.Add("errorlog", errorlog);
                timing.Add("url", saveItemTest1);
                sbfulllog.AppendLine(JsonConvert.SerializeObject(timing));
                sberrorlog.AppendLine(JsonConvert.SerializeObject(timing));
                File.WriteAllText(LogFile + fileNumber.ToString() + ".txt", sbfulllog.ToString());
                File.WriteAllText(ErrLogFile + fileNumber.ToString() + ".txt", sberrorlog.ToString());
                cnt = 0;
                sbfulllog = new StringBuilder();
                sberrorlog = new StringBuilder();
            }
          
        }

        public async Task<bool> CreateSession()
        {
            saveObjectDirect sd = new saveObjectDirect();
           return await sd.createSession(sitedirectlink);
        }
    }
}
