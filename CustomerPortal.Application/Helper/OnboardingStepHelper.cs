using CustomerPortal.Application.Enums;

namespace CustomerPortal.Application.Helper
{
    public static class OnboardingStepHelper
    {
        public static EnOnboardingStep Resolve(
            bool isMobileVerified,
            bool isEmailVerified,
            bool isConsentAccepted,
            bool isPinSet,
            bool isBiometricEnrolled)
        {
            if (!isMobileVerified)
            {
                return EnOnboardingStep.MobileVerification;
            }

            if (!isEmailVerified)
            {
                return EnOnboardingStep.EmailVerification;
            }

            if (!isConsentAccepted)
            {
                return EnOnboardingStep.Consent;
            }

            if (!isPinSet)
            {
                return EnOnboardingStep.PinSetup;
            }

            if (!isBiometricEnrolled)
            {
                return EnOnboardingStep.BiometricEnrolment;
            }

            return EnOnboardingStep.Completed;
        }
    }
}
