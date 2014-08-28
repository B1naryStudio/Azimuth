
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class ErrorData
    {
        [JsonProperty(PropertyName = "error")]
        public VkError Error { get; set; }

        public class RequestParam
        {
            [JsonProperty(PropertyName = "key")]
            public string Key { get; set; }
            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }
        }

        public class VkError
        {
            [JsonProperty(PropertyName = "error_code")]
            public int ErrorCode { get; set; }
            [JsonProperty(PropertyName = "error_msg")]
            public string ErrorMessage { get; set; }
            [JsonProperty(PropertyName = "request_params")]
            public List<RequestParam> RequestParams { get; set; }
        }
    }
}
