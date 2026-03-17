using GerenciarAssinaturas.Domain.Enums;

namespace GerenciarAssinaturas.Application.DTOs;

public class EditarAssinanteRequest
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Plano Plano { get; set; }
    public decimal ValorMensal { get; set; }
}
