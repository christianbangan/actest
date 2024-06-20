namespace GeneratorAPI.Models.Response
{
    public class YoutubePopularVideoSuccessResponseModel
    {
        public string? VideoId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ChannelId { get; set; }
        public string? VideoUrl { get; set; }
        public string? ChannelTitle { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? ThumbnailUrl { get; set; }
        public ulong? ViewCount { get; set; }
        public ulong? LikeCount { get; set; }
        public ulong? DislikeCount { get; set; }
        public ulong? CommentCount { get; set; }
    }
}