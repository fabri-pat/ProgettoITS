using app.Entities;
using app.Helpers;
using app.Middleware.Authorization;
using app.Repositories;
using app.Services;

var builder = WebApplication.CreateBuilder(args);

var localhostOrigins = "localhostorigins";

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMongo()
                .AddMongoRepository<User, Guid>("Users");

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Configure Dependency Injection 
builder.Services.AddScoped<IEMailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: localhostOrigins,
                        policy =>
                        {
                            policy.WithOrigins("http://localhost:4200")
                            .AllowCredentials()
                            .AllowAnyHeader();
                        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(localhostOrigins);

app.UseHttpsRedirection();

app.UseExceptionHandlerMiddleware();

app.UseAuthMiddleware();

app.MapControllers();

app.Run();
