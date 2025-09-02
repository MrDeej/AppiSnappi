@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource familyapp_kv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: take('familyappkv-${uniqueString(resourceGroup().id)}', 24)
  location: location
  properties: {
    tenantId: tenant().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
  }
  tags: {
    'aspire-resource-name': 'familyapp-kv'
  }
}

output vaultUri string = familyapp_kv.properties.vaultUri

output name string = familyapp_kv.name