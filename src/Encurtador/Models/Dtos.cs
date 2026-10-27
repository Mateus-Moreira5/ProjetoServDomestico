namespace Encurtador.Models;

public record CriarLinkRequest(string? Url);
public record LinkResponse(string Codigo, string UrlOriginal);