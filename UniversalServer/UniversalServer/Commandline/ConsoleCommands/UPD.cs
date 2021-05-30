using Moonbyte.UniversalServer.Core.Logging;
using Moonbyte.UniversalServer.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UniversalServer.Interfaces;

namespace UniversalServer.Commandline.ConsoleCommands
{
    public class UPD : IConsoleCommand
    {
        public List<string> GetActiveStrings()
        {
            var activeStrings = new List<string>();

            activeStrings.Add("upd");
            activeStrings.Add("universalplugindownload");

            return activeStrings;
        }

        public (string, string) GetHelpCommandLog()
            => ("Upd [Download-Url] [ServerName]", "Helps download plugins on-to servers / universal server.");

        public string GetName()
            => "UniversalPluginDownload";

        public void RunCommand(string[] args)
        {
            var downloadUrl = new Uri(args[1]);
            var server = args[2];

            try
            {
                using (var webClient = new WebClient())
                {
                    string fileName = Path.GetFileName(downloadUrl.LocalPath);
                    ILogger.AddToLog(ILogger.Levels.INFO, $"Downloading plugin {fileName} for server {server}");

                    webClient.DownloadProgressChanged += (obj, arg) =>
                    {
                        var translatedArgs = (DownloadProgressChangedEventArgs)arg;
                        Console.Write("\r --> Downloading " + fileName + ": " + string.Format("{0:n0}", translatedArgs.BytesReceived / 1000) + " kb");
                    };
                    webClient.DownloadFileCompleted += (obj, arg) =>
                    {
                        ILogger.AddToLog(ILogger.Levels.INFO, $"Downloading plugin {fileName} completed! Please restart the server.");
                    };

                    var universalServer = Universalserver.TcpServers.FirstOrDefault(x => Utility.EqualsIgnoreCase(x.ServerName, server));
                    var downloadDirectory = universalServer.GetPluginDirectory;

                    webClient.DownloadFileAsync(downloadUrl, Path.Combine(downloadDirectory, fileName));
                }
            }
            catch (Exception e)
            {
                ILogger.LogExceptions(e);
            }
        }
    }
}
