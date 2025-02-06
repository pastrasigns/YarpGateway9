using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilderContext =>
    {
        // Before forwarding the request to the destination
        transformBuilderContext.AddRequestTransform(async transformContext =>
        {
            Console.WriteLine($"Forwarding request to: {transformContext.DestinationPrefix}");

            var headers = transformContext.HttpContext.Request.Headers;

            transformContext.ProxyRequest.Headers.Add("MyNewHeader2", "MyNewHeaderValue456");

            await Task.CompletedTask;
        });

        // After receiving the response from the destination
        transformBuilderContext.AddResponseTransform(async transformContext =>
        {
            Console.WriteLine($"Response received with status code: {transformContext.HttpContext.Response.StatusCode}");

            var headers = transformContext.HttpContext.Response.Headers;

            await Task.CompletedTask;
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapReverseProxy();

app.MapControllers();

app.Run();
