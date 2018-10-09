// MassDownloader
//
// This application was made to solve an issue my wife had at work. Surprisingly we did not see any quick solution to download 
// and run. 
//
// Issue: Needed a way to download multiple picture images from a list of URLs and save them locally using a special naming 
// convention. 
//
// This application looks for a csv file where this executable is. The csv file needs to be named 'reference.csv'. The format 
// of the csv should be two columns. The first column will be the URL of the image to be downloaded. The second column will be 
// the name to save the image in. 
//
// All images along with a log file will be created in a folder called 'Processed' in the location of the massdownloader.exe 
// file. 
//
// Any comments or suggestions let me know and I will get on it. 
//
// Daniel Gail
// daniel.gail@gmail.com
// www.danintexas.com

using System;
using System.IO;
using System.Net;

namespace MassDownloader
{
    class Program
    {
        public static string ExecutablePath { get; }

        static void Main(string[] args)
        {
            IntroMessage();

            int duplicate = 2, logCount = 1;

            using (StreamReader sr = new StreamReader("reference.csv"))
            {
                string currentLine, logWrite, currentFileName, currentFileNamePath = "", currentFileStatus, 
                    path = Path.Combine(Environment.CurrentDirectory, "Processed\\");

                Directory.CreateDirectory(path);

                logWrite = "File URL,Name,Status" + Environment.NewLine;
                File.WriteAllText(path + "\\Log.csv", logWrite);

                Console.Clear();

                while ((currentLine = sr.ReadLine()) != null)
                {
                    string url = currentLine.Substring(0, currentLine.LastIndexOf(","));
                    string name = currentLine.Substring(currentLine.LastIndexOf(',') + 1);

                    if (url.Contains("\""))
                    {
                        string urlClean = url.Replace("\"", "");
                        url = urlClean;
                    }

                    currentFileName = name + ".jpg";
                    currentFileNamePath = GetPath(currentFileNamePath, path, currentFileName);
                        
                    if (!File.Exists(currentFileNamePath))
                    {
                        DownloadIt(url, currentFileNamePath);

                        if (!File.Exists(currentFileNamePath))
                            currentFileStatus = "Failed to Download" + Environment.NewLine;
                        else
                            currentFileStatus = "Download Successful" + Environment.NewLine;
                                                       
                        if (logCount == 1)
                        {
                            AppendLog(logWrite, url, currentFileName, currentFileStatus, path);
                        }

                        else
                        {
                            AppendLog(logWrite, url, currentFileName, currentFileStatus, path);
                        }
                    }

                    else
                    {
                        while (File.Exists(currentFileNamePath))
                        {
                            currentFileName = name + "-" + duplicate + ".jpg";
                            currentFileNamePath = GetPath(currentFileNamePath, path, currentFileName);

                            if (!File.Exists(currentFileNamePath))
                            {
                                DownloadIt(url, currentFileNamePath);

                                if (!File.Exists(currentFileNamePath))
                                    currentFileStatus = "Failed to Download" + Environment.NewLine;
                                else
                                    currentFileStatus = "Download Successful" + Environment.NewLine;

                                AppendLog(logWrite, url, currentFileName, currentFileStatus, path);

                                duplicate = 2;
                                break;
                            }

                            else
                            {
                                duplicate++;
                            }
                        }
                    }
                    Console.Write("Processed Image: #" + logCount + Environment.NewLine);
                    logCount++;
                }
            }
            
            Console.WriteLine(Environment.NewLine + Environment.NewLine + 
                "Process is complete. All your files are located in the Processed folder.");
            Console.WriteLine(Environment.NewLine + "Total Files processed: " + (logCount -1));
            Console.WriteLine(Environment.NewLine + "   Press <ENTER> to end this application." + Environment.NewLine);
            Console.ReadLine();
        }

        static void IntroMessage()
        {
            // Initial message to user
            Console.WriteLine(Environment.NewLine + "   **********************Mass Image Downloader**********************" + Environment.NewLine);
            Console.WriteLine("   Please ensure you have a 'reference.csv' located in the folder");
            Console.WriteLine("   that this executable resides in." + Environment.NewLine);
            Console.WriteLine("   This csv should be formated as such:");
            Console.WriteLine(Environment.NewLine + "                        IMAGE URL | IMAGE NAME" + Environment.NewLine);
            Console.WriteLine("   The image will be saved as the name of the file at the URL." + Environment.NewLine);
            Console.WriteLine("   Your images along with a log.csv file will be located in the ");
            Console.WriteLine("   folder named 'Processed' in the folder this executable resides.");
            Console.WriteLine(Environment.NewLine + "   Press <ENTER> to begin the process." + Environment.NewLine);
            Console.WriteLine("   *****************************************************************");
            Console.ReadLine();

            // Run application or fail if reference.csv doesn't exist.
            if (!File.Exists("reference.csv"))
            {
                Console.Clear();
                Console.WriteLine("   *****************************************************************");
                Console.WriteLine("   'reference.csv' File Not Found");
                Console.WriteLine(Environment.NewLine + "   Press <ENTER> to end this application." + Environment.NewLine);
                Console.WriteLine("   *****************************************************************");
                Console.ReadLine();
                Environment.Exit(0);
            }

            else
            {
                Console.Clear();
            }
        }

        static void DownloadIt(string url, string currentFileNamePath)
        {
            try
            {
                System.Net.WebClient Client = new WebClient();
                Client.DownloadFile(url, currentFileNamePath);
            }

            catch
            {

            }
        }

        static void AppendLog(string logWrite, string url, string currentFileName, string currentFileStatus, string path)
        {
            logWrite = "\"" + url + "\"" + "," + currentFileName + "," + currentFileStatus;
            File.AppendAllText(path + "\\Log.csv", logWrite);
        }

        static string GetPath(string currentFileNamePath, string path, string currentFileName)
        {
            currentFileNamePath = path + "\\" + currentFileName;

            return currentFileNamePath;
        }
    }
}
