using backend;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Register RiotSettings as a singleton
builder.Services.AddSingleton<RiotSettings>();

// Register RiotService with HttpClient dependency
builder.Services.AddHttpClient<RiotService>();
builder.Services.AddSingleton<RiotService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();