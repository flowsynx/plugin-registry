using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using FlowSynx.PluginRegistry.Infrastructure.Configurations;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Profile;
using FlowSynx.PluginRegistry.Domain.Statistic;
using FlowSynx.PluginRegistry.Domain;
using FlowSynx.PluginRegistry.Application.Services;
using System.Reflection.Emit;

namespace FlowSynx.PluginRegistry.Infrastructure.Contexts;

public class ApplicationContext : AuditableContext
{
    private readonly ILogger<ApplicationContext> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISystemClock _systemClock;

    public ApplicationContext(DbContextOptions<ApplicationContext> contextOptions,
        ILogger<ApplicationContext> logger, IHttpContextAccessor httpContextAccessor, 
        ISystemClock systemClock)
        : base(contextOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _systemClock = systemClock;
        _logger = logger;
    }

    public DbSet<ProfileEntity> Profiles { get; set; }
    public DbSet<PluginEntity> Plugins { get; set; }
    public DbSet<PluginVersionEntity> PluginVersions { get; set; }
    public DbSet<StatisticEntity> Statistics { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        try
        {
            HandleSoftDelete();
            ApplyAuditing();

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return await base.SaveChangesAsync(cancellationToken);

            return await base.SaveChangesAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void HandleSoftDelete()
    {
        try
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDeletable);
            foreach (var entry in entries)
            {
                entry.State = EntityState.Modified;
                ((ISoftDeletable)entry.Entity).IsDeleted = true;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void ApplyAuditing()
    {
        try
        {
            var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            );

            foreach (var entry in entries)
            {
                var auditable = (IAuditableEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedOn = _systemClock.UtcNow;
                    auditable.CreatedBy = GetUserId();
                }

                if (entry.State == EntityState.Modified)
                {
                    auditable.LastModifiedOn = _systemClock.UtcNow;
                    auditable.LastModifiedBy = GetUserId();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private string GetUserId() => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        try
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            builder.HasDefaultSchema("FlowSynxPluginRegistry");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}