## Azure OpenAI Service with Blazor YoutubeSummarizer sample app

This repo contains a demo for an Azure OpenAI Service, together with a Blazor "Youtube Summarizer" sample app. By providing a Youtube URL and selecting your preferred language, a request is sent to the Azure OpenAI Service, after which the ChatCompletion provides a 5-bullet point summary of the video in your selected language.

<p>The app is inspired by <a href="https://twitter.com/justinchronicle">Justin Yoo, Principal Cloud Advocate at Microsoft</a>, who presented <a href="https://learn.microsoft.com/en-us/shows/dotnetconf-2023/building-intelligent-applications-with-blazor-and-open-ai-service" target="_blank">Building intelligent applications with Blazor and Open AI Service </a>, at the <a href="https://learn.microsoft.com/en-us/shows/dotnetconf-2023/">2023 .NET Conference</a>. His idea allowed me to learn more about how to develop Azure OpenAI-based applications in Blazor.

This scenario can be deployed to Azure using the [Azure Developer CLI - AZD](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/overview). 

💪 This template scenario is part of the larger **[Microsoft Trainer Demo Deploy Catalog](https://aka.ms/trainer-demo-deploy)**.

## ⬇️ Installation
- [Azure Developer CLI - AZD](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/install-azd)
    - When installing AZD, the above the following tools will be installed on your machine as well, if not already installed:
        - [GitHub CLI](https://cli.github.com)
        - [Bicep CLI](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install)
    - You need Owner or Contributor access permissions to an Azure Subscription to  deploy the scenario.

## 🚀 Deploying the scenario in 3 easy steps:

1. From within a new folder on your machine, run `azd init` to initialize the deployment.
```
azd init -t petender/azd-youtubesummarizer-openai
```
2. Next, run `azd up` to trigger an actual deployment.
```
azd up
```
3. If you want to delete the scenario from your Azure subscription, use `azd down`
```
azd down --purge --force
```

⏩ Note: running `azd down` deletes the RG and Resources, but will keep the artifacts on your local machine.

## What is the demo scenario about?

- Use the [demo guide]([insert raw link to the demoguide within your repo](https://github.com/petender/azd-youtubesummarizer-openai/blob/main/demoguide/demoguide.md)) for inspiration for your demo

## 💭 Feedback and Contributing
Feel free to create issues for bugs, suggestions or Fork and create a PR with new demo scenarios or optimizations to the templates. 
If you like the scenario, consider giving a GitHub ⭐
 

