using GerenciarAssinaturas.Domain.Entities;
using GerenciarAssinaturas.Domain.Enums;

namespace GerenciarAssinaturas.Application.DTOs;

public class AssinanteResponse
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataInicioAssinatura { get; set; }
    public Plano Plano { get; set; }
    public decimal ValorMensal { get; set; }
    public StatusAssinatura StatusAssinatura { get; set; }
    public int TempoAssinaturaMeses { get; set; }

    public static AssinanteResponse FromEntity(Assinante a) => new()
    {
        Id = a.Id,
        NomeCompleto = a.NomeCompleto,
        Email = a.Email,
        DataInicioAssinatura = a.DataInicioAssinatura,
        Plano = a.Plano,
        ValorMensal = a.ValorMensal,
        StatusAssinatura = a.StatusAssinatura,
        TempoAssinaturaMeses = a.TempoAssinaturaMeses
    };
}
