﻿using GeneratorAPI.Models.Common;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;
using GeneratorAPI.Services.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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

        private async Task<object> CallOpenAIAPI(object body, string config)
        {
            try
            {
                var headers = new List<HeadersModel> { new() { HeaderName = "Authorization", HeaderValue = "Bearer " + _configuration[$"{config}:ApiKey"] } };

                Models.Request.Message message = new()
                {
                    Role = _configuration[$"{config}:Role"]
                };

                if (body is GenerateYoutubeTitleRequestModel ytTitleParam)
                    message.Content = $"Creating compelling YouTube titles that are {ytTitleParam.ContentType} about {ytTitleParam.Keywords} that draws viewers' attention and encourages them to click and watch your video.";
                else if (body is HookGeneratorRequestModel hookParam)
                    message.Content = $"If my idea is about {hookParam.Idea} and the content-type is {hookParam.ContentType}, Generate the following 1. Intriguing Question, 2. Visual Imagery 3. Quotation. Make it look like crafted with the precision of a seasoned digital marketer, this hook is designed to captivate attention across all types of content. Make it as  json response with property names: intriguing_question, visual_imagery, quotation.";

                var payload = new GenerateYoutubeTitleApiRequestModel
                {
                    Model = _configuration[$"{config}:Model"],
                    Messages =
                    [
                        message
                    ],
                    MaxTokens = int.Parse(_configuration[$"{config}:MaxTokens"]!)
                };

                var result = await RequestDataOpenAI(_configuration[$"{config}:URL"]!, payload, headers);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<object> GenerateYoutubeTitle(GenerateYoutubeTitleRequestModel body)
        {
            try
            {
                var result = await CallOpenAIAPI(body, "GenerateYoutubeTitle");

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
                if (!String.IsNullOrEmpty(body.Region))
                {
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

                        if (video != null && video.Statistics.LikeCount >= 500000
                            && video.Statistics.ViewCount >= 800000)
                        {
                            // Create YoutubeChannelFinderData object with channel details
                            var result = new YoutubePopularVideoData
                            {
                                VideoId = videoId,
                                Title = video.Snippet.Title,
                                Description = video.Snippet.Description,
                                ChannelId = video.Snippet.ChannelId,
                                VideoUrl = $"https://www.youtube.com/watch?v={videoId}",
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
                    { "Argentina", "AR" },
                    { "Australia", "AU" },
                    { "Austria", "AT" },
                    { "Belgium", "BE" },
                    { "Brazil", "BR" },
                    { "Canada", "CA" },
                    { "Chile", "CL" },
                    { "Colombia", "CO" },
                    { "Czech Republic", "CZ" },
                    { "Denmark", "DK" },
                    { "Egypt", "EG" },
                    { "Finland", "FI" },
                    { "France", "FR" },
                    { "Germany", "DE" },
                    { "Greece", "GR" },
                    { "Hong Kong", "HK" },
                    { "Hungary", "HU" },
                    { "India", "IN" },
                    { "Indonesia", "ID" },
                    { "Ireland", "IE" },
                    { "Israel", "IL" },
                    { "Italy", "IT" },
                    { "Japan", "JP" },
                    { "Jordan", "JO" },
                    { "Kenya", "KE" },
                    { "Kuwait", "KW" },
                    { "Malaysia", "MY" },
                    { "Mexico", "MX" },
                    { "Morocco", "MA" },
                    { "Netherlands", "NL" },
                    { "New Zealand", "NZ" },
                    { "Nigeria", "NG" },
                    { "Norway", "NO" },
                    { "Oman", "OM" },
                    { "Pakistan", "PK" },
                    { "Peru", "PE" },
                    { "Philippines", "PH" },
                    { "Poland", "PL" },
                    { "Portugal", "PT" },
                    { "Qatar", "QA" },
                    { "Romania", "RO" },
                    { "Russia", "RU" },
                    { "Saudi Arabia", "SA" },
                    { "Senegal", "SN" },
                    { "Singapore", "SG" },
                    { "South Africa", "ZA" },
                    { "South Korea", "KR" },
                    { "Spain", "ES" },
                    { "Sweden", "SE" },
                    { "Switzerland", "CH" },
                    { "Taiwan", "TW" },
                    { "Tanzania", "TZ" },
                    { "Thailand", "TH" },
                    { "Tunisia", "TN" },
                    { "Turkey", "TR" },
                    { "Uganda", "UG" },
                    { "Ukraine", "UA" },
                    { "United Arab Emirates", "AE" },
                    { "United Kingdom", "GB" },
                    { "United States", "US" },
                    { "Vietnam", "VN" },
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

        public async Task<object> HookGenerator(HookGeneratorRequestModel body)
        {
            try
            {
                var result = await CallOpenAIAPI(body, "HookGenerator");

                if (result is OpenAISuccessResponse success)
                {
                    var rawResponse = success?.Choices?.FirstOrDefault()?.Message?.Content!;

                    string content = string.Empty;

                    int startIndex = rawResponse.IndexOf('{');

                    int endIndex = rawResponse.LastIndexOf('}');

                    if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                    {
                        string jsonContent = rawResponse.Substring(startIndex, endIndex - startIndex + 1);

                        content = jsonContent.Trim();
                        content = content.Replace("\n", "");
                    }

                    var response = JsonConvert.DeserializeObject<HookGeneratorApiResponseModel>(content);

                    return response!;
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
    }
}