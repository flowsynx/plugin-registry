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
                .FirstOrDefaultAsync(p => p.UserId.ToLower() == userId.ToLower() && p.IsDeleted == false, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<(int PluginCount, int DownloadCount, string? ProfileEmail)> GetPluginStatisticsByUsernameAsync(string username, 
        CancellationToken cancellationToken = default)
    {
        await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Profiles
            .Where(p => p.UserName == username)
            .SelectMany(p => p.Owners
                .Where(o => !o.Plugin!.IsDeleted)
                .Select(o => o.Plugin!));

        var pluginCount = await query.CountAsync(cancellationToken);

        var downloadCount = await query
            .SelectMany(p => p.Versions.Where(v => !v.IsDeleted))
            .SelectMany(v => v.Statistics)
            .CountAsync(cancellationToken);

        var email = await context.Profiles.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        return (pluginCount, downloadCount, email?.Email);
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