
using HotPotAPI.Contexts;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Repositories;
using HotPotAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace HotPotAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            builder.Services.AddHttpContextAccessor();

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173")
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            #endregion

            #region Context
            builder.Services.AddDbContext<HotPotDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion

            #region Repositories
            builder.Services.AddScoped<IRepository<int, Customer>, CustomerRepository>();
            builder.Services.AddScoped<IRepository<string, User>, UserRepository>();
            builder.Services.AddScoped<IRepository<int, Admin>, AdminRepository >();
            builder.Services.AddScoped<IRepository<int, Restaurant>, RestaurantRepository>();
            builder.Services.AddScoped<IRepository<int, RestaurantManager>, RestaurantManagerRepository>();
            builder.Services.AddScoped<IRepository<int, DeliveryPartner>, DeliveryPartnerRepository>();
            builder.Services.AddScoped<IRepository<int, MenuItem>, MenuItemRepository>();
            builder.Services.AddScoped<IRepository<int, Cart>, CartRepository>();
            builder.Services.AddScoped<IRepository<int, CartItem>, CartItemRepository>();
            builder.Services.AddScoped<IRepository<int, Order>, OrderRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            #endregion

            #region Services
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IRestaurantManagerService, RestaurantManagerService>();
            builder.Services.AddScoped<IDeliveryPartnerService, DeliveryPartnerService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            #endregion


            #region Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtToken"]))
                    };
                });
            #endregion
            var app = builder.Build();

            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
