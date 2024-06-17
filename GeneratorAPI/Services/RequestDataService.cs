using GeneratorAPI.Models.Common;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;
using GeneratorAPI.Services.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System.CodeDom;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using static System.Net.WebRequestMethods;

namespace GeneratorAPI.Services
{
    public partial class RequestDataService(IHttpClientWrapperService httpClient, ILoggerService logger, IConfiguration configuration) : IRequestDataService
    {
        private readonly IHttpClientWrapperService _httpClient = httpClient;
        private readonly ILoggerService _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private static readonly char[] separator = ['\n'];

        private async Task<object> RequestDataOpenAI(string apiUrl, object payload, List<HeadersModel>? headers = null)
        {
            try
            {
                await _logger.Log($"Connecting to {apiUrl}");

                var dataString = JsonConvert.SerializeObject(payload);

                await _logger.Log($"Payload to send: {dataString}");

                var content = new StringContent(dataString, Encoding.UTF8, "application/json");

                HttpRequestMessage httpRequestMessage = new()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(apiUrl),
                    Content = content
                };

                var clientResponse = await _httpClient.SendAsync(httpRequestMessage, headers);

                var apiRes = await clientResponse.Content.ReadAsStringAsync();

                await _logger.Log($"Remote API Result: {apiRes}");

                object response;

                if (clientResponse.StatusCode == HttpStatusCode.OK)
                    response = JsonConvert.DeserializeObject<OpenAISuccessResponse>(apiRes)!;
                else
                    response = JsonConvert.DeserializeObject<OpenAIErrorResponse>(apiRes)!;

                return response;
            }
            catch
            {
                throw;
            }
        }

        [GeneratedRegex(@"^\d+\.\s*")]
        private static partial Regex ResponseSeparator();

