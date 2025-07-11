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
                    CategoryId = "ai",
                    Title = "Artificial Intelligence plugins",
                    Description = "Plugins that offer artificial intelligence capabilities such as computer vision, natural language processing, or reasoning engines."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("e15e0724-977f-4f9d-92fe-f3fdf8c32bc3"),
                    CategoryId = "api",
                    Title = "Application Programming Interfaces plugins",
                    Description = "Plugins that provide or consume Application Programming Interfaces (APIs), including REST, GraphQL, or RPC-based services."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("72d186f0-b05a-43b5-84eb-218b111066b1"),
                    CategoryId = "authentication",
                    Title = "Authentication and Autherization plugins",
                    Description = "Plugins that handle identity and access management (IAM), SSO, OAuth, or multi-factor authentication services."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("7981b309-9d3c-449e-99c1-65170aa8c747"),
                    CategoryId = "businessintelligence",
                    Title = "Business Intelligence plugins",
                    Description = "Plugins that help in collecting, analyzing, and visualizing business data to support decision-making."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("163910c5-c4e5-4f80-a877-0626a1e8602f"),
                    CategoryId = "blockchain",
                    Title = "Blockchain and Web3 plugins",
                    Description = "Plugins that specifically target blockchain protocols, nodes, ledger manipulation, or tokenomics, beyond just Web3."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("6f085a72-8695-47ad-b194-613c7b926c24"),
                    CategoryId = "cloud",
                    Title = "Cloud service providers plugins",
                    Description = "Plugins that enable integration with cloud service providers such as AWS, Azure, Google Cloud, and others."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("cfdbae20-173d-4667-9a8b-05fa97af0bc4"),
                    CategoryId = "communication",
                    Title = "Communication & Collaboration plugins",
                    Description = "Plugins that handle messaging, chat, email, VoIP, SMS, or other communication protocols and services."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("0ed2e4ba-86e2-4b5e-b760-5210cbdb4660"),
                    CategoryId = "data",
                    Title = "Data processing plugins",
                    Description = "Plugins for data processing, transformation, pipelines, and data flow orchestration."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("18dbd19e-b651-416f-a172-be284db8ac7c"),
                    CategoryId = "database",
                    Title = "Database management system plugins",
                    Description = "Plugins that provide access to or manage relational and non-relational database systems like PostgreSQL, MySQL, MongoDB, etc."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("fe560e84-e2de-4f35-b438-09c0230c7eda"),
                    CategoryId = "devops",
                    Title = "DevOps & CI/CD plugins",
                    Description = "Plugins that assist with infrastructure automation, CI/CD pipelines, configuration management, and deployment tooling."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("eb3a4607-5729-4612-93bd-e39286077c89"),
                    CategoryId = "finance",
                    Title = "Financial & Payment plugins",
                    Description = "Plugins related to financial systems, transactions, accounting, billing, and related computations."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("547356f1-f81d-4faa-9de4-dd05fbdbcc90"),
                    CategoryId = "ml",
                    Title = "Machine Learning plugins",
                    Description = "Plugins that implement or assist with machine learning workflows, including model training, evaluation, and prediction."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("91e983fc-7b31-4e58-a40f-4a7f70f66e88"),
                    CategoryId = "monitoring",
                    Title = "Monitoring & Observability plugins",
                    Description = "Plugins that observe the health, performance, and uptime of applications and infrastructure."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("c2b74292-46fc-414d-adac-464876d44aec"),
                    CategoryId = "logging",
                    Title = "Logging plugins",
                    Description = "Plugins that capture logs, aggregate them, parse, and send them to external logging systems or dashboards."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("34a072bd-bfc7-492c-9cef-e0a21a53ffc1"),
                    CategoryId = "networking",
                    Title = "Networking plugins",
                    Description = "Plugins that support network operations, diagnostics, connectivity, DNS, routing, firewalls, and load balancers."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("c5e6e8fc-3408-4f40-903c-6d3985957b6e"),
                    CategoryId = "projectworkflow",
                    Title = "Project & Workflow Management plugins",
                    Description = "Plugins that support task management, project planning, issue tracking, or general workflow automation."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("b0874841-e20f-4ba4-865f-b63a3e8aaf5c"),
                    CategoryId = "resourceplanning",
                    Title = "Enterprise Software & ERP Plugins",
                    Description = "Plugins for managing company resources such as HR, inventory, scheduling, budgeting, or operations planning."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("3fe9ed42-daae-483e-881f-dbae0d5aa90f"),
                    CategoryId = "security",
                    Title = "Security Plugins",
                    Description = "Plugins that enhance security features such as encryption, vulnerability scanning, firewalls, or threat detection."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("bce5c475-fc1f-4b16-bdc0-fda857f3a4d0"),
                    CategoryId = "storage",
                    Title = "Storage & File system plugins",
                    Description = "Plugins for file storage, blob storage, object storage, or distributed storage systems."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("340106eb-5aa6-4916-bc9b-eab8f2edefbb"),
                    CategoryId = "testing",
                    Title = "Testing & Quality plugins",
                    Description = "Plugins that run or support automated and manual tests, including unit, integration, and end-to-end testing tools."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("bf722954-89da-4c14-802c-53ae6a9adf2f"),
                    CategoryId = "web",
                    Title = "Web service/application plugins",
                    Description = "Plugins that build and serve websites or web applications, including frontend and backend frameworks."
                },
                new PluginCategoryEntity
                {
                    Id = Guid.Parse("64e2474c-9a6a-43fc-b202-a42c1cf71019"),
                    CategoryId = "controlflow",
                    Title = "ControlFlow plugins",
                    Description = "Plugins that provide control-flow constructs for workflows, such as conditional branching, loops, parallel execution, and event-driven triggers. These plugins are used to control the execution logic of workflow tasks."
                },
            };

            context.PluginCategories.AddRange(categories);
            await context.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}