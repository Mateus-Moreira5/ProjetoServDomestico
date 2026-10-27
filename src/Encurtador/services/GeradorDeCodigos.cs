using System.Security.Cryptography;

namespace Encurtador.Api.Servicos;

public static class GeradorDeCodigo
{
    private const string Alfabeto = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Gerar(int tamanho = 7)
    {
        return string.Create(tamanho, 0, (span, _) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = Alfabeto[RandomNumberGenerator.GetInt32(Alfabeto.Length)];
            }
        });
    }
}