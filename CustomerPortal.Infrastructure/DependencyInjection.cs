using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Settings;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using CustomerPortal.Infrastructure.Providers.Hashing;
using CustomerPortal.Infrastructure.Providers.OtpSender;
using CustomerPortal.Infrastructure.Providers.Security;
using CustomerPortal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerPortal.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSettings(configuration);

            var connectionString = ConnectionStringResolver.Resolve(configuration);
            var databaseSettings = configuration
                .GetSection(ApplicationConstant.ConfigurationSections.Database)
                .Get<DatabaseSettings>()
                ?? throw new InvalidOperationException(
                    $"The '{ApplicationConstant.ConfigurationSections.Database}' configuration section is missing.");

            services.AddDbContext<CustomerPortalDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Transient faults are routine against Azure SQL; without this they surface
                    // to the customer as 500s.
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: databaseSettings.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(databaseSettings.MaxRetryDelaySeconds),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(databaseSettings.CommandTimeoutSeconds);
                }));

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            services.AddRepositories();
            services.AddProviders();

            return services;
        }

        private static IServiceCollection AddSettings(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(
                configuration.GetSection(ApplicationConstant.ConfigurationSections.Database));
            services.Configure<HashingSettings>(
                configuration.GetSection(ApplicationConstant.ConfigurationSections.Hashing));
            services.Configure<OnboardingSettings>(
                configuration.GetSection(ApplicationConstant.ConfigurationSections.Onboarding));
            services.Configure<OtpSettings>(
                configuration.GetSection(ApplicationConstant.ConfigurationSections.Otp));
            services.Configure<PinSettings>(
                configuration.GetSection(ApplicationConstant.ConfigurationSections.Pin));
            services.Configure<RateLimitSettings>(
                configuration.GetSection(ApplicationConstant.ConfigurationSections.RateLimiting));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IConsentDocumentRepository, ConsentDocumentRepository>();
            services.AddScoped<IHomeContentCardRepository, HomeContentCardRepository>();
            services.AddScoped<IOnboardingRequirementRepository, OnboardingRequirementRepository>();
            services.AddScoped<IOtpChallengeRepository, OtpChallengeRepository>();
            services.AddScoped<IUserConsentRepository, UserConsentRepository>();
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            services.AddScoped<IUserPinRepository, UserPinRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        private static IServiceCollection AddProviders(this IServiceCollection services)
        {
            // Stateless and holding no scoped dependency, so these are safe as singletons.
            services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
            services.AddSingleton<ISecretHasher, Argon2SecretHasher>();
            services.AddSingleton<ISecureCodeGenerator, SecureCodeGenerator>();
            services.AddSingleton<IOtpSender, OtpSenderProvider>();

            return services;
        }
    }
}
