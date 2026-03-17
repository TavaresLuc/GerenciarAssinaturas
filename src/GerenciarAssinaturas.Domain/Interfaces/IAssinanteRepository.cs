using GerenciarAssinaturas.Domain.Entities;

namespace GerenciarAssinaturas.Domain.Interfaces;

public interface IAssinanteRepository
{
    Task<Assinante?> ObterPorIdAsync(Guid id);
    Task<Assinante?> ObterPorEmailAsync(string email);
    Task<(IEnumerable<Assinante> Itens, int TotalItens)> ListarAtivosAsync(int pagina, int tamanhoPagina);
    Task AdicionarAsync(Assinante assinante);
    Task AtualizarAsync(Assinante assinante);
    Task RemoverAsync(Assinante assinante);
    Task<int> SalvarAsync();
}
