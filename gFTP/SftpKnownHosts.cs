using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace gFtp
{
    [DataContract]
    internal class KnownHostEntry
    {
        [DataMember(Name = "keyType")] public String KeyType { get; set; }
        [DataMember(Name = "sha256")] public String Sha256 { get; set; }
        [DataMember(Name = "addedUtc")] public String AddedUtc { get; set; }
    }

    /// <summary>
    /// TOFU host-key trust, sharing gdl's known_hosts.json store
    /// (%APPDATA%\gdl\known_hosts.json): unknown keys are trusted and cached
    /// (accept-new), changed keys are rejected loudly.
    /// </summary>
    internal static class SftpKnownHosts
    {
        private static readonly String StorePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "gdl", "known_hosts.json");

        private static readonly Object Sync = new Object();

        public static String Sha256Fingerprint(byte[] hostKey)
        {
            using (var sha = SHA256.Create())
            {
                return "SHA256:" + Convert.ToBase64String(sha.ComputeHash(hostKey)).TrimEnd('=');
            }
        }

        /// <summary>
        /// Returns true when the key may be trusted (known match, or first-seen key
        /// which is then cached). Throws when a previously cached key has changed.
        /// </summary>
        public static bool TrustOnFirstUse(String host, int port, String keyType, byte[] hostKey)
        {
            String fingerprint = Sha256Fingerprint(hostKey);
            String mapKey = host.ToLowerInvariant() + ":" + port;

            lock (Sync)
            {
                Dictionary<String, KnownHostEntry> entries = Load();
                KnownHostEntry stored;
                if (entries.TryGetValue(mapKey, out stored))
                {
                    if (String.Equals(stored.Sha256, fingerprint, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    throw new Exception(String.Format(
                        "SFTP host key for {0}:{1} has CHANGED (cached: {2}, presented: {3}). " +
                        "Possible man-in-the-middle attack. Remove the entry from {4} if the change is expected.",
                        host, port, stored.Sha256, fingerprint, StorePath));
                }

                entries[mapKey] = new KnownHostEntry
                {
                    KeyType = keyType,
                    Sha256 = fingerprint,
                    AddedUtc = DateTimeOffset.UtcNow.ToString("o"),
                };
                Save(entries);
                return true;
            }
        }

        private static Dictionary<String, KnownHostEntry> Load()
        {
            if (!File.Exists(StorePath))
            {
                return new Dictionary<String, KnownHostEntry>();
            }
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(Dictionary<String, KnownHostEntry>));
                using (var fs = File.OpenRead(StorePath))
                {
                    var result = serializer.ReadObject(fs) as Dictionary<String, KnownHostEntry>;
                    return result ?? new Dictionary<String, KnownHostEntry>();
                }
            }
            catch (Exception)
            {
                return new Dictionary<String, KnownHostEntry>();
            }
        }

        private static void Save(Dictionary<String, KnownHostEntry> entries)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StorePath));
            var serializer = new DataContractJsonSerializer(typeof(Dictionary<String, KnownHostEntry>));
            String temp = StorePath + ".tmp";
            using (var fs = File.Create(temp))
            {
                serializer.WriteObject(fs, entries);
            }
            File.Copy(temp, StorePath, true);
            File.Delete(temp);
        }
    }
}
