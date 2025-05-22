using v4posme_printer_window_services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
//Comentario test
var host = builder.Build();
host.Run();
