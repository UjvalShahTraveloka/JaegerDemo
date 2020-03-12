# JaegerDemo 

Sample Application showcasing the Jaeger Tracer with OpenTracing 

# Changes to StartUp.cs

```csharp


public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    //Step 1  : Configure and Add Open Tracing.
    services.AddOpenTracing();
    
    // Step 2 : Adds the Jaeger Tracer.
    services.AddSingleton<ITracer>(serviceProvider =>
    {
        string serviceName = serviceProvider.GetRequiredService<IWebHostEnvironment>().ApplicationName;

        // This will log to a default localhost installation of Jaeger.
        var tracer = new Tracer.Builder(serviceName)
            .WithSampler(new ConstSampler(true))
            .Build();

        // Allows code that can't use DI to also access the tracer.
        GlobalTracer.Register(tracer);

        return tracer;
    });

```



# Changes to Default Controller 

```csharp
private readonly ILogger<WeatherForecastController> _logger;
private readonly ITracer tracer;

public WeatherForecastController(ILogger<WeatherForecastController> logger,ITracer tracer)
{
    _logger = logger;
    this.tracer = tracer;
}

[HttpGet]
public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
{
    var rng = new Random();
    using (IScope scope = tracer.BuildSpan("waitingForValues").StartActive(finishSpanOnDispose: true))
    {
        await Task.Delay(1000);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
    }
}

```
