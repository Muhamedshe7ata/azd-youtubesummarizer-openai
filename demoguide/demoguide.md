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
This scenario deploys **an Azure Storage Account**, having blobs and file shares as main data storage scenarios, displaying Seattle scenery images.   

* rg-%azdenvname% - Azure Resource Group.
* id-Script-%guid% - Azure Managed Identity to run the file copy script
* id-upload-%guid% - Azure Managed Identity to upload files into blobs and file share
* %guid%storageaccount - Azure Storage Account, used for Blobs and File Share services
* pwscript-uploadDataScript - PowerShell script to upload files into blobs and file share

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/ResourceGroup_Overview.png" alt="Stor Account Resource Group" style="width:70%;">
<br></br>

### 2. What can I demo from this scenario after deployment

#### 2a. Blob Storage Demo

1. In this scenario, the focus is highlighting the main features of an Azure Storage Account, being Blob storage.
1. Navigate to the **Storage Account**, and open the **Overview** tab.
1. Highlight the **Replication** and **Account Kind** settings.
1. Navigate to **Storage Browser**. Select **Blob Containers**.
1. Open the **images** blob.
1. Notice there are **6 images files** in here. Select one of the image files.
1. From the detailed blade, select URL and copy/paste this into a new browser window.
1. The file will be downloaded in the browser; Open the file to see the actual Seattle scenery image.

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/blob_image_URL.png" alt="Blob Image" style="width:70%;">
<br></br>

**Note:** The fact the image can be downloaded just by connecting to the URL, is because of the **blob anonymous access level** as well as **storage account public access**.
</div>

1. Return to the Blob container view, select **image-01.jpg** and click the **Change Access Level** button. 
1. Switch from **Blob (anonymous read access for blobs only) to **Private (no anonymous access).

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/blob_access_level.png" alt="Blob Access Levels" style="width:70%;">
<br></br>

1. Select **image-01.jpg** to open its Overview blade. 
1. Copy the URL into a new browser tab.
1. Notice, the image file is no longer available for download, but an <Resource Not Found> error message appears.

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/resourcenotfound.png" alt="Resource Not Found error" style="width:70%;">
<br></br>

1. From the Storage Account resource, navigate to **Settings / Configuration **.
1. Notice the **Allow Blob Anonymous Access**, currently set as **Enabled**.
1. Switch this setting to **Disabled**.
1. Save the changes.
1. Return to the Storage Browser view, images blob folder. 
1. Switch **Access Level** back to **Allow Blob Anonymous Access**.
1. Notice the error message on screen, saying this setting cannot be enabled, as the Storage Account does not allow Public Access. 

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/blob_no_anonymous_accesslevel.png" alt="Resource Not Found error" style="width:70%;">
<br></br>

#### 2b. File Share Storage Demo

1. In this scenario, the focus is highlighting some of the additonal features of an Azure Storage Account, in this case, File Share storage.
1. Navigate to **Storage Browser**. Select **File Share**.
1. Open the **fileshare** File Share.
1. Notice there are **6 images files** in here. Select one of the image files.
1. From the detailed blade, select URL and copy/paste this into a new browser window.
1. Notice, the image file is not available for download, but an <InvalidHeader> error message appears.

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/invalidheader.png" alt="Resource Not Found error" style="width:70%;">
<br></br>

1. From the Storage Browser view, navigate to **File Share**.
1. From the right hand view, select **fileshare**; notice the **Connect** menu item becomes available.
1. Depending on your client Operating System, the steps might be slightly different. For this guide, I assume you are using a Windows OS client.
1. From the **Windows** Tab, accept the default **Drive Letter** and **Storage Account Key** settings.
1. Click the **Show Script** button.

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/smb_connect.png" alt="SMB Connect Settings" style="width:70%;">
<br></br>

1. Copy the script in a **PowerShell** Terminal, and run it.
1. The script will first validate connectivity to SMB Port 445.

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/test_connection.png" alt="SMB Port 445 connection test" style="width:70%;">
<br></br>

1. If the connectivity is successful, it will map the fileshare to the Drive Letter defined earlier. In my case, this was the Z-drive.

<img src="https://raw.githubusercontent.com/petender/azd-storaccnt/main/Demoguides/z_drive.png" alt="Z-Drive mapped" style="width:70%;">
<br></br>

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



