using CustomerPortal.Application.Services.Biometric;
using CustomerPortal.Application.Services.Consent;
using CustomerPortal.Application.Services.Home;
using CustomerPortal.Application.Services.Migration;
using CustomerPortal.Application.Services.Onboarding;
using CustomerPortal.Application.Services.Otp;
using CustomerPortal.Application.Services.Pin;
using CustomerPortal.Application.Services.Registration;
using CustomerPortal.Application.Services.UserProfile;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerPortal.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<IBiometricService, BiometricService>();
            services.AddScoped<IConsentService, ConsentService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IMigrationService, MigrationService>();
            services.AddScoped<IOnboardingService, OnboardingService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IPinService, PinService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IUserProfileService, UserProfileService>();

            return services;
        }
    }
}
