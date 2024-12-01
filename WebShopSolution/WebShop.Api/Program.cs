using Microsoft.EntityFrameworkCore;
using WebShop.Application.InterfacesServices;
using WebShop.Application.Notifications;
using WebShop.Application.Services;
using WebShop.Domain.InterfacesRepositories;
using WebShop.Infrastructure.Persistence;
using WebShop.Infrastructure.Persistence.Repositories;
using WebShop.Infrastructure.Persistence.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Lägg till DbContext med din anslutning
builder.Services.AddDbContext<WebShopDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Registrera dina repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Registrera tjänster
builder.Services.AddScoped<IProductService, ProductService>();

// Registrera UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Lägg till notifieringstjänster om du har det
builder.Services.AddSingleton<ProductSubject>();
builder.Services.AddTransient<INotificationObserver, EmailNotification>();


// Add services to the container.
builder.Services.AddControllers();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();




// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();