using FinalProject.Repository;
using FinalProject.Repository.Implementations.Account;
using FinalProject.Repository.Implementations.Payment;
using FinalProject.Repository.Implementations.User;
using FinalProject.Repository.Implementations.UserAccount;
using FinalProject.Repository.Interfaces.Account;
using FinalProject.Repository.Interfaces.Payment;
using FinalProject.Repository.Interfaces.User;
using FinalProject.Repository.Interfaces.UserAccount;
using FinalProject.Services.Implementations.Authentication;
using FinalProject.Services.Implementations.Payment;
using FinalProject.Services.Implementations.User;
using FinalProject.Services.Implementations.UserAccount;
using FinalProject.Services.Interfaces.Authentication;
using FinalProject.Services.Interfaces.Payment;
using FinalProject.Services.Interfaces.User;
using FinalProject.Services.Interfaces.UserAccount;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Connection to database
builder.Services.AddSingleton<ConnectionFactory>();

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
