

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
};

webSocketOptions.AllowedOrigins.Add("*");
app.UseWebSockets(webSocketOptions);


app.UseMiddleware<backend.GameService.com.frogsoft.doudizhu.WS.WebsocketHandlerMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
