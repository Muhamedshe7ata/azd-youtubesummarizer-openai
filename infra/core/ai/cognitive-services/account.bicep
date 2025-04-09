metadata description = 'Creates an Azure Cognitive Services account.'

param accountname string
param location string = resourceGroup().location
param tags object = {}

@allowed([ 'OpenAI', 'ComputerVision', 'TextTranslation', 'CognitiveServices' ])
@description('Sets the kind of account.')
param kind string

type managedIdentity = {
  resourceId: string
  clientId: string
}



@allowed([
  'S0'
])
@description('SKU for the account. Defaults to "S0".')
param sku string = 'S0'

@description('Enables access from public networks. Defaults to true.')
param enablePublicNetworkAccess bool = true

resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: accountname
  location: location
  tags: tags
  kind: kind
  sku: {
    name: sku
  }
  properties: {
    customSubDomainName: accountname
    publicNetworkAccess: enablePublicNetworkAccess ? 'Enabled' : 'Disabled'
  }
  

}




output endpoint string = account.properties.endpoint
output name string = account.name

#disable-next-line outputs-should-not-contain-secrets // Don't use this in production, it's just for testing purposes.
output apikey string = account.listKeys().key1


