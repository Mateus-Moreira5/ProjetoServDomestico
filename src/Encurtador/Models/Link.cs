using System;

namespace Encurtador.Models;

public class Link
{
    public long Id { get; set; }
    public string Codigo { get; set; }
    public string UrlOriginal { get; set; }
    public DateTime CriadoEm { get; set; }
}
