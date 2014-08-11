using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IVkApi
    {
        Task<List<VKTrackData>> GetUserTracks(string userId, string accessToken);
    }
}