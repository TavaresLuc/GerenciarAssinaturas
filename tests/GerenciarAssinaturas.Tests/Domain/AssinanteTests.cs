using GerenciarAssinaturas.Domain.Entities;
using GerenciarAssinaturas.Domain.Enums;
using GerenciarAssinaturas.Domain.Exceptions;

namespace GerenciarAssinaturas.Tests.Domain;

public class AssinanteTests
{
    // ─── Criar ────────────────────────────────────────────────────────────────

    [Fact]
    public void Criar_DadosValidos_RetornaAssinanteAtivo()
    {
        // Arrange
        var data = DateTime.Today.AddMonths(-3);

        // Act
        var assinante = Assinante.Criar("João Silva", "joao@email.com", data, Plano.Basico, 29.90m);

        // Assert
        Assert.Equal("João Silva", assinante.NomeCompleto);
        Assert.Equal("joao@email.com", assinante.Email);
        Assert.Equal(StatusAssinatura.Ativo, assinante.StatusAssinatura);
        Assert.NotEqual(Guid.Empty, assinante.Id);
    }

    [Fact]
    public void Criar_NomeVazio_LancaDomainException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            Assinante.Criar("", "joao@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m));

        Assert.Contains("nome completo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("emailsemarrobase")]
    [InlineData("email@")]
    [InlineData("@dominio.com")]
    [InlineData("")]
    public void Criar_EmailInvalido_LancaDomainException(string emailInvalido)
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() =>
            Assinante.Criar("João Silva", emailInvalido, DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m));
    }

    [Fact]
    public void Criar_DataFutura_LancaDomainException()
    {
        // Arrange
        var dataFutura = DateTime.Today.AddDays(1);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            Assinante.Criar("João Silva", "joao@email.com", dataFutura, Plano.Basico, 29.90m));

        Assert.Contains("futura", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Criar_ValorMensalInvalido_LancaDomainException(decimal valor)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            Assinante.Criar("João Silva", "joao@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, valor));

        Assert.Contains("valor mensal", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── CalcularTempoAssinaturaMeses ─────────────────────────────────────────

    [Fact]
    public void CalcularTempoAssinaturaMeses_DataHa12Meses_Retorna12()
    {
        // Arrange
        var data = DateTime.Today.AddMonths(-12);
        var assinante = Assinante.Criar("João Silva", "joao@email.com", data, Plano.Basico, 29.90m);

        // Act
        var tempo = assinante.TempoAssinaturaMeses;

        // Assert
        Assert.Equal(12, tempo);
    }

    [Fact]
    public void CalcularTempoAssinaturaMeses_DataHoje_RetornaZero()
    {
        // Arrange
        var assinante = Assinante.Criar("João Silva", "joao@email.com", DateTime.Today, Plano.Basico, 29.90m);

        // Act & Assert
        Assert.Equal(0, assinante.TempoAssinaturaMeses);
    }

    // ─── Desativar ────────────────────────────────────────────────────────────

    [Fact]
    public void Desativar_AssinanteAtivo_MudaStatusParaInativo()
    {
        // Arrange
        var assinante = Assinante.Criar("João Silva", "joao@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m);

        // Act
        assinante.Desativar();

        // Assert
        Assert.Equal(StatusAssinatura.Inativo, assinante.StatusAssinatura);
    }

    // ─── Editar ───────────────────────────────────────────────────────────────

    [Fact]
    public void Editar_DadosValidos_AtualizaPropriedades()
    {
        // Arrange
        var assinante = Assinante.Criar("João Silva", "joao@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m);

        // Act
        assinante.Editar("João Atualizado", "joao.novo@email.com", Plano.Premium, 99.90m);

        // Assert
        Assert.Equal("João Atualizado", assinante.NomeCompleto);
        Assert.Equal("joao.novo@email.com", assinante.Email);
        Assert.Equal(Plano.Premium, assinante.Plano);
        Assert.Equal(99.90m, assinante.ValorMensal);
    }

    [Fact]
    public void Editar_EmailInvalido_LancaDomainException()
    {
        // Arrange
        var assinante = Assinante.Criar("João Silva", "joao@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            assinante.Editar("João Silva", "emailinvalido", Plano.Basico, 29.90m));
    }

    [Fact]
    public void Editar_ValorZero_LancaDomainException()
    {
        // Arrange
        var assinante = Assinante.Criar("João Silva", "joao@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            assinante.Editar("João Silva", "joao@email.com", Plano.Basico, 0));
    }
}
