﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.DataProviders.Concrete
{
    public class VkApi : ISocialNetworkApi
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "https://api.vk.com/method/";
        private const int MaxCntTracksPerReq = 6000;

        public VkApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<List<TrackData.Audio>> GetTracks(string userId, string accessToken)
        {
            var tracks = new TrackData();
            int i = 0;
            int count = 0;

            //if user have more than 6000 tracks we must execute several req
            while ((i == 0) || (count > (i*MaxCntTracksPerReq)))
            {
                var url = BaseUri + "audio.get" +
                            "?owner_id=" + Uri.EscapeDataString(userId) +
                            "&need_user=0" + //without user_info
                            "&v=5.24" +
                            "&count=" + MaxCntTracksPerReq + //max count tracks for 1 req
                            "&offset=" + (MaxCntTracksPerReq*i) +
                            "&access_token=" + Uri.EscapeDataString(accessToken);

                var json = await _webClient.GetWebData(url);
                tracks = JsonConvert.DeserializeObject<TrackData>(json);

                if (tracks.Response != null)
                {
                    count = tracks.Response.Count;
                    i++;
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<ErrorData>(json);
                    if (error.Error == null)
                    {
                        return tracks.Response.Audios;
                    }
                    int code = error.Error.ErrorCode;
                    string message = error.Error.ErrorMessage;
                    switch (code)
                    {
                        case 1:
                            throw new UnknownErrorException(message, code);
                        case 2:
                            throw new ApplicationDisabledException(message, code);
                        case 4:
                            throw new IncorrectSignatureException(message, code);
                        case 5:
                            throw new UserAuthorizationException(message, code);
                        case 6:
                            throw new ManyRequestException(message, code);
                        case 100:
                            throw new BadParametersException(message, code);
                        case 201:
                            throw new AccessDeniedException(message, code);
                        default:
                            throw new VkApiException(message, code);
                    }
                }
            }
             
            return tracks.Response.Audios;
        }

        public async Task<List<TrackData.Audio>> GetSelectedTracks(string userId, List<string> trackIds, string accessToken)
        {
            var url = BaseUri + "audio.get" +
                      "?owner_id=" + userId +
                      "&audio_ids=";

            url = trackIds.Aggregate(url, (current, trackId) => current + (trackId + ","));
            url = url.Remove(url.Length - 1);
            url += "&need_user=0" +
                   "&count=" + MaxCntTracksPerReq +
                   "&v=5.24" +
                   "&access_token=" + accessToken;

            var json = await _webClient.GetWebData(url);
            var tracks = JsonConvert.DeserializeObject<TrackData>(json);

            if (tracks.Response == null)
            {
                var error = JsonConvert.DeserializeObject<ErrorData>(json);
                if (error.Error == null && tracks.Response != null)
                {
                    return tracks.Response.Audios;
                }
                int code = error.Error.ErrorCode;
                string message = error.Error.ErrorMessage;
                switch (code)
                {
                    case 1:
                        throw new UnknownErrorException(message, code);
                    case 2:
                        throw new ApplicationDisabledException(message, code);
                    case 4:
                        throw new IncorrectSignatureException(message, code);
                    case 5:
                        throw new UserAuthorizationException(message, code);
                    case 6:
                        throw new ManyRequestException(message, code);
                    case 100:
                        throw new BadParametersException(message, code);
                    default:
                        throw new VkApiException(message, code);
                }
            }

            return tracks.Response.Audios;
        }

        public async Task<string> GetLyricsById(string userId, long lyricsId, string accessToken)
        {
            var url = BaseUri + "audio.getLyrics" +
                      "?lyrics_id=" + Uri.EscapeDataString(lyricsId.ToString(CultureInfo.InvariantCulture)) +
                      "&v=5.24" +
                      "&access_token=" + Uri.EscapeDataString(accessToken);

            var json = JObject.Parse(await _webClient.GetWebData(url));
            return json["response"]["text"].ToString();
        }

        public async Task<List<VkFriendData.Friend>> GetFriendsInfo(string userId, string accessToken, int offset, int count)
        {
            var url = BaseUri + "friends.get" +
                      "?user_id=" + userId +
                      "&count=" + count +
                      "&offset=" + offset +
                      "&fields=screen_name,bdate,sex,city,country,photo_100" +
                      "&v=5.24" +
                      "&access_token=" + Uri.EscapeDataString(accessToken);
            var jsonData = await _webClient.GetWebData(url);
            var friends = JsonConvert.DeserializeObject<VkFriendData>(jsonData);
            if (friends.Response == null)
            {
                var error = JsonConvert.DeserializeObject<ErrorData>(jsonData);

                int code = error.Error.ErrorCode;
                string message = error.Error.ErrorMessage;
                switch (code)
                {
                    case 1:
                        throw new UnknownErrorException(message, code);
                    case 2:
                        throw new ApplicationDisabledException(message, code);
                    case 4:
                        throw new IncorrectSignatureException(message, code);
                    case 5:
                        throw new UserAuthorizationException(message, code);
                    case 6:
                        throw new ManyRequestException(message, code);
                    case 100:
                        throw new BadParametersException(message, code);
                    default:
                        throw new VkApiException(message, code);
                }
            }

            return friends.Response.Friends;
        }
    }
}