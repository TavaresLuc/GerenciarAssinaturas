using GerenciarAssinaturas.Application.DTOs;

namespace GerenciarAssinaturas.Application.Interfaces;

public interface IAssinanteService
{
    Task<AssinanteResponse> CriarAsync(CriarAssinanteRequest request);
    Task<ResultadoPaginado<AssinanteResponse>> ListarAtivosAsync(int pagina, int tamanhoPagina);
    Task<AssinanteResponse> ObterPorIdAsync(Guid id);
    Task<AssinanteResponse> EditarAsync(Guid id, EditarAssinanteRequest request);
    Task DesativarAsync(Guid id);
    Task RemoverAsync(Guid id);
}
