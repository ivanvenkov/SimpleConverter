using FluentValidation;
using FluentValidation.AspNetCore;
using jConverter.Application.Filters;
using jConverter.Application.Managers;
using jConverter.Application.Validators;
using JConverter.Data;
using JConverter.Midllewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews(opt =>
{
    opt.Filters.Add<ValidationFilter>();
}).ConfigureApiBehaviorOptions(opt =>
{
    opt.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<XmlFileValidator>();

builder.Services.AddScoped<IFileManager, FileManager>();

builder.Services.AddDbContext<ConverterDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
