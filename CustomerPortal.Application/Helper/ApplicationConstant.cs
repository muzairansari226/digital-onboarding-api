namespace CustomerPortal.Application.Helper
{
    public static class ApplicationConstant
    {
        public static class ApiVersions
        {
            public const string V1 = "1.0";
        }

        public static class StatusCodes
        {
            public const int Ok = 200;
            public const int Created = 201;
            public const int BadRequest = 400;
            public const int NotFound = 404;
            public const int Conflict = 409;
            public const int Locked = 423;
            public const int TooManyRequests = 429;
            public const int InternalServerError = 500;
            public const int BadGateway = 502;
        }

        public static class ApiRoutes
        {
            public const string Base = "api/v{version:apiVersion}";

            public const string Onboarding = Base + "/onboarding";
            public const string Registration = Base + "/registration";
            public const string Migration = Base + "/migration";
            public const string Otp = Base + "/otp";
            public const string Consents = Base + "/consents";
            public const string Pin = Base + "/pin";
            public const string Biometric = Base + "/biometric";
            public const string Users = Base + "/users";
            public const string Home = Base + "/home";
        }
        
        public static class ConfigurationSections
        {
            public const string AllowedOrigins = "AllowedOrigins";
            public const string EnvironmentVariables = "EnvironmentVariables";
            public const string SqlConnectionVariableName = "EnvironmentVariables:SqlConnection";
            public const string Database = "Database";
            public const string Hashing = "Hashing";
            public const string Onboarding = "Onboarding";
            public const string Otp = "Otp";
            public const string Pin = "Pin";
            public const string RateLimiting = "RateLimiting";
        }

        public static class CorsPolicies
        {
            public const string Default = "Default";
        }

        public static class RateLimitPolicies
        {
            public const string OtpSend = "OtpSend";
            public const string OtpVerify = "OtpVerify";
            public const string PinVerify = "PinVerify";
            public const string RegistrationStart = "RegistrationStart";
        }

        public static class Swagger
        {
            public const string DocumentName = "v1";
            public const string Title = "Digital Onboarding API";
            public const string Version = "v1";
            public const string EndpointPath = "/swagger/v1/swagger.json";
        }

        public static class ResponseHeaders
        {
            public const string ContentTypeOptions = "X-Content-Type-Options";
            public const string ContentTypeOptionsValue = "nosniff";
            public const string CacheControl = "Cache-Control";
            public const string CacheControlValue = "no-store, no-cache";
        }

        public static class Masking
        {
            public const char MaskCharacter = '*';
            public const int VisibleMobileDigits = 3;
        }

        public static class RegularExpressions
        {
            //E.164: a leading plus, then 8 to 15 digits.
            public const string Mobile = @"^\+[1-9]\d{7,14}$";

            //Digits only, used for OTP codes and PINs.
            public const string NumericOnly = @"^\d+$";

            //A person's name: letters, spaces, hyphens and apostrophes.
            public const string PersonName = @"^[\p{L}][\p{L}\s'\-]*$";
        }

        public static class FieldLengths
        {
            public const int Name = 100;
            public const int Email = 256;
            public const int Mobile = 20;
            public const int DeviceId = 128;
            public const int DeviceName = 100;
            public const int IpAddress = 45;
        }

        public static class PinRules
        {
            public const string AscendingDigits = "0123456789";
            public const string DescendingDigits = "9876543210";
        }

        public static class ValidationMessages
        {
            public const string FirstNameRequired = "First name is required.";
            public const string FirstNameInvalid = "First name contains unsupported characters.";
            public const string LastNameRequired = "Last name is required.";
            public const string LastNameInvalid = "Last name contains unsupported characters.";
            public const string EmailRequired = "Email address is required.";
            public const string EmailInvalid = "Enter a valid email address.";
            public const string MobileRequired = "Mobile number is required.";
            public const string MobileInvalid = "Enter the mobile number in international format, for example +971501234567.";
            public const string ContactDetailRequired = "Provide an email address or a mobile number.";
            public const string RegistrationIdRequired = "Registration id is required.";
            public const string ChannelRequired = "Delivery channel is required.";
            public const string ChannelInvalid = "Delivery channel must be Mobile or Email.";
            public const string CodeRequired = "Verification code is required.";
            public const string CodeInvalid = "The verification code must contain digits only.";
            public const string PinRequired = "PIN is required.";
            public const string PinInvalid = "The PIN must contain digits only.";
            public const string DocumentIdRequired = "Document id is required.";
            public const string DocumentTypeInvalid = "Document type is not recognised.";
            public const string AcceptanceRequired = "The document must be accepted to continue.";
            public const string DeviceIdRequired = "Device id is required.";
            public const string UserIdRequired = "User id is required.";
            public const string RequestBodyInvalid = "The request body could not be read. Check that every field has the expected type.";
        }

        public static class ResponseMessages
        {
            // Registration and migration
            public const string AccountAlreadyExists = "An account already exists for these details.";
            public const string EmailAlreadyRegistered = "An account already exists for this email address.";
            public const string MobileAlreadyRegistered = "An account already exists for this mobile number.";
            public const string DetailsAvailable = "These details are available.";
            public const string RegistrationStarted = "Registration started.";
            public const string MigrationStarted = "Existing account found.";
            public const string MigrationAccountNotFound = "We could not find an existing account for that mobile number.";
            public const string MigrationAlreadyCompleted = "This account has already completed onboarding.";

            // Users and content
            public const string UserNotFound = "We could not find that account.";
            public const string ProfileRetrieved = "Profile retrieved successfully.";
            public const string ProfileUpdated = "Profile updated successfully.";
            public const string DevicesRetrieved = "Devices retrieved successfully.";
            public const string RequirementsRetrieved = "Requirements retrieved successfully.";
            public const string OnboardingStatusRetrieved = "Onboarding status retrieved successfully.";
            public const string HomeRetrieved = "Home content retrieved successfully.";

            // OTP
            public const string OtpSent = "Verification code sent.";
            public const string OtpVerified = "Verification successful.";
            public const string OtpChannelAlreadyVerified = "This channel has already been verified.";
            public const string OtpNotFound = "Request a verification code to continue.";
            public const string OtpExpired = "That code has expired. Request a new one.";
            public const string OtpAttemptsExhausted = "Too many incorrect attempts. Request a new code.";
            public const string OtpSendFailed = "We could not send the verification code. Please try again.";
            public const string OtpIncorrectFormat = "Incorrect code. You have {0} attempt(s) remaining.";
            public const string OtpResendCooldownFormat = "Please wait {0} more second(s) before requesting another code.";
            public const string OtpResendLimitFormat = "You have requested too many codes. Try again in {0} minute(s).";

            // Consent
            public const string ConsentDocumentNotFound = "The requested document is not available.";
            public const string ConsentDocumentRetrieved = "Document retrieved successfully.";
            public const string ConsentRecorded = "Consent recorded.";
            public const string ConsentAlreadyRecorded = "Consent has already been recorded for this document.";
            public const string ConsentRequired = "Accept the privacy policy before continuing.";

            // PIN
            public const string PinSet = "PIN created successfully.";
            public const string PinAlreadySet = "A PIN has already been set for this account.";
            public const string PinNotSet = "No PIN has been set for this account.";
            public const string PinVerified = "PIN verified successfully.";
            public const string PinTooWeak = "Choose a less predictable PIN - avoid repeated digits and simple sequences.";
            public const string PinLengthFormat = "The PIN must be exactly {0} digits.";
            public const string PinIncorrectFormat = "Incorrect PIN. You have {0} attempt(s) remaining.";
            public const string PinLockedFormat = "Too many incorrect attempts. Try again in {0} minute(s).";

            // Biometric
            public const string BiometricEnrolled = "Biometric login enabled for this device.";

            // Flow preconditions
            public const string MobileVerificationRequired = "Verify your mobile number before continuing.";
            public const string EmailVerificationRequired = "Verify your email address before continuing.";

            // Infrastructure
            public const string ValidationFailed = "One or more validation errors occurred.";
            public const string TooManyRequests = "Too many requests. Please slow down and try again shortly.";
            public const string UnexpectedError = "An unexpected error occurred. Please contact support with the reference below.";
            public const string ReferenceFormat = "Reference: {0}";
        }

        public static class LogMessages
        {
            public const string UnhandledException =
                "Unhandled exception. CorrelationId={CorrelationId} Method={Method} Path={Path}";

            public const string OtpDispatched =
                "OTP dispatched. Channel={Channel} Destination={MaskedDestination}";

            public const string OtpDispatchPending =
                "OTP delivery provider is not configured; the code was generated but not transmitted. Channel={Channel} Destination={MaskedDestination}";
        }
    }
}
