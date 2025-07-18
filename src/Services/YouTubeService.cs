using Azure;
using Azure.AI.OpenAI;
using System.Linq;
using YoutubeSummarizer.Configurations;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;
using System.Text;
using YoutubeExplode.Exceptions; // <-- Added for specific error handling

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

            // The try-catch is now inside the Summarize method
            try
            {
                var subtitle = await GetSubtitle(videoLink, videoLanguage);
                var summary = await GetSummary(subtitle, summaryLanguage);
                return summary;
            }
            // THIS IS THE NEW, IMPORTANT PART
            catch (VideoUnavailableException)
            {
                // This will now show a friendly message instead of crashing.
                return "The requested video is unavailable. It may be private, age-restricted, or blocked in your region. Please try a different video.";
            }
            catch (Exception ex) // Catch any other unexpected errors
            {
                return $"An unexpected error occurred: {ex.Message}";
            }
        }

        private async Task<string> GetSubtitle(string videoUrl, string videoLanguage)
        {
            var youtube = new YoutubeClient();
            var videoId = YoutubeExplode.Videos.VideoId.Parse(videoUrl);

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
                    new ChatMessage(ChatRole.System, $"You are a Youtube video summarizer. Assume the source language of a transcript file is in English. You need to provide a summary in {summaryLanguage} in maximum 5 bullet points."),
                    new ChatMessage(ChatRole.User, subtitle)
                }
            };
            
            // Note: The try-catch for OpenAI-specific errors is kept here
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