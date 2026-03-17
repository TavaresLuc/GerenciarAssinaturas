using GerenciarAssinaturas.Domain.Entities;
using GerenciarAssinaturas.Domain.Enums;
using GerenciarAssinaturas.Domain.Interfaces;
using GerenciarAssinaturas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciarAssinaturas.Infrastructure.Repositories;

public class AssinanteRepository : IAssinanteRepository
{
    private readonly AppDbContext _context;

    public AssinanteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Assinante?> ObterPorIdAsync(Guid id)
        => await _context.Assinantes.FindAsync(id);

    public async Task<Assinante?> ObterPorEmailAsync(string email)
        => await _context.Assinantes
            .FirstOrDefaultAsync(a => a.Email == email);

    public async Task<(IEnumerable<Assinante> Itens, int TotalItens)> ListarAtivosAsync(int pagina, int tamanhoPagina)
    {
        var query = _context.Assinantes
            .Where(a => a.StatusAssinatura == StatusAssinatura.Ativo);

        var total = await query.CountAsync();

        var itens = await query
            .OrderBy(a => a.NomeCompleto)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(Assinante assinante)
        => await _context.Assinantes.AddAsync(assinante);

    public Task AtualizarAsync(Assinante assinante)
    {
        _context.Assinantes.Update(assinante);
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Assinante assinante)
    {
        _context.Assinantes.Remove(assinante);
        return Task.CompletedTask;
    }

    public async Task<int> SalvarAsync()
        => await _context.SaveChangesAsync();
}
