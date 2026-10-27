using Encurtador.Api.Servicos;

namespace Encurtador.Tests;

public class GeradorDeCodigoTests
{
    private const string Alfabeto = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    [Fact]
    public void Gerar_RetornaSeteCaracteresPorPadrao()
    {
        var codigo = GeradorDeCodigo.Gerar();

        Assert.Equal(7, codigo.Length);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(10)]
    public void Gerar_RespeitaOTamanhoPedido(int tamanho)
    {
        var codigo = GeradorDeCodigo.Gerar(tamanho);

        Assert.Equal(tamanho, codigo.Length);
    }

    [Fact]
    public void Gerar_UsaApenasCaracteresDoAlfabeto()
    {
        for (var i = 0; i < 1000; i++)
        {
            var codigo = GeradorDeCodigo.Gerar();

            Assert.All(codigo, c => Assert.Contains(c, Alfabeto));
        }
    }

    [Fact]
    public void Gerar_ProduzCodigosDiferentes()
    {
        var codigos = Enumerable.Range(0, 1000)
            .Select(_ => GeradorDeCodigo.Gerar())
            .ToHashSet();

        Assert.Equal(1000, codigos.Count);
    }
}