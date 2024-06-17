namespace GeneratorAPI.Models.Response
{
    public class YoutubeChannelFinderSuccessResponseModel
    {
        public int StatusCode { get; set; }
        public List<YoutubeChannelFinderData>? Data { get; set; }
    }

    public class YoutubeChannelFinderData
    {
        public string? ChannelName { get; set; }
        public string? ChannelUrl { get; set; }
        public string? ChannelThumbnail { get; set; }
        public string? ChannelLocation { get; set; }
        public string? ChannelDescription { get; set; }
        public ulong SubscriberCount { get; set; }
        public ulong ViewCount { get; set; }
        public ulong VideoCount { get; set; }
    }
}
