# Read variables from environment variables
$openaikey = $env:AZURE_OPENAI_KEY
$webappname = $env:AZURE_WEB_APP_NAME
$resourceGroupName = $env:AZURE_RESOURCE_GROUP_NAME


# Set the key in the Azure Web App configuration
az webapp config appsettings set --name $webappname --resource-group $resourceGroupName --settings "OPENAI__APIKEY=$openaikey" --output none

# Restart the Azure Web App to apply the new settings
az webapp restart --name $webappname --resource-group $resourceGroupName --output none