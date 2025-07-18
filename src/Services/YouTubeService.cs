using Azure;
using Azure.AI.OpenAI;
using System.Linq;
using YoutubeSummarizer.Configurations;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;
using System.Text;
using YoutubeExplode.Exceptions;

namespace YoutubeSummarizer.Services
{
    public interface IYouTubeService
    {
        Task<string> Summarize(string videoLink, string videoLanguage = "en", string summaryLanguage = "en");
    }

    public class YouTubeService : IYouTubeService
    {
        private readonly OpenAIClient _openAIClient;
        private readonly OpenAISettings _openAISettings;
        private readonly PromptSettings _promptSettings;

        public YouTubeService(
            OpenAIClient openAIClient,
            OpenAISettings openAISettings,
            PromptSettings promptSettings)
        {
            _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
            _openAISettings = openAISettings ?? throw new ArgumentNullException(nameof(openAISettings));
            _promptSettings = promptSettings ?? throw new ArgumentNullException(nameof(promptSettings));
        }

        public async Task<string> Summarize(string videoLink, string videoLanguage = "en", string summaryLanguage = "en")
        {
            if (string.IsNullOrEmpty(videoLink))
            {
                throw new ArgumentNullException(nameof(videoLink), "Video link cannot be null or empty.");
            }
            
            var videoId = YoutubeExplode.Videos.VideoId.Parse(videoLink);
            try
            {
                var subtitle = await GetSubtitle(videoId, videoLanguage);
                var summary = await GetSummary(subtitle, summaryLanguage);
                return summary;
            }
            catch (VideoUnavailableException)
            {
                return $"The video with ID '{videoId}' is unavailable from Azure's network. It may be private, age-restricted, or regionally blocked. Please try a different public video.";
            }
            catch (Exception ex)
            {
                return $"An unexpected error occurred: {ex.Message}";
            }
        }

        // --- THIS IS THE CORRECTED LINE ---
        // I have added "YoutubeExplode.Videos." before "VideoId"
        private async Task<string> GetSubtitle(YoutubeExplode.Videos.VideoId videoId, string videoLanguage)
        {
            var youtube = new YoutubeClient();

            var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId);
            var trackInfo = trackManifest.GetByLanguage(videoLanguage);

            if (trackInfo == null)
            {
                throw new InvalidOperationException($"Could not find subtitles for language '{videoLanguage}'. Please try another video.");
            }

            var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

            var subtitleBuilder = new StringBuilder();
            foreach (var caption in track.Captions)
            {
                subtitleBuilder.Append(caption.Text).Append(' ');
            }

            return subtitleBuilder.ToString();
        }

        private async Task<string> GetSummary(string subtitle, string summaryLanguage)
        {
            var deploymentName = _openAISettings.DeploymentName;
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                MaxTokens = _promptSettings.MaxTokens,
                Temperature = _promptSettings.Temperature,
                DeploymentName = deploymentName,
                Messages =
                {
                    new ChatMessage(ChatRole.System, $"You are a Youtube video summarizer. You will be given a transcript. Provide a summary in {summaryLanguage} in maximum 5 bullet points."),
                    new ChatMessage(ChatRole.User, subtitle)
                }
            };

            try
            {
                var summary = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
                return summary.Value.Choices[0].Message.Content;
            }
            catch (RequestFailedException ex) when (ex.Status == 429)
            {
                return "The request to OpenAI has exceeded the rate limit. Please retry after 60 seconds.";
            }
        }
    }
}