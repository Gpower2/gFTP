using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace gFtp
{
    public class gFTP
    {

        private string _FtpDomain = "";

        public String FtpDomain 
        { 
            get 
            { 
                return _FtpDomain; 
            } 
            set
            {
                if (!value.StartsWith("ftp://"))
                {
                    _FtpDomain = String.Format("ftp://{0}", value);
                }
                else
                {
                    _FtpDomain = value;
                }
            }
        }

        public String FtpUsername { get; set; } = "";
        public String FtpPassword { get; set; } = "";

        public gFTP() { }

        public gFTP(String argFtpServer, String argUser, String argPassword)
        {
            FtpDomain = argFtpServer;
            if (!FtpDomain.StartsWith("ftp://"))
            {
                FtpDomain = String.Format("ftp://{0}", FtpDomain);
            }
            FtpUsername = argUser;
            FtpPassword = argPassword;
        }

        private FtpWebRequest GetFtpWebRequest(String argDirectory)
        {
            argDirectory = argDirectory.Replace(FtpDomain, "").Replace("#", "%23").Replace("?", "%3f");
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(UrlHelper.Combine(FtpDomain, argDirectory));
            NetworkCredential credentials = new NetworkCredential(FtpUsername, FtpPassword);
            request.Credentials = credentials;
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            return request;
        }

        public async Task DeleteRemoteFile(string argFile)
        {
            argFile = argFile.Replace(FtpDomain, "").Replace("#", "%23").Replace("?", "%3f");
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(UrlHelper.Combine(FtpDomain, argFile));
            NetworkCredential credentials = new NetworkCredential(FtpUsername, FtpPassword);
            request.Credentials = credentials;
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            request.Method = WebRequestMethods.Ftp.DeleteFile;

            using (FtpWebResponse response = (FtpWebResponse)(await request.GetResponseAsync()))
            {
                Debug.WriteLine($"Delete command Complete, status: '{response.StatusDescription}'");
                
                if (!response.StatusDescription.StartsWith("250"))
                {
                    throw new Exception(response.StatusDescription);
                }                
            }
        }

        public async Task<FtpFolder> GetFtpFolderDetailsAsync(String argPath, Int32 argLevel)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = GetFtpWebRequest(argPath);

            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            String contents = "";
            using (FtpWebResponse response = (FtpWebResponse)(await request.GetResponseAsync()))
            {
                Debug.WriteLine($"Directory {argPath} List Complete, status: '{response.StatusDescription}'");

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    contents = await reader.ReadToEndAsync();

                    Debug.WriteLine(contents);
                }
            }

            FtpFolder f = ParseFtpFolderResponse(argPath, contents);

            // Read subfolders, if argLevel>0
            if (argLevel > 0 && f.Name != "..." && f.Name != ".." && f.Name != ".")
            {
                if (f.Folders.Count > 0)
                {
                    for (int i = 0; i < f.Folders.Count; i++)
                    {
                        DateTime tmpDate = f.Folders[i].Date;
                        f.Folders[i] = await GetFtpFolderDetailsAsync(f.Folders[i].FullPath, argLevel - 1);
                        f.Folders[i].Date = tmpDate;
                    }
                }
            }
            return f;
        }

        private FtpFolder ParseFtpFolderResponse(String argPath, String argContents)
        {
            FtpFolder fRoot = new FtpFolder();
            fRoot.FullPath = argPath;
            fRoot.Name = argPath.Replace(FtpDomain, "").Split(new String[] { "/" }, StringSplitOptions.None).Last();
            if(String.IsNullOrWhiteSpace(fRoot.Name))
            {
                fRoot.Name = "/";
            }
            String[] lines = argContents.Replace("\r", "").Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String line in lines)
            {
                String[] elements = GetElements(line);
                if (line.StartsWith("d"))
                {
                    // We have a folder
                    FtpFolder childFolder = new FtpFolder();
                    childFolder.Name = elements[8];
                    childFolder.FullPath = UrlHelper.Combine(argPath, elements[8]);
                    if (elements[7].Contains(":"))
                    {
                        // the elements[7] contains hour
                        //Debug.WriteLine(String.Format("{0} {1} {2} {3}", elements[5], elements[6].PadLeft(2, '0'), DateTime.Today.Year, elements[7]));
                        childFolder.Date = DateTime.ParseExact(String.Format("{0} {1} {2} {3}", elements[5], elements[6].PadLeft(2, '0'), DateTime.Today.Year, elements[7]), 
                            "MMM dd yyyy HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture
                        );
                        if(childFolder.Date >= DateTime.Today.AddDays(1))
                        {
                            childFolder.Date = new DateTime(DateTime.Today.Year - 1, childFolder.Date.Month, childFolder.Date.Day, childFolder.Date.Hour, childFolder.Date.Minute, childFolder.Date.Second);
                        }
                    }
                    else
                    {
                        // the elements[7] contains year
                        //Debug.WriteLine(String.Format("{0} {1} {2}", elements[5], elements[6].PadLeft(2, '0'), elements[7]));
                        childFolder.Date = DateTime.ParseExact(String.Format("{0} {1} {2}", elements[5], elements[6].PadLeft(2, '0'), elements[7]),
                              "MMM dd yyyy",
                              System.Globalization.CultureInfo.InvariantCulture
                        );
                    }
                    fRoot.Folders.Add(childFolder);
                }
                else
                {
                    // We have a file
                    FtpFile childFile = new FtpFile();
                    childFile.Name = elements[8];
                    childFile.Folder = fRoot;
                    childFile.FullPath = fRoot.Name;
                    childFile.Size = Int64.Parse(elements[4]);
                    if (elements[7].Contains(":"))
                    {
                        // the elements[7] contains hour
                        //Debug.WriteLine(String.Format("{0} {1} {2} {3}", elements[5], elements[6].PadLeft(2, '0'), DateTime.Today.Year, elements[7]));
                        childFile.Date = DateTime.ParseExact(String.Format("{0} {1} {2} {3}", elements[5], elements[6].PadLeft(2, '0'), DateTime.Today.Year, elements[7]),
                            "MMM dd yyyy HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture
                        );
                        if (childFile.Date > DateTime.Today.AddDays(1))
                        {
                            childFile.Date = new DateTime(DateTime.Today.Year - 1, childFile.Date.Month, childFile.Date.Day, childFile.Date.Hour, childFile.Date.Minute, childFile.Date.Second);
                        }
                    }
                    else
                    {
                        // the elements[7] contains year
                        //Debug.WriteLine(String.Format("{0} {1} {2}", elements[5], elements[6].PadLeft(2, '0'), elements[7]));
                        childFile.Date = DateTime.ParseExact(String.Format("{0} {1} {2}", elements[5], elements[6].PadLeft(2, '0'), elements[7]),
                              "MMM dd yyyy",
                              System.Globalization.CultureInfo.InvariantCulture
                        );
                    }
                    fRoot.Files.Add(childFile);
                }
            }
            return fRoot;
        }

        private String[] GetElements(String argLine)
        {
            String[] elements = new string[9];
            Int32 currentElement = 0;
            Int32 startIndex = 0;
            Int32 endIndex = 0;
            bool foundEndIndex = false;
            for(int i = 1; i < argLine.Length; i++)
            {
                if (i == argLine.Length - 1)
                {
                    endIndex = i;
                    elements[currentElement] = argLine.Substring(startIndex, endIndex - startIndex + 1);
                    continue;
                }
                else
                {
                    if (argLine[i] == ' ')
                    {
                        if (!foundEndIndex)
                        {
                            endIndex = i;
                            elements[currentElement] = argLine.Substring(startIndex, endIndex - startIndex);
                            currentElement++;
                            foundEndIndex = true;
                            if(currentElement == 8)
                            {
                                elements[currentElement] = argLine.Substring(endIndex + 1).Trim();
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (foundEndIndex)
                        {
                            startIndex = i;
                            foundEndIndex = false;
                        }
                    }
                }
            }
            return elements;
        }
    }
}
