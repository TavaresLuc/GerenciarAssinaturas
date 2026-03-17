using GerenciarAssinaturas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciarAssinaturas.Infrastructure.Data.Configurations;

public class AssinanteConfiguration : IEntityTypeConfiguration<Assinante>
{
    public void Configure(EntityTypeBuilder<Assinante> builder)
    {
        builder.ToTable("Assinantes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.NomeCompleto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(150);

        // Índice único no e-mail — garante unicidade no banco
        builder.HasIndex(a => a.Email).IsUnique();

        builder.Property(a => a.DataInicioAssinatura)
            .IsRequired();

        builder.Property(a => a.Plano)
            .IsRequired();

        builder.Property(a => a.ValorMensal)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.StatusAssinatura)
            .IsRequired();

        // TempoAssinaturaMeses é calculado em runtime, não persiste no banco
        builder.Ignore(a => a.TempoAssinaturaMeses);
    }
}
