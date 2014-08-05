using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.Infrastructure
{
    public class TwitterAccountProvider: AccountProvider
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;
        private readonly string _userId;

        public string UserInfoUrl { get; set; }

        public TwitterAccountProvider(string userId, string accessToken)
        {
            this._accessToken = accessToken;
            this._userId = userId;

            UserInfoUrl = String.Format(@"https://api.twitter.com/1.1/users/show.json?user_id={0}", _userId, accessToken);







            	
        //https://api.twitter.com/1/users/show.json?screen_name=TwitterAPI&include_entities=true
            // https://api.twitter.com/1.1/users/show.json?user_id={0}
        }
        public override async Task<User> GetUserInfoAsync(string email = "")
        {

            // oauth application keys
            var oauth_token = "2697661404-9fytb8S1s5ajfHm6nfBWXxImCT7O9lJ16A95eTX";
            var oauth_token_secret = "GJYqxBrhtwJkIfO2aSdRkxccv71sPczAwYKXJcVAb30ei";
            var oauth_consumer_key = "WUOz1dJWadM5NSUmgMrcPgiIa";
            var oauth_consumer_secret = "9tO77dgpGcQuve4MDf0ZTKuHY3TVw8QLpjRTCTxDXh9vJpQXyc";

            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";

            // unique request details
            var oauth_nonce = Convert.ToBase64String(
                new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow
                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // message api details
            //var status = "Updating status via REST API 1.1";
            var resource_url = "https://api.twitter.com/1.1/followers/list.json";
            //var resource_url = "https://api.twitter.com/1.1/users/show.json?user_id=" + _userId;

            // create oauth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version
                //,Uri.EscapeDataString(status)
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );


            // make the request
            // var postBody = "status=" + Uri.EscapeDataString(status);

            ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            //using (Stream stream = request.GetRequestStream())
            //{
            //    byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
            //    stream.Write(content, 0, content.Length);
            //}
            WebResponse response = request.GetResponse();
            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();





























            string URL = "http://api.twitter.com/1/users/show.json?screen_name=" + "AzimuthP";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL);
            try
            {
                var webResponse = (HttpWebResponse) webRequest.GetResponse();
            }
            catch (HttpException ex)
            {
                throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }





            var userDataJson = await GetRequest(UserInfoUrl);
            var userDataObject = JObject.Parse(userDataJson);
            var userData = JsonConvert.DeserializeObject<TwitterUserData>(userDataJson);

            return new User();
        }
    }
}