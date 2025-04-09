targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention.')
param environmentName string

@description('Primary location for all resources.')
param location string

@description('Id of the principal to assign database and application roles.')
param principalId string = ''

// Optional parameters
param userAssignedIdentityName string = ''
param appServicePlanName string = ''
param appServiceWebAppName string = ''



// serviceName is used as value for the tag (azd-service-name) azd uses to identify deployment host
param serviceName string = 'web'

var abbreviations = loadJsonContent('abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = {
  'azd-env-name': environmentName
  
}

param completionModelName string = 'gpt-4o'
param completionDeploymentName string = 'gpt-4o'
  


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

module openAiAccount 'core/ai/cognitive-services/account.bicep' = {
  scope: resourceGroup
  name: 'openai-account'
  params: {
    accountname: '${abbreviations.openAiAccount}-${resourceToken}'
    location: location
    tags: tags
    kind: 'OpenAI'
    sku: 'S0'
    enablePublicNetworkAccess: true
    // add deployment for gpt-4o model


  }
}

module openAiModelDeployments 'core/ai/cognitive-services/deployment.bicep' = {
  scope: resourceGroup
  name: completionDeploymentName
  params: {
    name: completionDeploymentName
    parentAccountName: openAiAccount.outputs.name
    skuName: 'Standard'
    skuCapacity: 10
    modelName: completionModelName
    modelVersion: '2024-05-13'
    modelFormat: 'OpenAI'
    }
  }


module web 'app/web.bicep' = {
  name: 'web'
  scope: resourceGroup
  params: {
    appName: !empty(appServiceWebAppName) ? appServiceWebAppName : '${abbreviations.appServiceWebApp}-${resourceToken}'
    planName: !empty(appServicePlanName) ? appServicePlanName : '${abbreviations.appServicePlan}-${resourceToken}'
    openAiAccountEndpoint: openAiAccount.outputs.endpoint
    accountName: openAiAccount.outputs.name
    openAiSettings: {
      completionDeploymentName: openAiModelDeployments.name
      openAiApiKey: openAiAccount.outputs.apikey
    }

    userAssignedManagedIdentity: {
      resourceId: identity.outputs.resourceId
      clientId: identity.outputs.clientId
    }
    location: location
    tags: tags
    serviceTag: serviceName
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
output AZURE_OPENAI_ACCOUNT_ENDPOINT string = openAiAccount.outputs.endpoint
#disable-next-line outputs-should-not-contain-secrets // Deployment name is not a secret, although it gets flagged as one
output AZURE_OPENAI_COMPLETION_DEPLOYMENT_NAME string = openAiModelDeployments.outputs.name




