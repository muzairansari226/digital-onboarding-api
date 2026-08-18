using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IOtpChallengeRepository
    {
        Task<OtpChallenge?> GetLatestUnconsumedAsync(Guid fkUser, int channel);

        Task<OtpChallenge?> GetLatestAsync(Guid fkUser, int channel);

        Task<int> CountIssuedSinceAsync(Guid fkUser, int channel, DateTime issuedSince);

        Task<Guid> AddAsync(OtpChallenge otpChallenge);

        Task<bool> UpdateAsync(OtpChallenge otpChallenge);

        Task<int> ConsumeOutstandingAsync(Guid fkUser, int channel, DateTime consumedAt);
    }
}
