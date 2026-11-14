using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.ServiceAPI
{
    public interface ISecurityService
    {
        void SaveKey(string name, string key);
        void Encrypt(string keyname, string filename, string content);
        string Decrypt(string keyname, string filename);

    }
}
