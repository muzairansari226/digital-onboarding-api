using System.ComponentModel.DataAnnotations;

namespace CustomerPortal.Application.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class NotEmptyGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
            => value switch
            {
                null => false,
                Guid guid => guid != Guid.Empty,
                _ => false
            };
    }
}
