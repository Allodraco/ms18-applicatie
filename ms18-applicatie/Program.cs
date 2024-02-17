using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.OpenApi.Models;
using ms18_applicatie.Models.team_a;
using Maasgroep.Database;
using Maasgroep.Database.Interfaces;
using Maasgroep.Database.Receipts;
using Maasgroep.Database.Admin;
using Maasgroep.Database.Orders;
using Maasgroep.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ms18_applicatie.Repository.PhotoAlbum;
using ms18_applicatie.Interfaces;
using Maasgroep.Interfaces;
using Maasgroep.Services;

var builder = WebApplication.CreateBuilder(args);

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MaasgroepContext>(options =>
    options.UseNpgsql(MaasgroepContext.GetConnectionString(connectionString)));

// Add HTTP context
builder.Services.AddHttpContextAccessor();

// Add repositories
// - Admin/login
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ITokenStoreRepository, TokenStoreRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

// - Receipt
builder.Services.AddScoped<ICostCentreRepository, CostCentreRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IReceiptApprovalRepository, ReceiptApprovalRepository>();
builder.Services.AddScoped<IReceiptPhotoRepository, ReceiptPhotoRepository>();
builder.Services.AddScoped<IReceiptStatusRepository, ReceiptStatusRepository>();

// - Order
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();

// - PhotoAlbum
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();

// Authentication
builder.Services.AddTransient<IMaasgroepAuthenticationService, MaasgroepAuthenticationService>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{

    // Filter special MaasgroepExceptions that are thrown in our controllers
    options.Filters.Add<MaasgroepExceptionFilter>();

}).ConfigureApiBehaviorOptions(options =>
{
    // Model validation error when posting/putting data
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(new
    {
        error = 400,
        message = "Ongeldige gegevens opgegeven",
    });
});

builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Make a call to User/Login, then paste your token here:",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.CustomOperationIds(e => e.ActionDescriptor.RouteValues["action"]);

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var appSettings = @"./Controllers/team-a/CalendarIds.json";
    if (File.Exists(appSettings))
    {
        config.AddJsonFile(appSettings,
            optional: false,
            reloadOnChange: false);
    }
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
        builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
});

builder.Services.Configure<CalendarSettings>(builder.Configuration.GetSection(nameof(CalendarSettings)));

builder.Services.AddSingleton((x) =>
{
    var settings = builder.Configuration
        .GetSection(nameof(CalendarSettings))
        .Get<CalendarSettings>();

    string[] scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };
    using var stream =
           new FileStream(settings.FilePath, FileMode.Open, FileAccess.Read);
    var credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);


    //Create the Calendar service.
    return new CalendarService(new BaseClientService.Initializer()
    {
        HttpClientInitializer = credential,
        ApplicationName = "project c",
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}


app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("../swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

app.UseSwagger();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("MyAllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TokenMiddleware>();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();