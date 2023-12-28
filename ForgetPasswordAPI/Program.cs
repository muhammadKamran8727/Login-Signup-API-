using ForgetPasswordAPI.BusinessLogic;
using ForgetPasswordAPI.IBusinessLogic;
using ForgetPasswordAPI.Models;
using Microsoft.EntityFrameworkCore;
using ForgetPasswordAPI.IBusinessLogic;
using ForgetPasswordAPI.BusinessLogic;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var provider = builder.Services.BuildServiceProvider();
builder.Services.AddScoped<ILoginBL, LoginBL>();

var Configuration = provider.GetService<IConfiguration>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(Configuration.GetConnectionString("sql-conn")));
var app = builder.Build();


app.UseCors(builder => builder
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowAnyOrigin()
    );
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
