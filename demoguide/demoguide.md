[comment]: <> (please keep all comment items at the top of the markdown file)
[comment]: <> (please do not change the ***, as well as <div> placeholders for Note and Tip layout)
[comment]: <> (please keep the ### 1. and 2. titles as is for consistency across all demoguides)
[comment]: <> (section 1 provides a bullet list of resources + clarifying screenshots of the key resources details)
[comment]: <> (section 2 provides summarized step-by-step instructions on what to demo)


[comment]: <> (this is the section for the Note: item; please do not make any changes here)
***
### Azure OpenAI Service with YoutubeSummarizer Blazor sample app - demo scenario

<div style="background: lightgreen; 
            font-size: 14px; 
            color: black;
            padding: 5px; 
            border: 1px solid lightgray; 
            margin: 5px;">

**Note:** Below demo steps should be used **as a guideline** for doing your own demos. Please consider contributing to add additional demo steps.
</div>

[comment]: <> (this is the section for the Tip: item; consider adding a Tip, or remove the section between <div> and </div> if there is no tip)

<div style="background: lightblue; 
            font-size: 14px; 
            color: black;
            padding: 5px; 
            border: 1px solid lightgray; 
            margin: 5px;">

</div>

***
### 1. What Resources are getting deployed
This scenario deploys **an Azure Web App, showcasing a Youtube Summarizer app, allowing users to provide a Youtube video link, select their preferred language and get a summmarized bullet list back from the Azure OpenAI Chat Completion.   

This scenario is based on 4 Azure Resources:
* rg-%azdenvname% - Azure Resource Group.
* openai-%guid% - The Azure OpenAI Resource with GPT-4o model deployed
* plan-%guid% - Azure App Service Plan, used by the Azure App Service
* ua-id-%guid% - User Managed Identity, used by the Azure App Service
* web-%guid% - Azure App Service, running the Blazor .NET YoutubeSummarizer sample web application

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/ResourceGroup_Overview.png" alt="Youtube Summarizer Resource Group" style="width:70%;">
<br></br>

### 2. What can I demo from this scenario after deployment

I think it is a powerful approach to first show the capability of the app, before digging into the actual Azure Resources behind the scenes.

#### 2a. Using the sample web app

1. Once the deployment is complete, navigate to the **web app URL**, and open it in your browser window.

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/YoutubeSummarizer_home.png" alt="Youtube Summarizer Home Page" style="width:70%;">
<br></br>

1. Provide a **link to a Youtube video** (Note: the video should be in English and provide Closed Captions)
1. Select the **target language** you want to use for the summary
1. Click the **Summary** button. 
1. A spinner appears, together with an informative message, saying it is communicating with the OpenAI Service.

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/Trigger_Summary.png" alt="Complete all fields" style="width:70%;">
<br></br>

1. Wait for the process to complete, after which a summary in 5 bullet point sentences will be provided for the video, in the language output of choice.

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/Summary_in_choosen_language.png" alt="Summary Results in foreign language" style="width:70%;">
<br></br>

#### 2b. Azure OpenAI Service

1. From the Azure Portal, navigate to the Resource Group for this scenario.
1. Select the OpenAI Resource.

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/OpenAI_ResourceBlade.png" alt="OpenAI Resource Blade" style="width:70%;">
<br></br>

1. From the Left Side Menu, **open several sections**, such as Resource Management, Security, Monitoring,... to highlight this is just like most other Azure Resources.
1. From the **Overview** tab, select **Endpoints**. This opens the **Keys and Endpoints** section, showing the different API-Keys, as well as the actual URL Endpoint for this OpenAI Service. These are 2 parameters the web app needs to function correctly.

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/OpenAI_ResourceBlade.png" alt="OpenAI Resource Blade" style="width:70%;">
<br></br>

1. **Select** the OpenAI Resource, and from the **Overview** blade, highlight **"Explore Azure AI Foundry Portal"**.
1. Click the button, which redirects you to the **Azure AI Foundry** portal, allowing you to interact with the different AI Services building blocks for your AI applications. 

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/AI_Foundry.png" alt="Explore AI Foundry Portal" style="width:70%;">
<br></br>

1. From **Shared Resources**, select **Deployments**, which will show you the **GPT-4o** model we deployed as part of the scenario.  

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/gpt-4o_deployedmodel.png" alt="GPT-4o Deployed Model" style="width:70%;">
<br></br>

1. Next, navigate to **Playground** and select **Chat**. This is the main place where a developer would validate the working of the selected GPT model, as well as fine-tune the System Message. Notice how - in this scenario - the System Message is still the default, generic one provided by AI Foundry. However, the YoutubeSummarizer application is not using the default message, but got a customized one, defined as part of the application source code (see later).

<img src="https://raw.githubusercontent.com/petender/azd-youtubesummarizer-openai/main/demoguide/chat_playground.png" alt="Chat Playground" style="width:70%;">
<br></br>

#### Application Code view [Optional demo, for developer audience]

Assuming you deployed this demo scenario through AZD, know you have the full source code of the application on your local machine. Perfect to walk developer learners through some code snippets.

You find the full code in the scenario subfolder, in the **/src** location.

[comment]: <> (this is the closing section of the demo steps. Please do not change anything here to keep the layout consistant with the other demoguides.)
<br></br>
***
<div style="background: lightgray; 
            font-size: 14px; 
            color: black;
            padding: 5px; 
            border: 1px solid lightgray; 
            margin: 5px;">

**Note:** This is the end of the current demo guide instructions.
</div>



