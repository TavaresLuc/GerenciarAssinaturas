using GerenciarAssinaturas.Domain.Entities;
using GerenciarAssinaturas.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace GerenciarAssinaturas.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Assinante> Assinantes => Set<Assinante>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssinanteConfiguration());
    }
}
