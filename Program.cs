using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using TimeGraphServer.Database;
using TimeGraphServer.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddDocumentFromFile("schema.graphql")
    .BindRuntimeType<Query>();

builder.Services.AddCors();

builder.Services.AddGraphQL(provider =>
    SchemaBuilder.New().AddServices(provider)
        .AddType<ProjectType>()
        .AddType<TimeLogType>()
        .AddQueryType<Query>()
        .Create());

var app = builder.Build();

using (var dbContext = new TimeGraphContext())
{
    dbContext.Database.EnsureCreated();

    dbContext.SaveChanges();
    
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UsePlayground(new PlaygroundOptions()
    {
        QueryPath = "/api",
        Path = "/Playground"
    });
    app.UseRouting();
}
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL().WithOptions(new GraphQLServerOptions()
    {
        AllowedGetOperations = AllowedGetOperations.Query
    });
});



app.Run();