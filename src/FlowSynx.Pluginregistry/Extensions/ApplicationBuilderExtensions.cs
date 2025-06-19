using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;

namespace FlowSynx.Pluginregistry.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> EnsureApplicationDatabaseCreated(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var result = context.Database.EnsureCreated();
            if (result)
                logger.LogInformation("Application database created successfully.");
            else
                logger.LogInformation("Application database already exists.");

            await Initialize(context);
            return app;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while creating the application database: {ex.Message}");
        }
    }

    public static async Task Initialize(ApplicationContext context)
    {
        if (!context.PluginCategories.Any())
        {
            var categories = new List<PluginCategoryEntity>
            {
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("c519149a-a13e-4210-898c-c199b13eb8c5"),
                    CategoryId = "web-api",
                    Title = "Web & API Plugins",
                    Description = "Plugins for web services and API integrations"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("e15e0724-977f-4f9d-92fe-f3fdf8c32bc3"),
                    CategoryId = "cloud",
                    Title = "Cloud Platform Plugins",
                    Description = "Plugins for integrating with cloud platforms such as AWS, Azure, GCP"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("72d186f0-b05a-43b5-84eb-218b111066b1"),
                    CategoryId = "enterprise-erp",
                    Title = "Enterprise Software & ERP Plugins",
                    Description = "Plugins for integrating with enterprise and ERP systems"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("7981b309-9d3c-449e-99c1-65170aa8c747"),
                    CategoryId = "data-platform-bi",
                    Title = "Data Platform & BI Plugins",
                    Description = "Plugins for data platforms and business intelligence tools\""
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("163910c5-c4e5-4f80-a877-0626a1e8602f"),
                    CategoryId = "communication",
                    Title = "Communication & Collaboration Plugins",
                    Description = "Plugins for email, messaging, and collaboration tools"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("6f085a72-8695-47ad-b194-613c7b926c24"),
                    CategoryId = "devops",
                    Title = "DevOps & CI/CD Tool Plugins",
                    Description = "Plugins for DevOps tools and continuous integration/delivery systems"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("fe560e84-e2de-4f35-b438-09c0230c7eda"),
                    CategoryId = "project-workflow",
                    Title = "Project & Workflow Management Plugins",
                    Description = "Plugins for project tracking and workflow management systems"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("c2b74292-46fc-414d-adac-464876d44aec"),
                    CategoryId = "storage-transfer",
                    Title = "Storage & File Transfer Plugins",
                    Description = "Plugins for file storage, transfer, and synchronization"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("cfdbae20-173d-4667-9a8b-05fa97af0bc4"),
                    CategoryId = "identity-auth",
                    Title = "Identity & Authentication Plugins",
                    Description = "Plugins for identity management and authentication services"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("c69fdc3e-5aa1-4190-a4f8-a79f988bb411"),
                    CategoryId = "ai-ml",
                    Title = "AI & ML Plugins",
                    Description = "Plugins for integrating artificial intelligence and machine learning services"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("91e983fc-7b31-4e58-a40f-4a7f70f66e88"),
                    CategoryId = "monitoring",
                    Title = "Monitoring, Observability & Logging Plugins",
                    Description = "Plugins for system monitoring, observability, and logging"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("34a072bd-bfc7-492c-9cef-e0a21a53ffc1"),
                    CategoryId = "testing-quality",
                    Title = "Testing & Quality Plugins",
                    Description = "Plugins for software testing, QA automation, and code quality analysis"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("eb3a4607-5729-4612-93bd-e39286077c89"),
                    CategoryId = "finance",
                    Title = "Financial & Payment Plugins",
                    Description = "Plugins for finance systems and payment gateways"
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("0ed2e4ba-86e2-4b5e-b760-5210cbdb4660"),
                    CategoryId = "blockchain",
                    Title = "Blockchain & Web3 Plugins",
                    Description = "Plugins for blockchain protocols, crypto wallets, and Web3 services"
                },
            };

            context.PluginCategories.AddRange(categories);
            await context.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}