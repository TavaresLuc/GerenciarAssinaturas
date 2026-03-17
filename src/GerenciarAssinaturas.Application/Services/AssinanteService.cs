using GerenciarAssinaturas.Application.DTOs;
using GerenciarAssinaturas.Application.Interfaces;
using GerenciarAssinaturas.Domain.Entities;
using GerenciarAssinaturas.Domain.Exceptions;
using GerenciarAssinaturas.Domain.Interfaces;

namespace GerenciarAssinaturas.Application.Services;

public class AssinanteService : IAssinanteService
{
    private readonly IAssinanteRepository _repository;

    public AssinanteService(IAssinanteRepository repository)
    {
        _repository = repository;
    }

    public async Task<AssinanteResponse> CriarAsync(CriarAssinanteRequest request)
    {
        var emailEmUso = await _repository.ObterPorEmailAsync(request.Email);
        if (emailEmUso != null)
            throw new DomainException($"O e-mail '{request.Email}' já está em uso.");

        var assinante = Assinante.Criar(
            request.NomeCompleto,
            request.Email,
            request.DataInicioAssinatura,
            request.Plano,
            request.ValorMensal);

        await _repository.AdicionarAsync(assinante);
        await _repository.SalvarAsync();

        return AssinanteResponse.FromEntity(assinante);
    }

    public async Task<ResultadoPaginado<AssinanteResponse>> ListarAtivosAsync(int pagina, int tamanhoPagina)
    {
        var (itens, total) = await _repository.ListarAtivosAsync(pagina, tamanhoPagina);

        return new ResultadoPaginado<AssinanteResponse>
        {
            Itens = itens.Select(AssinanteResponse.FromEntity),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = total
        };
    }

    public async Task<AssinanteResponse> ObterPorIdAsync(Guid id)
    {
        var assinante = await ObterAtivoOuLancarAsync(id);
        return AssinanteResponse.FromEntity(assinante);
    }

    public async Task<AssinanteResponse> EditarAsync(Guid id, EditarAssinanteRequest request)
    {
        var assinante = await ObterAtivoOuLancarAsync(id);

        // Valida unicidade de e-mail apenas se estiver trocando
        if (!assinante.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailEmUso = await _repository.ObterPorEmailAsync(request.Email);
            if (emailEmUso != null)
                throw new DomainException($"O e-mail '{request.Email}' já está em uso.");
        }

        assinante.Editar(request.NomeCompleto, request.Email, request.Plano, request.ValorMensal);

        await _repository.AtualizarAsync(assinante);
        await _repository.SalvarAsync();

        return AssinanteResponse.FromEntity(assinante);
    }

    public async Task DesativarAsync(Guid id)
    {
        var assinante = await ObterAtivoOuLancarAsync(id);

        assinante.Desativar();

        await _repository.AtualizarAsync(assinante);
        await _repository.SalvarAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var assinante = await _repository.ObterPorIdAsync(id);
        if (assinante == null)
            throw new DomainException("Assinante não encontrado.");

        await _repository.RemoverAsync(assinante);
        await _repository.SalvarAsync();
    }

    // Busca o assinante e garante que está ativo — reutilizado em vários métodos
    private async Task<Assinante> ObterAtivoOuLancarAsync(Guid id)
    {
        var assinante = await _repository.ObterPorIdAsync(id);

        if (assinante == null)
            throw new DomainException("Assinante não encontrado.");

        if (assinante.StatusAssinatura == Domain.Enums.StatusAssinatura.Inativo)
            throw new DomainException("Operação não permitida para assinantes inativos.");

        return assinante;
    }
}
