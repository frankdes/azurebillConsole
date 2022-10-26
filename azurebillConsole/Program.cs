
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using azurebillConsole;
using Newtonsoft.Json;

IConfiguration dsconfig = new ConfigurationBuilder()
             .AddJsonFile("settings.json")
             .AddEnvironmentVariables()
             .Build();
Settings settings = dsconfig.GetRequiredSection("Settings").Get<Settings>();
azurebilldatapost azurebilldatapost = new azurebilldatapost();
azurebilldatapost.fulllog = settings.fulllog;
azurebilldatapost.errorlog =settings.errorlog;
azurebilldatapost.saveItem = settings.saveItem;
azurebilldatapost.saveItemTest1 = settings.saveItemTest1;
azurebilldatapost.maxtrycount = settings.maxtrycount;
azurebilldatapost.trypause = settings.trypause;
azurebilldatapost.NumberOfUpdates = settings.NumberOfUpdates;
azurebilldatapost.folderCount = settings.folderCount;
azurebilldatapost.fileCount = settings.fileCount;
azurebilldatapost.siteCount = settings.siteCount;
azurebilldatapost.runmode = settings.runmode;
azurebilldatapost.sitedirectlink = settings.sitedirectlink;
Console.WriteLine(JsonConvert.SerializeObject(settings));
Console.WriteLine("enter anykey to start");
Console.ReadKey();
await azurebilldatapost.RunTest();
Console.WriteLine("Done");
Console.ReadLine();