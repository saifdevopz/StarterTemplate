WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Blazor Code
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //Blazor Code
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHttpsRedirection();
}

//Blazor Code
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
