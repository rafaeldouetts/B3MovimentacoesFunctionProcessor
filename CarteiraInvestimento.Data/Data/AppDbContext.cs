using System.ComponentModel.DataAnnotations;
using CarteiraInvestimento.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvestimento.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Movimentacoes> Movimentacoes { get; set; }
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<MovimentacoesResumo> MovimentacoesResumo { get; set; }
        public DbSet<Indicadores> Indicadores { get; set; }
        public DbSet<TickerSetor> TickerSetor { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Ticker
            builder.Entity<Ticker>()
                .HasKey(t => t.Id);  // Chave primária

            // Relacionamento entre MovimentacoesEntity e TickerEntity
            builder.Entity<Movimentacoes>()
                .HasOne(m => m.Ticker)  // Um Movimentacoes tem um Ticker
                .WithMany()  // Um Ticker pode estar em várias Movimentacoes (relacionamento "um-para-muitos")
                .HasForeignKey(m => m.TickerId);  // A chave estrangeira é TickerId

            // Movimentacao resumo
            builder.Entity<MovimentacoesResumo>()
                .HasKey(t => t.Id); // Chave primária

            // Relacionamento entre MovimentacoesResumo e Ticker
            builder.Entity<MovimentacoesResumo>()
             .HasOne(m => m.Ticker) // Um MovimentacoesResumo tem um Ticker
             .WithOne(); // Um Ticker tem um Resumo (relacionamento "um-para-um")

            // Indicadores
            builder.Entity<Indicadores>()
                .HasKey(t => t.Id); // Chave primária

            //ticker setor 
            builder.Entity<TickerSetor>()
                .HasKey(t => t.Id); // Chave primária

            builder.Ignore<ValidationResult>();
        }
    }
}
