

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var customerService = builder.AddProject<Projects.CustomerServiceApi>("customerserviceapi");

var apiGateWay = builder.AddProject<Projects.ApiGateWay>("apigateway").WithReference(customerService);

builder.AddProject<Projects.TestProject1>("testproject1");

var app = builder.Build();

app.Run();

builder.Build().Run();
