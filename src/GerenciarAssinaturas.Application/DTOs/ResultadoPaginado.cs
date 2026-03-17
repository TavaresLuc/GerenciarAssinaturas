namespace GerenciarAssinaturas.Application.DTOs;

public class ResultadoPaginado<T>
{
    public IEnumerable<T> Itens { get; set; } = [];
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
}
