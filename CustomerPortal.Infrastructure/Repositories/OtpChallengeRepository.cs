using CustomerPortal.Application.Contracts;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class OtpChallengeRepository : IOtpChallengeRepository
    {
        private readonly CustomerPortalDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public OtpChallengeRepository(
            CustomerPortalDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<OtpChallenge?> GetLatestUnconsumedAsync(Guid fkUser, int channel)
            => _context.OtpChallenges
                .Where(challenge => challenge.FkUser == fkUser
                    && challenge.Channel == channel
                    && challenge.ConsumedAt == null)
                .OrderByDescending(challenge => challenge.CreatedAt)
                .FirstOrDefaultAsync();

        public Task<OtpChallenge?> GetLatestAsync(Guid fkUser, int channel)
            => _context.OtpChallenges
                .Where(challenge => challenge.FkUser == fkUser && challenge.Channel == channel)
                .OrderByDescending(challenge => challenge.CreatedAt)
                .FirstOrDefaultAsync();

        public Task<int> CountIssuedSinceAsync(Guid fkUser, int channel, DateTime issuedSince)
            => _context.OtpChallenges
                .CountAsync(challenge => challenge.FkUser == fkUser
                    && challenge.Channel == channel
                    && challenge.CreatedAt >= issuedSince);

        public async Task<Guid> AddAsync(OtpChallenge otpChallenge)
        {
            otpChallenge.RecId = Guid.NewGuid();
            otpChallenge.CreatedAt = _dateTimeProvider.UtcNow;

            _context.OtpChallenges.Add(otpChallenge);
            await _context.SaveChangesAsync();

            return otpChallenge.RecId;
        }

        public async Task<bool> UpdateAsync(OtpChallenge otpChallenge)
        {
            _context.OtpChallenges.Update(otpChallenge);

            return await _context.SaveChangesAsync() > 0;
        }

        public Task<int> ConsumeOutstandingAsync(Guid fkUser, int channel, DateTime consumedAt)
            => _context.OtpChallenges
                .Where(challenge => challenge.FkUser == fkUser
                    && challenge.Channel == channel
                    && challenge.ConsumedAt == null)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(challenge => challenge.ConsumedAt, consumedAt));
    }
}
