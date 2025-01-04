/*using GiftStore.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Db_API>();
*//*builder.Services.AddAuthentication(x=>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = bool.Parse(builder.Configuration["JwtConfig:Issuer"]),
            ValidateAudience = bool.Parse(builder.Configuration["JwtConfig:Audience"]),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
            ValidIssuer = ""+true,
            ValidAudience="" + true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey=true


        };



    });*//*
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();
//app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
*/
///javad
///
using System;
using System.Text;
using GiftStore.Data;
using GiftStore.Models;
using GiftStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;


var builder = WebApplication.CreateBuilder(args);

// 1. Configuration را از appsettings.json بارگذاری کنید:
builder.Configuration.AddJsonFile("appsettings.json");

#region policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "myCors", policy => {

        //policy.WithOrigins("http://example.com","http://www.contoso.com");
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});
#endregion policy

// HttpClient
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.Configure<SMSirConfig>(builder.Configuration.GetSection("SMSir"));

// Register SMSirService with HttpClient
builder.Services.AddHttpClient<SMSirService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region swagger 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SimoAPI", Version = "v1" });

    #region JWT Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    #endregion

});
#endregion


#region JWT
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
#endregion

#region  httpcontext
builder.Services.AddHttpContextAccessor();
#endregion

// use in OTP 
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

#region dbContext
builder.Services.AddDbContext<Db_API>(options =>
{
    options.UseSqlServer(builder.Configuration["defultConnection"]); //  localConnection                                                                
});


#endregion


// Register IMemoryCache
builder.Services.AddMemoryCache();


var app = builder.Build();

app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.DocExpansion(DocExpansion.None);
});
// }


app.UseHttpsRedirection();

app.UseRouting();


//jwt
app.UseAuthentication();

//CORS policy
app.UseCors("myCors");

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();