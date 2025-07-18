using Azure.AI.OpenAI;
using YoutubeSummarizer.Configurations;
using YoutubeSummarizer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var openAISettings = builder.Configuration.GetSection("OpenAI").Get<OpenAISettings>();
var promptSettings = builder.Configuration.GetSection("Prompt").Get<PromptSettings>();

builder.Services.AddSingleton(openAISettings);
builder.Services.AddSingleton(promptSettings);

// --- THIS IS THE CORRECTED LINE ---
builder.Services.AddSingleton(p => new OpenAIClient(new Uri(openAISettings.Endpoint), new Azure.AzureKeyCredential(openAISettings.ApiKey)));

builder.Services.AddScoped<IYouTubeService, YouTubeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<YoutubeSummarizer.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
