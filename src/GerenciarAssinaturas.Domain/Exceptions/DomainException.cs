namespace GerenciarAssinaturas.Domain.Exceptions;

// Exceção lançada quando uma regra de negócio do domínio é violada
public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem) { }
}
