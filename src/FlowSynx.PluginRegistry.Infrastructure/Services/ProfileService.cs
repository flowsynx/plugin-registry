using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Domain.Profile;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        IDbContextFactory<ApplicationContext> appContextFactory, 
        ILogger<ProfileService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<ProfileEntity?> GetByUserId(string userId, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            var result = await context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsDeleted == false, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Add(ProfileEntity profileEntity, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            await context.Profiles
                .AddAsync(profileEntity, cancellationToken)
                .ConfigureAwait(false);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> Delete(ProfileEntity profileEntity, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            context.Profiles.Remove(profileEntity);
            context.SoftDelete(profileEntity);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}