WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<LolModelValidator>();
builder.Services.AddScoped(typeof(ValidationFilterService<>));

builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

app.MapControllers();

app.Run();
