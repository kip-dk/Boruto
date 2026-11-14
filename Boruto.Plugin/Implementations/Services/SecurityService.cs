using Boruto.Extensions.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Boruto.Implementations.Services
{
    public class SecurityService : ServiceAPI.ISecurityService
    {
        public SecurityService() 
        { 
        }

        public void SaveKey(string name, string key)
        {
            var folder = this.getFolder(name, false);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            };

            var file = Path.Combine(folder, $"{name}.key");

            var encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(key),null,DataProtectionScope.CurrentUser);
            File.WriteAllBytes(file, encrypted);
        }

        public string Decrypt(string keyname, string filename)
        {
            var file = Path.Combine(getFolder(keyname), filename);
            var key = this.getKey(keyname);

            var encrypted = File.ReadAllBytes(file);
            var decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);

            var decryptedTxt = Encoding.UTF8.GetString(decrypted);
            return decryptedTxt.Decrypt(key);
        }

        public void Encrypt(string keyname, string filename, string content)
        {
            var key = this.getKey(keyname);
            var _s1Content = content.Encrypt(key);

            var encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(_s1Content), null, DataProtectionScope.CurrentUser);
            var file = Path.Combine(getFolder(keyname), filename);
            File.WriteAllBytes(file, encrypted);
        }


        private string getKey(string keyname)
        {
            var folder = this.getFolder(keyname, true);

            var file = Path.Combine(folder, $"{keyname}.key");
            if (!File.Exists(file))
            {
                throw new ArgumentException($"Key file for {keyname} was not found");
            }

            var encrypted = File.ReadAllBytes(file);
            var decrypted = ProtectedData.Unprotect(encrypted, null,DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decrypted);
        }

        private string getFolder(string keyname, bool validate = true)
        {
            var res = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), keyname);
            if (validate && !Directory.Exists(res))
            {
                throw new ArgumentException($"Key file for {keyname} was not found");
            };

            Console.WriteLine(res);
            return res;
        }
    }
}
