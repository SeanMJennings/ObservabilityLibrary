# Observability

Dotnet packages for observability.

Observability means we can understand what is happening in our distributed systems.
This means the ability to log whenever we need, and the ability to monitor dependencies and raise alerts if needed.

This repository provides a global static logger that should be configured once on startup in any application that uses this library:
```
OpenTelemetry.ConfigureOpenTelemetry();
```
This logger is then available to call anywhere in any application:

```
Logger.LogInformation("Message received");
```
Logging is a cross-cutting concern. By removing inversion-of-control for this specific use case only, we no longer need to provide an `ILogger` dependency to every constructor in every class of our applications.

During any tests, we can set a mock function to capture any logs passed to the global logger by using the `LoggingMocker`.
