﻿using System;
using System.Threading.Tasks;
using TweetSharp;

namespace Azimuth.Infrastructure
{
    public interface IWebClient
    {
        Task<String> GetWebData(string url);
        Task<TwitterUser> GetWebData(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret);
    }
}