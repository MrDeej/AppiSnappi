@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource familyapp_keyvault1 'Microsoft.KeyVault/vaults@2024-11-01' = {
  name: take('familyappkeyvault1-${uniqueString(resourceGroup().id)}', 24)
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
    'aspire-resource-name': 'familyapp-keyvault1'
  }
}

output vaultUri string = familyapp_keyvault1.properties.vaultUri

output name string = familyapp_keyvault1.name