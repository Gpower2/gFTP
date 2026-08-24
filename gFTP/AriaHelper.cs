using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gFtp
{
    public static class AriaHelper
    {
        public delegate void AriaProgressUpdatedEventHandler(AriaData data);

        public static event AriaProgressUpdatedEventHandler AriaProgressUpdated;

        private static readonly Regex _ProgressRegEx = new Regex(@"\s(.*)/.*\((\d*)%.*DL:(\S.*)\s.*ETA:(.*)].*", RegexOptions.Compiled);

        public static Process GetAriaProcess(String argAriaPath, String argFtpPath, String argFtpFilename, String argDestPath, String argDestFilename, gFTP argFtp)
        {
            StringBuilder argBuilder = new StringBuilder();
            argBuilder.AppendFormat(" --dir=\"{0}\"", argDestPath);
            argBuilder.AppendFormat(" --out=\"{0}\"", argDestFilename.Replace("?", ""));
            argBuilder.AppendFormat(" --file-allocation=none");
            argBuilder.AppendFormat(" --download-result=full");
            argBuilder.AppendFormat(" --max-connection-per-server=15");
            argBuilder.AppendFormat(" --min-split-size=5M");
            argBuilder.AppendFormat(" --split=15");
            argBuilder.AppendFormat(" --ftp-user={0}", argFtp.FtpUsername);
            argBuilder.AppendFormat(" --ftp-passwd={0}", argFtp.FtpPassword);
            String url = UrlHelper.Combine(argFtp.FtpDomain, argFtpPath, argFtpFilename).Replace("%", "%25").Replace("#", "%23").Replace("?", "%3f");
            bool isGdl = Path.GetFileNameWithoutExtension(argAriaPath).Equals("gdl", StringComparison.OrdinalIgnoreCase);
            if (isGdl && url.StartsWith("sftp://", StringComparison.OrdinalIgnoreCase))
            {
                // gdl-specific: trust-on-first-use for unknown SFTP host keys,
                // non-interactively (like FileZilla/WinSCP auto-accept)
                argBuilder.Append(" --host-key=accept-new");
            }
            argBuilder.AppendFormat(" \"{0}\"", url);

            ProcessStartInfo myProcessInfo = new ProcessStartInfo
            {
                FileName = argAriaPath,

                Arguments = argBuilder.ToString(),

                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process myProcess = new Process
            {
                StartInfo = myProcessInfo
            };

            Debug.WriteLine(String.Format("Created process Aria for DL {0} => {1}", UrlHelper.Combine(argFtp.FtpDomain, argFtpPath, argFtpFilename), Path.Combine(argDestPath, argDestFilename)));

            return myProcess;
        }

        public static void OnAriaProgressUpdated(AriaData argData)
        {
            AriaProgressUpdated?.Invoke(argData);
        }

        public static void ProcessOutputLine(String argLine)
        {
            if (String.IsNullOrWhiteSpace(argLine))
            {
                return;
            }
            Debug.WriteLine(argLine);
            argLine = argLine.Trim();
            Match m = _ProgressRegEx.Match(argLine);
            if (!m.Success)
            {
                return;
            }
            if(m.Groups.Count == 5)
            {
                String downloaded = m.Groups[1].Value.Trim();
                Int32 progress = Int32.Parse(m.Groups[2].Value.Trim());
                String speed = m.Groups[3].Value.Trim();
                String eta = m.Groups[4].Value.Trim();

                AriaData data = new gFtp.AriaData() { Downloaded = downloaded, Progress = progress, Speed = speed, ETA = eta };

                OnAriaProgressUpdated(data);
            }
        }

        /// <summary>
        /// Reads a Process's standard output stream character by character and calls the user defined method for each line
        /// </summary>
        /// <param name="argProcess"></param>
        /// <param name="argHandler"></param>
        public static void ReadStreamPerCharacter(Process argProcess)
        {
            StreamReader reader = argProcess.StandardOutput;
            StringBuilder line = new StringBuilder();
            while (true && argProcess != null && reader != null && !argProcess.HasExited)
            {
                if (!reader.EndOfStream)
                {
                    char c = (char)reader.Read();
                    if (c == '\r')
                    {
                        if ((char)reader.Peek() == '\n')
                        {
                            // consume the next character
                            reader.Read();
                        }

                        ProcessOutputLine(line.ToString());
                        line.Length = 0;
                    }
                    else if (c == '\n')
                    {
                        ProcessOutputLine(line.ToString());
                        line.Length = 0;
                    }
                    else
                    {
                        line.Append(c);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
