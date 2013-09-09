using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GeneralMethods
{
    public class Methods
    {
        public static string GetClientIpAddress ()
        {
            string Str = "";
            Str = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(Str);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }
    }
}
