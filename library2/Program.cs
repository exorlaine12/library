using library2.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controllers and Razor views.
builder.Services.AddControllersWithViews();

// Read the database connection from User Secrets.
var connectionString =
    builder.Configuration.GetConnectionString("Library");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'Library' is missing. " +
        "Add it in Manage User Secrets.");
}

// Create the shared PostgreSQL data source.
NpgsqlDataSource dataSource =
    NpgsqlDataSource.Create(connectionString);

// Register repositories.
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<MemberRepository>();
builder.Services.AddScoped<NoteRepository>();

// Register the shared data source.
builder.Services.AddSingleton(dataSource);

var app = builder.Build();

// Configure the HTTP pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
