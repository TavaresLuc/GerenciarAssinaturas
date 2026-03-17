# GerenciarAssinaturas

API REST para gerenciamento de assinantes de uma plataforma digital. Desenvolvida com ASP.NET Core 8, seguindo arquitetura DDD em 4 camadas.

## Tecnologias

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 (Code First)
- SQL Server (SQLEXPRESS)
- xUnit + Moq
- Swagger / OpenAPI

## Estrutura do Projeto

```
GerenciarAssinaturas.sln
│
├── src/
│   ├── GerenciarAssinaturas.Domain/         → Entidades, regras de negócio, interfaces
│   ├── GerenciarAssinaturas.Application/    → Casos de uso, DTOs, services
│   └── GerenciarAssinaturas.Infrastructure/ → EF Core, repositórios, migrations
│
├── GerenciarAssinaturas/                    → API (controllers, Program.cs)
│
└── tests/
    └── GerenciarAssinaturas.Tests/          → Testes unitários (xUnit + Moq)
```

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server com instância `SQLEXPRESS` rodando localmente
- `dotnet-ef` instalado globalmente:

```bash
dotnet tool install --global dotnet-ef
```

## Como executar

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd GerenciarAssinaturas
```

### 2. Configurar a connection string (opcional)

O projeto já vem configurado para `.\SQLEXPRESS`. Se precisar alterar, edite o arquivo:

```
GerenciarAssinaturas/appsettings.json
```

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=GerenciarAssinaturas;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Criar o banco de dados

As migrations já estão geradas. Execute para criar o banco:

```bash
dotnet ef database update --project src/GerenciarAssinaturas.Infrastructure --startup-project src/GerenciarAssinaturas.Infrastructure
```

### 4. Rodar a API

```bash
dotnet run --project GerenciarAssinaturas
```

A API estará disponível em: `http://localhost:5227`

### 5. Acessar o Swagger

Abra no navegador:

```
http://localhost:5227/swagger
```

### 6. Rodar os testes

```bash
dotnet test tests/GerenciarAssinaturas.Tests
```

Resultado esperado: **25 testes, 0 falhas**.

---

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/assinantes` | Criar novo assinante |
| `GET` | `/assinantes?pagina=1&tamanhoPagina=10` | Listar ativos com paginação |
| `GET` | `/assinantes/{id}` | Detalhes de um assinante ativo |
| `PATCH` | `/assinantes/{id}` | Editar assinante ativo |
| `PATCH` | `/assinantes/{id}/desativar` | Soft delete (status → Inativo) |
| `DELETE` | `/assinantes/{id}` | Exclusão física |

### Exemplo — Criar assinante

**Request:**
```json
POST /assinantes
{
  "nomeCompleto": "João Silva",
  "email": "joao@email.com",
  "dataInicioAssinatura": "2024-01-15",
  "plano": 1,
  "valorMensal": 29.90
}
```

**Planos:** `1` = Basico, `2` = Padrao, `3` = Premium

**Response `201 Created`:**
```json
{
  "id": "2e8db401-500e-44c3-a504-6375ce75982a",
  "nomeCompleto": "João Silva",
  "email": "joao@email.com",
  "dataInicioAssinatura": "2024-01-15T00:00:00",
  "plano": 1,
  "valorMensal": 29.90,
  "statusAssinatura": 1,
  "tempoAssinaturaMeses": 26
}
```

### Exemplo — Listar com paginação

**Response `200 OK`:**
```json
{
  "itens": [...],
  "paginaAtual": 1,
  "tamanhoPagina": 10,
  "totalItens": 42,
  "totalPaginas": 5
}
```

### Erros de negócio

Violações das regras retornam `400 Bad Request`:
```json
{ "erro": "O e-mail 'joao@email.com' já está em uso." }
```

---

## Regras de Negócio

1. `TempoAssinaturaMeses` é calculado dinamicamente — não persiste no banco
2. Data de início não pode ser futura
3. Valor mensal deve ser maior que zero
4. E-mail deve ter formato válido
5. E-mail deve ser único no sistema
6. Listagem, edição e visualização consideram apenas assinantes **Ativos**

---

## Decisões de Arquitetura

As decisões detalhadas estão documentadas em [`decisions.txt`](decisions.txt). Resumo:

**DDD — Domínio Rico**
A entidade `Assinante` tem construtor privado, setters privados e toda validação encapsulada dentro dela. A criação só ocorre via `Assinante.Criar()`, que valida as regras antes de instanciar (Factory Method). Isso evita o anti-pattern "Anemic Domain Model".

**SOLID aplicado**
- *Single Responsibility*: cada classe tem uma responsabilidade (entidade valida, service orquestra, repositório persiste)
- *Dependency Inversion*: `IAssinanteRepository` fica no Domain; a Infrastructure implementa — o Domain não depende de EF Core
- *Open/Closed*: novos planos ou status podem ser adicionados nos enums sem quebrar o resto

**Separação de camadas**
- Domain não referencia nada além do próprio projeto
- Application conhece apenas o Domain
- Infrastructure conhece Domain (para implementar as interfaces)
- API conhece Application e Infrastructure (apenas para registro de DI)

**Soft delete vs Hard delete**
`PATCH /desativar` muda status para Inativo (preserva histórico). `DELETE` é a exclusão física, separada e explícita.
