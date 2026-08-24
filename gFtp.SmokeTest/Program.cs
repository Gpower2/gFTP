using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace gFtp.SmokeTest
{
    /// <summary>
    /// Smoke test for gFTP's SFTP browsing. Reads the LAST connection from a
    /// gFTP connections.xml so no credentials live in this repo:
    ///   gFtp.SmokeTest.exe [path-to-connections.xml]
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string connectionsPath = args.Length > 0
                ? args[0]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connections.xml");

            var doc = XDocument.Load(connectionsPath);
            var last = doc.Root.Elements("FtpConnection").Last();
            string server = last.Element("Server").Value;
            string user = last.Element("Username").Value;
            string pass = last.Element("Password").Value;

            Console.WriteLine($"Server: {server}  User: {user}");

            var ftp = new gFTP(server, user, pass);
            Console.WriteLine($"IsSftp: {ftp.IsSftp}");

            FtpFolder root = ftp.GetFtpFolderDetailsAsync("", 0).GetAwaiter().GetResult();
            Console.WriteLine($"Root '{root.Name}': {root.Folders.Count} folders, {root.Files.Count} files");
            foreach (FtpFolder f in root.Folders.Take(5))
            {
                Console.WriteLine($"  [dir]  {f.Name}  -> {f.FullPath}");
            }
            foreach (FtpFile f in root.Files.Take(5))
            {
                Console.WriteLine($"  [file] {f.Name} ({f.Size} bytes)");
            }

            FtpFolder data = root.Folders.FirstOrDefault(f => f.Name == "data");
            if (data != null)
            {
                FtpFolder sub = ftp.GetFtpFolderDetailsAsync(data.FullPath, 0).GetAwaiter().GetResult();
                Console.WriteLine($"Data '{sub.FullPath}': {sub.Folders.Count} folders, {sub.Files.Count} files");
                foreach (FtpFile f in sub.Files.Take(3))
                {
                    Console.WriteLine($"  [file] {f.Name} ({f.Size} bytes)");
                }
            }

            Console.WriteLine("SMOKE TEST OK");
            return 0;
        }
    }
}
