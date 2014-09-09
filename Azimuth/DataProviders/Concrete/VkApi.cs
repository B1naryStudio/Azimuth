using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
        private const string PublicId = "76704311";

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
        public async Task<List<VkTrackResponse.Audio>> GetSelectedTracks(DataForTrackSaving tracksInfo, string accessToken)
        {
            var result = new List<VkTrackResponse.Audio>();

            for (int offset = 0; offset < tracksInfo.TrackInfos.Count; offset += 100)
            {
                var url = BaseUri + "audio.getById?audios=";
                url = tracksInfo.TrackInfos.Skip(offset)
                    .Take(100)
                    .Aggregate(url, (current, track) => current + (track.OwnerId + "_" + track.ThirdPartId + ","));
                url = url.Remove(url.Length - 1);
                url += "&itunes=0&v=5.24&access_token=" + accessToken;

                var json = await _webClient.GetWebData(url);
                var tracks = JsonConvert.DeserializeObject<VkTrackResponse>(json);

                if (tracks.Response == null)
                {
                    var error = JsonConvert.DeserializeObject<ErrorData>(json);
                    if (error.Error == null && tracks.Response != null)
                    {
                        return tracks.Response;
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

                result.AddRange(tracks.Response);
            }

            return result;
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

        public async Task<string[]> GetTrackLyricByArtistAndName(string artist, string trackName, string accessToken, string userId)
        {
            const string emailRegular = "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
            const string websiteRegular = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            const string phoneNumberRegular = @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$";
            var searchUrl = BaseUri + "audio.search?q=" + artist + " " + trackName +
                      "&auto_complete=0&lyrics=1&sort=2&offset=0&v=5.24&access_token=" +
                      Uri.EscapeDataString(accessToken);

            var trackJson = await _webClient.GetWebData(searchUrl);
            var trackData = JsonConvert.DeserializeObject<TrackData>(trackJson);
            if (trackData.Response.Audios.Any())
            {
                foreach (var item in trackData.Response.Audios)
                {
                    var trackLyric = await GetLyricsById(userId, item.LyricsId, accessToken);
                    string[] separator = new string[] { " ", "(", ")", "\r\n", "\r", "\n" };
                    var splittedLyric = trackLyric.Split(separator, StringSplitOptions.None);

                    var notLyric =
                        splittedLyric.FirstOrDefault(
                            splittedItem =>
                                Regex.IsMatch(splittedItem, emailRegular) ||
                                Regex.IsMatch(splittedItem, websiteRegular) ||
                                Regex.IsMatch(splittedItem, phoneNumberRegular));
                    if (notLyric!=null)
                        continue;
                    else
                        return trackLyric.Split(new string[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
                }
            }
            return null;
        }

        public async Task<List<TrackData.Audio>> SearchTracksForLyric(List<TrackSearchInfo.SearchData> tracks, string accessToken)
        {
            var searchedTracks = new List<TrackData.Audio>();
            if (tracks != null)
            {
                foreach (var searchData in tracks)
                {
                    if (searchData != null)
                    {
                        var url = BaseUri + "audio.search?q=" + searchData.Artist + " " + searchData.Name +
                                  "&auto_complete=1&sort=2&offset=0&count=10&v=5.24&access_token=" + accessToken;

                        var trackJson = await _webClient.GetWebData(url);
                        var track = JsonConvert.DeserializeObject<TrackData>(trackJson);
                        var topTrack =
                            track.Response.Audios.FirstOrDefault(
                                author => author.Artist.ToLower() == searchData.Artist.ToLower());
                        if (topTrack != null)
                            searchedTracks.Add(topTrack);
                    }
                }
            }
            return searchedTracks;
 
     }


        public async Task<List<TrackData.Audio>> SearchTracks(string searchText, string accessToken, byte inUserTracks, int offset = 0, int count = 10)
        {   
            var url = BaseUri + "audio.search?q=" + searchText +
                      "&auto_complete=1&lyrics=0&performer_only=0&sort=2&search_own=" + inUserTracks + "&offset=" + offset + "&count=" + count + "&v=5.24&access_token=" + accessToken;
            var trackJson = await _webClient.GetWebData(url);
            var track = JsonConvert.DeserializeObject<TrackData>(trackJson);
            return track.Response.Audios;
        }

        public async Task<string> AddTrack(string id, string audioId, string accessToken)
        {
            string url = BaseUri + "audio.add?owner_id=" + id + "&audio_id=" + audioId + "&v=5.24&access_token=" +
                         accessToken;
            var createdAudioId = await _webClient.GetWebData(url);
            return createdAudioId;
        }

        public async Task<List<string>> GetTrackUrl(TrackSocialInfo tracks, string accessToken)
        {
            var url = tracks.Tracks.Aggregate(BaseUri + "audio.getById?audios=", (current, trackSocialInfo) => current + (trackSocialInfo.OwnerId + "_" + trackSocialInfo.ThirdPartId + ","));
            url = url.Remove(url.Length - 1);
            url += "&access_token=" + accessToken;
            var trackJson = await _webClient.GetWebData(url);
                var track = JsonConvert.DeserializeObject<VkTrackResponse>(trackJson).Response.Select(list => list.Url).ToList();
                return track;
        } 
    }
}