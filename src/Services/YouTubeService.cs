using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Azure;
using Azure.AI.OpenAI;
using System.Threading;
using System.Web;
using YoutubeSummarizer.Configurations;

namespace YoutubeSummarizer.Services;

    public interface IYouTubeService
    {
        Task<string> Summarize(string videoLink, string videoLanguage = "en", string summaryLanguage = "en");
    }
public class YouTubeService : IYouTubeService
{
    private readonly IYouTubeVideo _youTubeVideo;
    private readonly OpenAIClient _openAIClient;
    private readonly OpenAISettings _openAISettings;
    private readonly PromptSettings _promptSettings;
    public YouTubeService(
        IYouTubeVideo youTubeVideo,
        OpenAIClient openAIClient,
        OpenAISettings openAISettings,
        PromptSettings promptSettings)
    {
        _youTubeVideo = youTubeVideo ?? throw new ArgumentNullException(nameof(youTubeVideo));
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
        var subtitle = await _youTubeVideo.ExtractSubtitleAsync(videoUrl, videoLanguage);
        if (subtitle == null)
    {
        // Handle the null case, maybe log an error or return an empty string
        return string.Empty;
    }
        var aggregated = subtitle.Content?.Select(p => p.Text ?? string.Empty).Aggregate((a, b) => $"{a}\n{b}") ?? string.Empty;
        return aggregated;
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
                    //new ChatMessage(ChatRole.System, _promptSettings.System),
                    new ChatMessage(ChatRole.System, $"You are a Youtube video summarizer. Assume the source language of a transcript file is in English. You need to provide a summary in {summaryLanguage} in maximum 5 bullet points."),
                    new ChatMessage(ChatRole.User, subtitle)
                }
        };
        // create var summary

        try
        {
            var summary = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
            return summary.Value.Choices[0].Message.Content;
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            return "Requests to the ChatCompletions_Create Operation under Azure OpenAI API version 2023-09-01-preview have exceeded token rate limit of your current OpenAI S0 pricing tier. Please retry after 60 seconds.";
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            return $"Error: {ex.Message}";
        }


    }
}

