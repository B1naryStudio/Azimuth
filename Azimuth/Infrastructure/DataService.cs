using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Infrastructure
{
    public abstract class DataService : IDataService
    {
        public abstract Task<User> GetUserInfoAsync();

        protected static async Task<string> GetRequest(string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                var myResponse = (HttpWebResponse) await Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse, 
                    request.EndGetResponse,
                    null);
                var stream = myResponse.GetResponseStream();

                if (stream == null)
                    return String.Empty;

                var objReader = new StreamReader(stream);

                var sb = new StringBuilder();
                while (true)
                {
                    string line = objReader.ReadLine();
                    if (line != null) sb.Append(line);

                    else
                    {
                        return sb.ToString();
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}