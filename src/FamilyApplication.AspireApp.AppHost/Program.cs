var builder = DistributedApplication.CreateBuilder(args);


var keyVault = builder.AddAzureKeyVault("familyapp-kv");

builder.AddProject<Projects.FamilyApplication_AspireApp_Web>("webfrontend")
    .WithReference(keyVault)
    .WithExternalHttpEndpoints();

builder.Build().Run();
