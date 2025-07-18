using Azure;
using Azure.AI.OpenAI;
using System.Linq; // <-- THIS WAS THE MISSING LINE THAT CAUSED THE ERROR
using YoutubeSummarizer.Configurations;
using YoutubeExplode;
using YoutubeExplode.Videos.ClosedCaptions;

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

            var subtitle = await GetSubtitle(videoLink, videoLanguage);
            var summary = await GetSummary(subtitle, summaryLanguage);
            return summary;
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
            var fullText = string.Join(" ", track.Captions.Select(c => c.Text));

            return fullText;
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

            try
            {
                var summary = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
                return summary.Value.Choices[0].Message.Content;
            }
            catch (RequestFailedException ex) when (ex.Status == 429)
            {
                return "Requests have exceeded the token rate limit of your current OpenAI pricing tier. Please retry after 60 seconds.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}