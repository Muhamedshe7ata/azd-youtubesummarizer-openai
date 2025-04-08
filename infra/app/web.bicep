metadata description = 'Create web apps.'

param planName string
param appName string
param serviceTag string
param location string = resourceGroup().location
param tags object = {}

@description('SKU of the App Service Plan.')
param sku string = 'B3'


param openAiApiKey string 

@description('Endpoint for Azure OpenAI account.')
param openAiAccountEndpoint string

type openAiOptions = {
  completionDeploymentName: string
  embeddingDeploymentName: string

}

@description('Application configuration settings for OpenAI.')
param openAiSettings openAiOptions


//@description('Application configuration settings for Azure Cosmos DB.')
//param cosmosDbSettings cosmosDbOptions





type managedIdentity = {
  resourceId: string
  clientId: string
}

@description('Unique identifier for user-assigned managed identity.')
param userAssignedManagedIdentity managedIdentity

module appServicePlan '../core/host/app-service/plan.bicep' = {
  name: 'app-service-plan'
  params: {
    name: planName
    location: location
    tags: tags
    sku: sku
    kind: 'linux'
  }
}

module appServiceWebApp '../core/host/app-service/site.bicep' = {
  name: 'app-service-web-app'
  params: {
    name: appName
    location: location
    tags: union(tags, {
      'azd-service-name': serviceTag
    })
    parentPlanName: appServicePlan.outputs.name
    runtimeName: 'dotnetcore'
    runtimeVersion: '8.0'
    kind: 'app,linux'
    enableSystemAssignedManagedIdentity: false
    userAssignedManagedIdentityIds: [
      userAssignedManagedIdentity.resourceId
    ]
  }
}

module appServiceWebAppConfig '../core/host/app-service/config.bicep' = {
  name: 'app-service-config'
  params: {
    parentSiteName: appServiceWebApp.outputs.name
    appSettings: {
      WEBSITE_RUN_FROM_PACKAGE: '1' //used to speed up deployment with no app runtime impact
      OPENAI__ENDPOINT: openAiAccountEndpoint
      OPENAI__COMPLETIONDEPLOYMENTNAME: openAiSettings.completionDeploymentName
      OPENAI__APIKEY: openAiApiKey
      OPENAI__DEPLOYMENTNAME: openAiSettings.completionDeploymentName
      AZURE_CLIENT_ID: userAssignedManagedIdentity.clientId
      PROMPT__SYSTEM: 'You are an AI assistant which is used to summarize YouTube videos'
      PROMPT__TEMPERATURE: '0.7'
      PROMPT__MAXTOKENS: '3000'
    }
  }
}

output name string = appServiceWebApp.outputs.name
output endpoint string = appServiceWebApp.outputs.endpoint
