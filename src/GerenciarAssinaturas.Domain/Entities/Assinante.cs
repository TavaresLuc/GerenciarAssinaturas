using GerenciarAssinaturas.Domain.Enums;
using GerenciarAssinaturas.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace GerenciarAssinaturas.Domain.Entities;

public class Assinante
{
    public Guid Id { get; private set; }
    public string NomeCompleto { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime DataInicioAssinatura { get; private set; }
    public Plano Plano { get; private set; }
    public decimal ValorMensal { get; private set; }
    public StatusAssinatura StatusAssinatura { get; private set; }

    // Calculado em tempo de execução, não persiste no banco
    public int TempoAssinaturaMeses => CalcularTempoAssinaturaMeses();

    // Construtor protegido exigido pelo EF Core
    protected Assinante() { }

    private Assinante(
        string nomeCompleto,
        string email,
        DateTime dataInicioAssinatura,
        Plano plano,
        decimal valorMensal)
    {
        Id = Guid.NewGuid();
        NomeCompleto = nomeCompleto;
        Email = email;
        DataInicioAssinatura = dataInicioAssinatura;
        Plano = plano;
        ValorMensal = valorMensal;
        StatusAssinatura = StatusAssinatura.Ativo;
    }

    // Único ponto de criação — valida regras antes de instanciar
    public static Assinante Criar(
        string nomeCompleto,
        string email,
        DateTime dataInicioAssinatura,
        Plano plano,
        decimal valorMensal)
    {
        ValidarNomeCompleto(nomeCompleto);
        ValidarEmail(email);
        ValidarDataInicioAssinatura(dataInicioAssinatura);
        ValidarValorMensal(valorMensal);

        return new Assinante(nomeCompleto, email, dataInicioAssinatura, plano, valorMensal);
    }

    public void Desativar()
    {
        StatusAssinatura = StatusAssinatura.Inativo;
    }

    public void Editar(
        string nomeCompleto,
        string email,
        Plano plano,
        decimal valorMensal)
    {
        ValidarNomeCompleto(nomeCompleto);
        ValidarEmail(email);
        ValidarValorMensal(valorMensal);

        NomeCompleto = nomeCompleto;
        Email = email;
        Plano = plano;
        ValorMensal = valorMensal;
    }

    public int CalcularTempoAssinaturaMeses()
    {
        var hoje = DateTime.Today;
        var meses = ((hoje.Year - DataInicioAssinatura.Year) * 12)
                    + hoje.Month - DataInicioAssinatura.Month;

        return Math.Max(meses, 0);
    }

    private static void ValidarNomeCompleto(string nomeCompleto)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new DomainException("O nome completo é obrigatório.");
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O e-mail é obrigatório.");

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        if (!regex.IsMatch(email))
            throw new DomainException($"O e-mail '{email}' não tem formato válido.");
    }

    private static void ValidarDataInicioAssinatura(DateTime data)
    {
        if (data > DateTime.Today)
            throw new DomainException("A data de início da assinatura não pode ser futura.");
    }

    private static void ValidarValorMensal(decimal valor)
    {
        if (valor <= 0)
            throw new DomainException("O valor mensal deve ser maior que zero.");
    }
}
