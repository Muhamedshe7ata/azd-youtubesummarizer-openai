targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention.')
param environmentName string

@minLength(1)
@allowed([
  'canadaeast'
  'eastus'
  'eastus2'
  'francecentral'
  'japaneast'
  'norwayeast'
  'polandcentral'
  'southindia'
  'swedencentral'
  'switzerlandnorth'
  'westus3'
])
@description('Primary location for all resources.')
param location string

@description('Id of the principal to assign database and application roles.')
param principalId string = ''

// Optional parameters
param openAiAccountName string = ''
param userAssignedIdentityName string = ''
param appServicePlanName string = ''
param appServiceWebAppName string = ''
param openAiApiKey string = ''

// serviceName is used as value for the tag (azd-service-name) azd uses to identify deployment host
param serviceName string = 'web'

var abbreviations = loadJsonContent('abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = {
  'azd-env-name': environmentName
  repo: 'https://github.com/AzureCosmosDB/cosmosdb-nosql-copilot'
}

var openAiSettings = {
  completionModelName: 'gpt-4o'
  completionDeploymentName: 'gpt-4o'
  
}


var principalType = 'User'

resource resourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: environmentName
  location: location
  tags: tags
}

module identity 'app/identity.bicep' = {
  name: 'identity'
  scope: resourceGroup
  params: {
    identityName: !empty(userAssignedIdentityName) ? userAssignedIdentityName : '${abbreviations.userAssignedIdentity}-${resourceToken}'
    location: location
    tags: tags
  }
}

module ai 'app/ai.bicep' = {
  name: 'ai'
  scope: resourceGroup
  params: {
    accountName: !empty(openAiAccountName) ? openAiAccountName : '${abbreviations.openAiAccount}-${resourceToken}'
    location: location
    completionModelName: openAiSettings.completionModelName
    completionsDeploymentName: openAiSettings.completionDeploymentName
    
    tags: tags
  }
}

module web 'app/web.bicep' = {
  name: 'web'
  scope: resourceGroup
  params: {
    appName: !empty(appServiceWebAppName) ? appServiceWebAppName : '${abbreviations.appServiceWebApp}-${resourceToken}'
    planName: !empty(appServicePlanName) ? appServicePlanName : '${abbreviations.appServicePlan}-${resourceToken}'
    openAiAccountEndpoint: ai.outputs.endpoint
    
    openAiSettings: {
      completionDeploymentName: ai.outputs.deployments[0].name
      embeddingDeploymentName: ai.outputs.deployments[1].name

    }

    userAssignedManagedIdentity: {
      resourceId: identity.outputs.resourceId
      clientId: identity.outputs.clientId
    }
    location: location
    tags: tags
    serviceTag: serviceName
    openAiApiKey: openAiApiKey
  }
}



module security 'app/security.bicep' = {
  name: 'security'
  scope: resourceGroup
  params: {
    appPrincipalId: identity.outputs.principalId
    userPrincipalId: !empty(principalId) ? principalId : null
    principalType: principalType
  }
}

// Web App outputs
output AZURE_WEB_APP_NAME string = '${abbreviations.appServiceWebApp}-${resourceToken}'
output AZURE_RESOURCE_GROUP_NAME string = resourceGroup.name

// AI outputs
output AZURE_OPENAI_ACCOUNT_ENDPOINT string = ai.outputs.endpoint
#disable-next-line outputs-should-not-contain-secrets // Deployment name is not a secret, although it gets flagged as one
output AZURE_OPENAI_COMPLETION_DEPLOYMENT_NAME string = ai.outputs.deployments[0].name
#disable-next-line outputs-should-not-contain-secrets // Deployment name is not a secret, although it gets flagged as one
output AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME string = ai.outputs.deployments[1].name
#disable-next-line outputs-should-not-contain-secrets // Explicitly using this to demo the usage of api key in the app settings. THIS IS NOT ACCEPTABLE FOR PRODUCTION ENVIRONMENTS.
output AZURE_OPENAI_KEY string = ai.outputs.apikey


