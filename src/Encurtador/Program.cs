using Encurtador.Api.Servicos;
using Encurtador.Data;
using Encurtador.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Configuration.GetValue<bool>("AplicarMigrations"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/links", async (CriarLinkRequest req, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Url)
        || req.Url.Length > 2048
        || !Uri.TryCreate(req.Url, UriKind.Absolute, out var uri)
        || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
    {
        return Results.BadRequest(new { erro = "Informe uma URL valida com http ou https." });
    }

    for (var tentativa = 0; tentativa < 5; tentativa++)
    {
        var link = new Link
        {
            Codigo = GeradorDeCodigo.Gerar(),
            UrlOriginal = req.Url,
            CriadoEm = DateTime.UtcNow
        };

        db.Links.Add(link);

        try
        {
            await db.SaveChangesAsync();
            return Results.Created($"/{link.Codigo}", new LinkResponse(link.Codigo, link.UrlOriginal));
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(link).State = EntityState.Detached;
        }
    }

    return Results.StatusCode(StatusCodes.Status500InternalServerError);
});

app.MapGet("/{codigo}", async (string codigo, AppDbContext db) =>
{
    var url = await db.Links
        .AsNoTracking()
        .Where(l => l.Codigo == codigo)
        .Select(l => l.UrlOriginal)
        .FirstOrDefaultAsync();

    return url is null ? Results.NotFound() : Results.Redirect(url);
});

app.Run();