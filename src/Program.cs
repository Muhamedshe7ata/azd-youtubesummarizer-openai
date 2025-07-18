using Azure.AI.OpenAI;
using YoutubeSummarizer.Configurations;
using YoutubeSummarizer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// This section configures settings from your appsettings.json
var openAISettings = builder.Configuration.GetSection("OpenAI").Get<OpenAISettings>();
var promptSettings = builder.Configuration.GetSection("Prompt").Get<PromptSettings>();

// This sets up the configuration objects to be used elsewhere
builder.Services.AddSingleton(openAISettings);
builder.Services.AddSingleton(promptSettings);

// This sets up the official Azure OpenAI client
builder.Services.AddSingleton(p => new OpenAIClient(new Uri(openAISettings.Endpoint), new Azure.AzureKeyCredential(openAISettings.Key)));

// This is the corrected line.
// It registers your YouTubeService correctly without the old, broken library.
builder.Services.AddScoped<IYouTubeService, YouTubeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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