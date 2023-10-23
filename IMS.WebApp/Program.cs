using IMS.PluginInterfaces;
using IMS.Plugins.EFCoreSqlServer;
using IMS.Plugins.InMemory;
using IMS.UseCases.Activities;
using IMS.UseCases.Inventories;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products;
using IMS.UseCases.Reports;
using IMS.WebApp.Data;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var constr = builder.Configuration.GetConnectionString("InventoryManagement");

// configure EF Core for Identity
builder.Services.AddDbContext<AccountDbContext>(options =>
{
    options.UseSqlServer(constr);
});

// configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // this is a very basic identity implementation
    options.SignIn.RequireConfirmedEmail=false;
}).AddEntityFrameworkStores<AccountDbContext>();

// this line is where we add Entity Frameword Core into our application
// also, this connects the sqlserver context to this web app
// it injects all our EFC related classes to the web app.
// changed from AddDbContext to DbContextFactory so that we don't have more than one dbContext active at a time
builder.Services.AddDbContextFactory<IMSContext>(options =>
{
    // the UseSqlServer method is accessing the connection string from builder.Configuration
    options.UseSqlServer(constr);
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

if (builder.Environment.IsEnvironment("TESTING"))
{
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

    builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
    builder.Services.AddSingleton<IProductRepository, ProductRepository>();
    builder.Services.AddSingleton<IInventoryTransactionRepository, InventoryTransactionRepository>();
    builder.Services.AddSingleton<IProductTransactionRepository, ProductTransactionRepository>();
}
else
{
    builder.Services.AddTransient<IInventoryRepository, InventoryEFCoreRepository>();
    builder.Services.AddTransient<IProductRepository, ProductEFCoreRepository>();
    builder.Services.AddTransient<IInventoryTransactionRepository, InventoryTransactionEFCoreRepository>();
    builder.Services.AddTransient<IProductTransactionRepository, ProductTransactionEFCoreRepository>();
}


builder.Services.AddTransient<IViewInventoriesByNameUseCase, ViewInventoriesByNameUseCase>();
builder.Services.AddTransient<IAddInventoryUseCase, AddInventoryUseCase>();
builder.Services.AddTransient<IEditInventoryUseCase, EditInventoryUseCase>();
builder.Services.AddTransient<IViewInventoryByIdUseCase, ViewInventoryByIdUseCase>();

builder.Services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
builder.Services.AddTransient<IViewProductsByNameUseCase, ViewProductsByNameUseCase>();
builder.Services.AddTransient<IAddProductUseCase, AddProductUseCase>();
builder.Services.AddTransient<IEditProductUseCase, EditProductUseCase>();
builder.Services.AddTransient<IViewProductByIdUseCase, ViewProductByIdUseCase>();

builder.Services.AddTransient<IProduceProductUseCase, ProduceProductUseCase>();
builder.Services.AddTransient<IPurchaseInventoryUseCase, PurchaseInventoryUseCase>();
builder.Services.AddTransient<ISellProductUseCase, SellProductUseCase>();

builder.Services.AddTransient<ISearchInventoryTransactionsUseCase, SearchInventoryTransactionsUseCase>();
builder.Services.AddTransient<ISearchProductTransactionsUseCase, SearchProductTransactionsUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
