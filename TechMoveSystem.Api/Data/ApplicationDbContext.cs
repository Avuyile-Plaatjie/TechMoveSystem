using Microsoft.EntityFrameworkCore;
using TechMoveSystem.Api.Models;

namespace TechMoveSystem.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany(c => c.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ServiceRequest>()
            .HasOne(s => s.Contract)
            .WithMany(c => c.ServiceRequests)
            .HasForeignKey(s => s.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
