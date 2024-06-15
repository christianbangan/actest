﻿using GeneratorAPI.Models.Common;
using GeneratorAPI.Models.Request;
using GeneratorAPI.Models.Response;
using GeneratorAPI.Services.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System.CodeDom;
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
                            Content = $"Generate 10 {body.ContentType} Youtube Titles about {body.Keywords}"
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

                var response = new YoutubeChannelFinderSuccessResponseModel()
                {
                    StatusCode = 200,
                    Data = []
                };

                foreach (var item in searchResponse.Items)
                {
                    if (item.Id.Kind == "youtube#channel")
                    {
                        string channelId = item.Id.ChannelId;

                        var channelRequest = youtubeService.Channels.List("snippet,statistics");
                        channelRequest.Id = channelId;

                        var channelResponse = await channelRequest.ExecuteAsync();
                        var channel = channelResponse.Items[0];

                        var result = new YoutubeChannelFinderData
                        {
                            ChannelName = channel.Snippet.Title,
                            ChannelUrl = $"https://www.youtube.com/channel/{channelId}",
                            ChannelThumbnail = channel.Snippet.Thumbnails.Default__.Url,
                            ChannelDescription = channel.Snippet.Description,
                            ChannelLocation = channel.Snippet.Country,
                            SubscriberCount = channel.Statistics.SubscriberCount ?? 0,
                            ViewCount = channel.Statistics.ViewCount ?? 0,
                            VideoCount = channel.Statistics.VideoCount ?? 0
                        };

                        response.Data.Add(result);
                    }
                }

                return response!;
            }
            catch (Google.GoogleApiException e)
            {
                await _logger.Log($"Error encountered: {e.Message}");

                YoutubeChannelFinderFailedResponseModel response = new()
                {
                    StatusCode = (int)e.HttpStatusCode,
                    Error = e.Message
                };

                return response;
            }
            catch
            {
                throw;
            }
        }
    }
}