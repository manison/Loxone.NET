using Rssdp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Loxone.Client
{
    public static class LoxoneMiniserverFinder
    {
        public static async Task<List<string>> SearchForDevices()
        {
            HashSet<string> miniservers = new HashSet<string>();

            using (var deviceLocator = new SsdpDeviceLocator())
            {
                var foundDevices = await deviceLocator.SearchAsync(); // Can pass search arguments here (device type, uuid). No arguments means all devices.

                foreach (var foundDevice in foundDevices)
                {
                    string xml = await downloadXml(foundDevice.DescriptionLocation.ToString());

                    if (String.IsNullOrEmpty(xml) == false)
                    {
                        if (xml.Contains("Loxone"))
                        {
                            miniservers.Add("http://" + foundDevice.DescriptionLocation.Host + "/");
                        }
                    }
                }
            }

            return miniservers.ToList();
        }

        private static async Task<string> downloadXml(string xmlPath)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, 5);

                var stringTask = client.GetStringAsync(xmlPath);

                return await stringTask;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
