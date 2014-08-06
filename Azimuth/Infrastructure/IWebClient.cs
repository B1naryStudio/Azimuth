using System;
using System.Threading.Tasks;

namespace Azimuth.Infrastructure
{
    public interface IWebClient
    {
        Task<String> GetWebData(string url);
    }
}