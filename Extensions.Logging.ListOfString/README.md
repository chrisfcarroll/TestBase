**Extensions.Logging.ListOfString** logs to a list of string so you can test your logging.

If your components depend on an `ILogger<T>` use `Microsoft.Extensions.Logging.LoggerFactory`
to make one:
```
ILoggerFactory factory=new LoggerFactory.AddProvider(new StringListLoggerProvider())
var logger= factory.CreateLogger<Test1>();

...tests... ;

StringListLogger.Instance
   .LoggedLines
   .ShouldContain(x=>x.Matches("kilroy was here"));
```
or
```
var loggedLines = new List<string>();
var logger= new LoggerFactory().AddStringListLogger(loggedLines).CreateLogger("Test2");

...tests... ;

loggedLines.ShouldContain(x=>x.Matches("kilroy was here too."));

```
Or if a component only depends on an `ILogger`, it's much simpler. You don't need the factory or provider:

```
var uut = new MyComponent( StringListLogger.Instance );
... tests ...
StringListLogger.Instance
   .LoggedLines
   .ShouldContain(x=>x.Matches("kilroy was here"));
```
Or if you are using `Microsoft.Extensions.DependencyInjection` in your test suite then:
```
public StringListLogger Log = new();
...
ServiceCollection.AddLogging(
    lb => lb
             /* .AddConfiguration(Configuration.GetSection("Logging")) */
             .AddProvider(new StringListLoggerProvider(Log))
             /* .AddConsole() */
             );

...tests...

Log.LoggedLines.ShouldContain(x=>x.Matches("kilroy was here"));
```

For tests running in parallel, each one creating a new LoggerFactory and LoggerProvider, don't
rely on the shared StringListLogger.Instance. Follow the above example, passing a known 
instance to the LoggerProvider.

This is for Microsoft.Extensions.Logging.Abstractions. For Serilog, see https://www.nuget.org/packages/Serilog.Sinks.ListOfString/

Part of https://www.nuget.org/packages/TestBase . *TestBase* gives you a flying start to unit
testing with fluent assertions that are easy to extend, and tools for testing with dependencies
on AspNetMvc, HttpClient, Ado.Net, Streams and Loggers.