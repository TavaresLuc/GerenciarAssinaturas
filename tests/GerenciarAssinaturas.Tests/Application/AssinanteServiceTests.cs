using GerenciarAssinaturas.Application.DTOs;
using GerenciarAssinaturas.Application.Services;
using GerenciarAssinaturas.Domain.Entities;
using GerenciarAssinaturas.Domain.Enums;
using GerenciarAssinaturas.Domain.Exceptions;
using GerenciarAssinaturas.Domain.Interfaces;
using Moq;

namespace GerenciarAssinaturas.Tests.Application;

public class AssinanteServiceTests
{
    private readonly Mock<IAssinanteRepository> _repositoryMock;
    private readonly AssinanteService _service;

    public AssinanteServiceTests()
    {
        _repositoryMock = new Mock<IAssinanteRepository>();
        _service = new AssinanteService(_repositoryMock.Object);
    }

    // ─── Criar ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CriarAsync_DadosValidos_RetornaAssinanteResponse()
    {
        // Arrange
        var request = new CriarAssinanteRequest
        {
            NomeCompleto = "Maria Souza",
            Email = "maria@email.com",
            DataInicioAssinatura = DateTime.Today.AddMonths(-2),
            Plano = Plano.Padrao,
            ValorMensal = 59.90m
        };

        _repositoryMock.Setup(r => r.ObterPorEmailAsync(request.Email)).ReturnsAsync((Assinante?)null);
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SalvarAsync()).ReturnsAsync(1);

        // Act
        var response = await _service.CriarAsync(request);

        // Assert
        Assert.Equal(request.NomeCompleto, response.NomeCompleto);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal(StatusAssinatura.Ativo, response.StatusAssinatura);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Assinante>()), Times.Once);
        _repositoryMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_EmailJaEmUso_LancaDomainException()
    {
        // Arrange
        var assinanteExistente = Assinante.Criar("Outro", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m);

        var request = new CriarAssinanteRequest
        {
            NomeCompleto = "Maria Souza",
            Email = "maria@email.com",
            DataInicioAssinatura = DateTime.Today.AddMonths(-2),
            Plano = Plano.Padrao,
            ValorMensal = 59.90m
        };

        _repositoryMock.Setup(r => r.ObterPorEmailAsync(request.Email)).ReturnsAsync(assinanteExistente);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _service.CriarAsync(request));
        Assert.Contains("já está em uso", ex.Message);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Assinante>()), Times.Never);
    }

    // ─── ObterPorId ───────────────────────────────────────────────────────────

    [Fact]
    public async Task ObterPorIdAsync_AssinanteAtivo_RetornaResponse()
    {
        // Arrange
        var assinante = Assinante.Criar("Maria Souza", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Padrao, 59.90m);
        _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id)).ReturnsAsync(assinante);

        // Act
        var response = await _service.ObterPorIdAsync(assinante.Id);

        // Assert
        Assert.Equal(assinante.Id, response.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_NaoEncontrado_LancaDomainException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Assinante?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.ObterPorIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ObterPorIdAsync_AssinanteInativo_LancaDomainException()
    {
        // Arrange
        var assinante = Assinante.Criar("Maria Souza", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Padrao, 59.90m);
        assinante.Desativar();
        _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id)).ReturnsAsync(assinante);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainException>(() => _service.ObterPorIdAsync(assinante.Id));
        Assert.Contains("inativo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── Editar ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task EditarAsync_DadosValidos_AtualizaERetorna()
    {
        // Arrange
        var assinante = Assinante.Criar("Maria Souza", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Padrao, 59.90m);
        var request = new EditarAssinanteRequest
        {
            NomeCompleto = "Maria Atualizada",
            Email = "maria@email.com", // mesmo e-mail, não precisa checar duplicidade
            Plano = Plano.Premium,
            ValorMensal = 99.90m
        };

        _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SalvarAsync()).ReturnsAsync(1);

        // Act
        var response = await _service.EditarAsync(assinante.Id, request);

        // Assert
        Assert.Equal("Maria Atualizada", response.NomeCompleto);
        Assert.Equal(Plano.Premium, response.Plano);
        _repositoryMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task EditarAsync_NovoEmailJaEmUso_LancaDomainException()
    {
        // Arrange
        var assinante = Assinante.Criar("Maria Souza", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Padrao, 59.90m);
        var outro = Assinante.Criar("Outro", "ocupado@email.com", DateTime.Today.AddMonths(-1), Plano.Basico, 29.90m);

        var request = new EditarAssinanteRequest
        {
            NomeCompleto = "Maria Souza",
            Email = "ocupado@email.com",
            Plano = Plano.Padrao,
            ValorMensal = 59.90m
        };

        _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.ObterPorEmailAsync("ocupado@email.com")).ReturnsAsync(outro);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.EditarAsync(assinante.Id, request));
        _repositoryMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    // ─── Desativar ────────────────────────────────────────────────────────────

    [Fact]
    public async Task DesativarAsync_AssinanteAtivo_DesativaESalva()
    {
        // Arrange
        var assinante = Assinante.Criar("Maria Souza", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Padrao, 59.90m);

        _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SalvarAsync()).ReturnsAsync(1);

        // Act
        await _service.DesativarAsync(assinante.Id);

        // Assert
        Assert.Equal(StatusAssinatura.Inativo, assinante.StatusAssinatura);
        _repositoryMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    // ─── Remover ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RemoverAsync_AssinanteExistente_RemoveESalva()
    {
        // Arrange
        var assinante = Assinante.Criar("Maria Souza", "maria@email.com", DateTime.Today.AddMonths(-1), Plano.Padrao, 59.90m);

        _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id)).ReturnsAsync(assinante);
        _repositoryMock.Setup(r => r.RemoverAsync(It.IsAny<Assinante>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SalvarAsync()).ReturnsAsync(1);

        // Act
        await _service.RemoverAsync(assinante.Id);

        // Assert
        _repositoryMock.Verify(r => r.RemoverAsync(assinante), Times.Once);
        _repositoryMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_NaoEncontrado_LancaDomainException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Assinante?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.RemoverAsync(Guid.NewGuid()));
    }
}
