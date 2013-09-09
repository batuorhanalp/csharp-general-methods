using System.Xml;
using System.Web;

namespace XMLProcess
{
    public class XMLProcess
    {
        public static string ReadXMLTag(string location, string xmlTag)
        {
            try
            {
                XmlDocument xSettings = new XmlDocument();
                string strServerMapPath = HttpContext.Current.Request.PhysicalApplicationPath;
                xSettings.Load(HttpContext.Current.Server.MapPath(location));
                XmlNode xSiteInfo = xSettings.DocumentElement;
                return xSiteInfo.SelectSingleNode(xmlTag).ChildNodes[0].Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
