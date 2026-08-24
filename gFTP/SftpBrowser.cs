using System;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace gFtp
{
    /// <summary>
    /// SFTP browsing (list/delete) over SSH.NET, used when the connection domain
    /// is sftp:// — FtpWebRequest cannot speak SFTP.
    /// Paths follow the same convention as the FTP flow: relative to the login
    /// home directory, "" = home.
    /// </summary>
    internal sealed class SftpBrowser : IDisposable
    {
        private readonly SftpClient _client;

        public SftpBrowser(String domain, String username, String password)
        {
            var uri = new Uri(domain);
            int port = uri.IsDefaultPort || uri.Port <= 0 ? 22 : uri.Port;

            var connectionInfo = new ConnectionInfo(uri.Host, port, username,
                new PasswordAuthenticationMethod(username, password))
            {
                Timeout = TimeSpan.FromSeconds(30),
            };
            _client = new SftpClient(connectionInfo);
            _client.HostKeyReceived += (sender, e) =>
            {
                e.CanTrust = SftpKnownHosts.TrustOnFirstUse(uri.Host, port, e.HostKeyName, e.HostKey);
            };
            _client.Connect();
        }

        private static String Normalize(String path)
        {
            if (String.IsNullOrWhiteSpace(path) || path == "/")
            {
                return "."; // login home
            }
            return path.TrimStart('/');
        }

        public FtpFolder ListFolder(String argPath)
        {
            String serverPath = Normalize(argPath);

            var folder = new FtpFolder
            {
                FullPath = argPath ?? "",
                Name = String.IsNullOrWhiteSpace(argPath) ? "/" : argPath.TrimEnd('/').Substring(argPath.TrimEnd('/').LastIndexOf('/') + 1),
            };

            foreach (ISftpFile entry in _client.ListDirectory(serverPath))
            {
                if (entry.Name == "." || entry.Name == "..")
                {
                    continue;
                }
                if (entry.IsDirectory)
                {
                    folder.Folders.Add(new FtpFolder
                    {
                        Name = entry.Name,
                        FullPath = UrlHelper.Combine(argPath ?? "", entry.Name),
                        Date = entry.LastWriteTime,
                    });
                }
                else if (entry.IsRegularFile)
                {
                    folder.Files.Add(new FtpFile
                    {
                        Name = entry.Name,
                        Size = entry.Length,
                        Date = entry.LastWriteTime,
                        Folder = folder,
                    });
                }
            }
            return folder;
        }

        public void DeleteFile(String argPath)
        {
            _client.DeleteFile(Normalize(argPath));
        }

        public void DeleteFolder(String argPath)
        {
            _client.DeleteDirectory(Normalize(argPath));
        }

        public void Dispose()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
            _client.Dispose();
        }
    }
}
