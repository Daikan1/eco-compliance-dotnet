using EcoCompliance.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoCompliance.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<ComplianceRecord> ComplianceRecords => Set<ComplianceRecord>();
    public DbSet<EsgIndicator> EsgIndicators => Set<EsgIndicator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("COMPANY");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(e => e.Name).HasColumnName("NAME").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Cnpj).HasColumnName("CNPJ").HasMaxLength(20);
            entity.Property(e => e.Sector).HasColumnName("SECTOR").HasMaxLength(100);
        });

        modelBuilder.Entity<ComplianceRecord>(entity =>
        {
            entity.ToTable("COMPLIANCE_RECORDS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(e => e.CompanyId).HasColumnName("COMPANY_ID");
            entity.Property(e => e.ComplianceType).HasColumnName("COMPLIANCE_TYPE").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Status).HasColumnName("STATUS").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ReportDate).HasColumnName("REPORT_DATE");

            entity.HasOne(e => e.Company)
                  .WithMany(c => c.ComplianceRecords)
                  .HasForeignKey(e => e.CompanyId)
                  .HasConstraintName("FK_CR_COMPANY")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EsgIndicator>(entity =>
        {
            entity.ToTable("ESG_INDICATORS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(e => e.CompanyId).HasColumnName("COMPANY_ID");
            entity.Property(e => e.IndicatorType).HasColumnName("INDICATOR_TYPE").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasColumnName("VALUE");
            entity.Property(e => e.Unit).HasColumnName("UNIT").HasMaxLength(50);
            entity.Property(e => e.MeasuredAt).HasColumnName("MEASURED_AT");

            entity.HasOne(e => e.Company)
                  .WithMany(c => c.EsgIndicators)
                  .HasForeignKey(e => e.CompanyId)
                  .HasConstraintName("FK_ESG_COMPANY")
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
