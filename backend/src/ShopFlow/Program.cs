using Microsoft.EntityFrameworkCore;
using ShopFlow.DAL;
using ShopFlow.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddPersistence(builder.Configuration)
       .AddApplication()
       .AddWeb(builder.Configuration);

var app = builder.Build();

await ApplyDatabaseMigrationsAsync(app);

// CORS runs first so that even a failed request still carries the headers the browser
// needs, otherwise the front-end would only ever see "blocked by CORS".
app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);

// GlobalExceptionHandler decides what an unhandled failure looks like.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Applies migrations and seeds the first administrator, so a fresh clone runs on one F5.
static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ShopFlowDbContext>();
    await context.Database.MigrateAsync();

    var seeder = services.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}