        public async Task<object> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body)
        {
            try
            {
                var headers = new List<HeadersModel> { new() { HeaderName = "Authorization", HeaderValue = "Bearer " + _configuration["GenerateYoutubeTitle:ApiKey"] } };
                var payload = new GenerateYoutubeTitleApiRequestModel
                {
                    Model = _configuration["GenerateYoutubeTitle:Model"],
                    Messages =
                    [
                        new Models.Request.Message
                        {
                            Role = _configuration["GenerateYoutubeTitle:Role"],
                            Content = $"Creating compelling YouTube titles that are {body.ContentType} about {body.Keywords} that draws viewers' attention and encourages them to click and watch your video."
                        }
                    ],
                    MaxTokens = int.Parse(_configuration["GenerateYoutubeTitle:MaxTokens"]!)
                };

                var result = await RequestDataOpenAI(_configuration["GenerateYoutubeTitle:URL"]!, payload, headers);

                if (result is OpenAISuccessResponse success)
                {
                    var rawResponse = success?.Choices?.FirstOrDefault()?.Message?.Content!;

                    string[] lines = rawResponse.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    List<string> response = [];

                    Regex regex = ResponseSeparator();

                    foreach (var line in lines)
                    {
                        string cleanedLine = regex.Replace(line, "").Trim();
                        cleanedLine = cleanedLine.Trim('"');
                        response.Add(cleanedLine);
                    }

                    return response;
                }
                else
                {
                    return ((OpenAIErrorResponse)result)?.Error?.Message!;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<object> YoutubeChannelFinder(YoutubeChannelFinderRequestModel body)
        {
            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _configuration["YoutubeChannelFinder:ApiKey"],
                    ApplicationName = "YouTubeChannelSearch"
                });

                var searchRequest = youtubeService.Search.List("snippet");
                searchRequest.Q = body.Query;
                searchRequest.Type = _configuration["YoutubeChannelFinder:Type"];
                searchRequest.MaxResults = int.Parse(_configuration["YoutubeChannelFinder:MaxResults"]!);

                var searchResponse = await searchRequest.ExecuteAsync();

                var channels = new List<YoutubeChannelFinderData>();

                // Iterate through search response items
                foreach (var searchResult in searchResponse.Items)
                {
                    // Ensure the item is a channel
                    if (searchResult.Id.Kind == "youtube#channel")
                    {
                        string channelId = searchResult.Id.ChannelId;

                        // Request channel details
                        var channelRequest = youtubeService.Channels.List("snippet,statistics");
                        channelRequest.Id = channelId;

                        // Execute channel details request asynchronously
                        var channelResponse = await channelRequest.ExecuteAsync();
                        var channel = channelResponse.Items.FirstOrDefault();

                        if (channel != null && channel.Statistics.SubscriberCount >= 200000
                            && channel.Statistics.ViewCount >= 500000)
                        {

                            // Create YoutubeChannelFinderData object with channel details
                            var result = new YoutubeChannelFinderData
                            {
                                ChannelName = channel.Snippet.Title,
                                ChannelUrl = $"https://www.youtube.com/channel/{channelId}",
                                ChannelThumbnail = channel.Snippet.Thumbnails.Default__.Url,
                                ChannelDescription = channel.Snippet.Description,
                                ChannelLocation = channel.Snippet.Country,
                                SubscriberCount = channel.Statistics?.SubscriberCount ?? 0,
                                ViewCount = channel.Statistics?.ViewCount ?? 0,
                                VideoCount = channel.Statistics?.VideoCount ?? 0
                            };

                            channels.Add(result);
                        }
                    }
                }

                // Sort channels by subscriber count (descending)
                channels = channels.OrderByDescending(c => c.SubscriberCount).ToList();


                var response = new YoutubeChannelFinderSuccessResponseModel()
                {
                    StatusCode = 200,
                    Data = channels
                };

                return response;
            }
            catch (Google.GoogleApiException e)
            {
                await _logger.Log($"Google API Error encountered: {e.Message}");

                var response = new YoutubeChannelFinderFailedResponseModel()
                {
                    StatusCode = (int)e.HttpStatusCode,
                    Error = e.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                await _logger.Log($"Error encountered: {ex.Message}");

                throw;  // Rethrow exception for further handling upstream
            }
        }

        public async Task<object> YoutubePopularVideos(YoutubePopularVideosRequestModel body)
        {
            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _configuration["YoutubePopularVideo:ApiKey"],
                    ApplicationName = "YoutubePopularVideo"
                });


                var searchRequest = youtubeService.Search.List("snippet");
                searchRequest.Q = body.Keyword;
                if (!String.IsNullOrEmpty(body.Region)) {
                    searchRequest.RegionCode = GetRegionCode(body.Region);
                }               
                searchRequest.Type = _configuration["YoutubePopularVideo:Type"];
                searchRequest.MaxResults = int.Parse(_configuration["YoutubePopularVideo:MaxResults"]!);

                var searchResponse = await searchRequest.ExecuteAsync();

                var videos = new List<YoutubePopularVideoData>();

                // Iterate through search response items
                foreach (var searchResult in searchResponse.Items)
                {
                    // Ensure the item is a channel
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        string videoId = searchResult.Id.VideoId;

                        // Request channel details
                        var videoRequest = youtubeService.Videos.List("snippet,statistics");
                        videoRequest.Id = videoId;

                        // Execute channel details request asynchronously
                        var videoResponse = await videoRequest.ExecuteAsync();
                        var video = videoResponse.Items.FirstOrDefault();

                        if (video != null && video.Statistics.LikeCount >= 200000
                            && video.Statistics.ViewCount >= 500000)
                        {

                            // Create YoutubeChannelFinderData object with channel details
                            var result = new YoutubePopularVideoData
                            {
                                VideoId = videoId,
                                Title = video.Snippet.Title,
                                Description = video.Snippet.Description,
                                ChannelId = video.Snippet.ChannelId,
                                ChannelUrl = $"https://www.youtube.com/watch?v={videoId}",
                                ChannelTitle = video.Snippet.ChannelTitle,
                                PublishedAt = video.Snippet.PublishedAt ?? DateTime.MinValue,
                                ThumbnailUrl = video.Snippet.Thumbnails.Default__.Url,
                                ViewCount = video.Statistics?.ViewCount,
                                LikeCount = video.Statistics?.LikeCount,
                                DislikeCount = video.Statistics?.DislikeCount,
                                CommentCount = video.Statistics?.CommentCount
                            };

                            videos.Add(result);
                        }
                    }
                }

                // Sort channels by subscriber count (descending)
                videos = videos.OrderByDescending(c => c.LikeCount).ToList();


                var response = new YoutubePopularVideoSuccessResponseModel()
                {
                    StatusCode = 200,
                    Data = videos
                };

                return response;
            }
            catch (Google.GoogleApiException e)
            {
                await _logger.Log($"Google API Error encountered: {e.Message}");

                var response = new YoutubeChannelFinderFailedResponseModel()
                {
                    StatusCode = (int)e.HttpStatusCode,
                    Error = e.Message
                };

                return response;
            }
            catch (Exception ex)
            {
                await _logger.Log($"Error encountered: {ex.Message}");

                throw;  // Rethrow exception for further handling upstream
            }
        }
        private string GetRegionCode(string regionName)
        {
            try
            {
                var regions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Afghanistan", "AF" },
                    { "Albania", "AL" },
                    { "Algeria", "DZ" },
                    { "American Samoa", "AS" },
                    { "Andorra", "AD" },
                    { "Angola", "AO" },
                    { "Anguilla", "AI" },
                    { "Antarctica", "AQ" },
                    { "Antigua and Barbuda", "AG" },
                    { "Argentina", "AR" },
                    { "Armenia", "AM" },
                    { "Aruba", "AW" },
                    { "Australia", "AU" },
                    { "Austria", "AT" },
                    { "Azerbaijan", "AZ" },
                    { "Bahamas", "BS" },
                    { "Bahrain", "BH" },
                    { "Bangladesh", "BD" },
                    { "Barbados", "BB" },
                    { "Belarus", "BY" },
                    { "Belgium", "BE" },
                    { "Belize", "BZ" },
                    { "Benin", "BJ" },
                    { "Bermuda", "BM" },
                    { "Bhutan", "BT" },
                    { "Bolivia", "BO" },
                    { "Bosnia and Herzegovina", "BA" },
                    { "Botswana", "BW" },
                    { "Brazil", "BR" },
                    { "British Indian Ocean Territory", "IO" },
                    { "Brunei Darussalam", "BN" },
                    { "Bulgaria", "BG" },
                    { "Burkina Faso", "BF" },
                    { "Burundi", "BI" },
                    { "Cabo Verde", "CV" },
                    { "Cambodia", "KH" },
                    { "Cameroon", "CM" },
                    { "Canada", "CA" },
                    { "Cayman Islands", "KY" },
                    { "Central African Republic", "CF" },
                    { "Chad", "TD" },
                    { "Chile", "CL" },
                    { "China", "CN" },
                    { "Colombia", "CO" },
                    { "Comoros", "KM" },
                    { "Congo", "CG" },
                    { "Congo (DRC)", "CD" },
                    { "Cook Islands", "CK" },
                    { "Costa Rica", "CR" },
                    { "Croatia", "HR" },
                    { "Cuba", "CU" },
                    { "Cyprus", "CY" },
                    { "Czech Republic", "CZ" },
                    { "Denmark", "DK" },
                    { "Djibouti", "DJ" },
                    { "Dominica", "DM" },
                    { "Dominican Republic", "DO" },
                    { "Ecuador", "EC" },
                    { "Egypt", "EG" },
                    { "El Salvador", "SV" },
                    { "Equatorial Guinea", "GQ" },
                    { "Eritrea", "ER" },
                    { "Estonia", "EE" },
                    { "Eswatini", "SZ" },
                    { "Ethiopia", "ET" },
                    { "Fiji", "FJ" },
                    { "Finland", "FI" },
                    { "France", "FR" },
                    { "Gabon", "GA" },
                    { "Gambia", "GM" },
                    { "Georgia", "GE" },
                    { "Germany", "DE" },
                    { "Ghana", "GH" },
                    { "Greece", "GR" },
                    { "Greenland", "GL" },
                    { "Grenada", "GD" },
                    { "Guam", "GU" },
                    { "Guatemala", "GT" },
                    { "Guinea", "GN" },
                    { "Guinea-Bissau", "GW" },
                    { "Guyana", "GY" },
                    { "Haiti", "HT" },
                    { "Honduras", "HN" },
                    { "Hong Kong", "HK" },
                    { "Hungary", "HU" },
                    { "Iceland", "IS" },
                    { "India", "IN" },
                    { "Indonesia", "ID" },
                    { "Iran", "IR" },
                    { "Iraq", "IQ" },
                    { "Ireland", "IE" },
                    { "Israel", "IL" },
                    { "Italy", "IT" },
                    { "Jamaica", "JM" },
                    { "Japan", "JP" },
                    { "Jordan", "JO" },
                    { "Kazakhstan", "KZ" },
                    { "Kenya", "KE" },
                    { "Kiribati", "KI" },
                    { "Kuwait", "KW" },
                    { "Kyrgyzstan", "KG" },
                    { "Laos", "LA" },
                    { "Latvia", "LV" },
                    { "Lebanon", "LB" },
                    { "Lesotho", "LS" },
                    { "Liberia", "LR" },
                    { "Libya", "LY" },
                    { "Liechtenstein", "LI" },
                    { "Lithuania", "LT" },
                    { "Luxembourg", "LU" },
                    { "Madagascar", "MG" },
                    { "Malawi", "MW" },
                    { "Malaysia", "MY" },
                    { "Maldives", "MV" },
                    { "Mali", "ML" },
                    { "Malta", "MT" },
                    { "Marshall Islands", "MH" },
                    { "Mauritania", "MR" },
                    { "Mauritius", "MU" },
                    { "Mexico", "MX" },
                    { "Micronesia", "FM" },
                    { "Moldova", "MD" },
                    { "Monaco", "MC" },
                    { "Mongolia", "MN" },
                    { "Montenegro", "ME" },
                    { "Montserrat", "MS" },
                    { "Morocco", "MA" },
                    { "Mozambique", "MZ" },
                    { "Myanmar", "MM" },
                    { "Namibia", "NA" },
                    { "Nauru", "NR" },
                    { "Nepal", "NP" },
                    { "Netherlands", "NL" },
                    { "New Zealand", "NZ" },
                    { "Nicaragua", "NI" },
                    { "Niger", "NE" },
                    { "Nigeria", "NG" },
                    { "North Korea", "KP" },
                    { "North Macedonia", "MK" },
                    { "Norway", "NO" },
                    { "Oman", "OM" },
                    { "Pakistan", "PK" },
                    { "Palau", "PW" },
                    { "Palestine", "PS" },
                    { "Panama", "PA" },
                    { "Papua New Guinea", "PG" },
                    { "Paraguay", "PY" },
                    { "Peru", "PE" },
                    { "Philippines", "PH" },
                    { "Poland", "PL" },
                    { "Portugal", "PT" },
                    { "Puerto Rico", "PR" },
                    { "Qatar", "QA" },
                    { "Romania", "RO" },
                    { "Russia", "RU" },
                    { "Rwanda", "RW" },
                    { "Saint Kitts and Nevis", "KN" },
                    { "Saint Lucia", "LC" },
                    { "Saint Vincent and the Grenadines", "VC" },
                    { "Samoa", "WS" },
                    { "San Marino", "SM" },
                    { "Sao Tome and Principe", "ST" },
                    { "Saudi Arabia", "SA" },
                    { "Senegal", "SN" },
                    { "Serbia", "RS" },
                    { "Seychelles", "SC" },
                    { "Sierra Leone", "SL" },
                    { "Singapore", "SG" },
                    { "Slovakia", "SK" },
                    { "Slovenia", "SI" },
                    { "Solomon Islands", "SB" },
                    { "Somalia", "SO" },
                    { "South Africa", "ZA" },
                    { "South Korea", "KR" },
                    { "South Sudan", "SS" },
                    { "Spain", "ES" },
                    { "Sri Lanka", "LK" },
                    { "Sudan", "SD" },
                    { "Suriname", "SR" },
                    { "Sweden", "SE" },
                    { "Switzerland", "CH" },
                    { "Syria", "SY" },
                    { "Taiwan", "TW" },
                    { "Tajikistan", "TJ" },
                    { "Tanzania", "TZ" },
                    { "Thailand", "TH" },
                    { "Timor-Leste", "TL" },
                    { "Togo", "TG" },
                    { "Tonga", "TO" },
                    { "Trinidad and Tobago", "TT" },
                    { "Tunisia", "TN" },
                    { "Turkey", "TR" },
                    { "Turkmenistan", "TM" },
                    { "Tuvalu", "TV" },
                    { "Uganda", "UG" },
                    { "Ukraine", "UA" },
                    { "United Arab Emirates", "AE" },
                    { "United Kingdom", "GB" },
                    { "United States", "US" },
                    { "Uruguay", "UY" },
                    { "Uzbekistan", "UZ" },
                    { "Vanuatu", "VU" },
                    { "Venezuela", "VE" },
                    { "Vietnam", "VN" },
                    { "Yemen", "YE" },
                    { "Zambia", "ZM" },
                    { "Zimbabwe", "ZW" }
                };

                if (regions.TryGetValue(regionName, out var code))
                {
                    return code;
                }
                return null;
            }
            catch (ArgumentException)
            {
                // Handle case where regionName is not a valid region name
                Console.WriteLine($"Invalid region name: {regionName}");
                return null; // or throw exception, log error, etc.
            }
        }

    }
}