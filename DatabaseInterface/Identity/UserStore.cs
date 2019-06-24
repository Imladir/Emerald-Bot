using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Identity
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>, IUserClaimStore<User>
    {
        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);
            foreach (var c in claims)
            {
                if (!user.Claims.Any(x => x.ClaimType.Equals(c.Type) && x.ClaimType.Equals(c.Value)))
                {
                    user.Claims.Add(new UserClaim() { User = user, ClaimType = c.Type, ClaimValue = c.Value });
                }
            }
            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            using var ctx = new EmeraldBotContext();
            if (ctx.Users.SingleOrDefault(x => x.UserName.Equals(user.UserName)) != null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "DuplicateUserName", Description = "There is already a user with that name." }));
            ctx.Users.Add(user);
            ctx.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // Not here
        }

        public void Dispose()
        {
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            using var ctx = new EmeraldBotContext();
            var user = ctx.Users.SingleOrDefault(x => x.UserName.Equals(userId));
            return Task.FromResult(user);
        }

        public Task<User> FindByIdAsync(int userId, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            var user = ctx.Users.SingleOrDefault(x => x.ID == userId);
            return Task.FromResult(user);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            var res = ctx.Users.SingleOrDefault(x => x.NormalizedUserName.Equals(normalizedUserName));
            return Task.FromResult(res);
        }

        public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);
            IList<Claim> res = new List<Claim>() { new Claim("UserID", $"{user.ID}") };
            foreach (var uc in user.Claims) res.Add(uc.ToClaim());
            foreach (var r in user.Roles)
            {
                foreach (var c in r.Role.Claims)
                {
                    res.Add(c.ToClaim());
                }
            }
            return Task.FromResult(res);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.UserName);
        }

        public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            IList<User> res = ctx.Users.Where(x => x.Claims.Any(y => y.ClaimType.Equals(claim.Type) && y.ClaimValue.Equals(claim.Value))).ToList();
            return Task.FromResult(res);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);
            foreach (var c in claims)
            {
                var existing = user.Claims.SingleOrDefault(x => x.ClaimType == c.Type && x.ClaimValue == c.Value);
                if (existing != null) user.Claims.Remove(existing);
            }
            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);

            var existing = user.Claims.SingleOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            if (existing != null)
            {
                existing.ClaimType = newClaim.Type;
                existing.ClaimValue = newClaim.Value;
            }

            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);
            user.NormalizedUserName = normalizedName ?? throw new ArgumentNullException(nameof(normalizedName));
            ctx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            ctx.SaveChanges();
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);
            user.PasswordHash = passwordHash;
            ctx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            using var ctx = new EmeraldBotContext();
            ctx.Users.Attach(user);
            user.UserName = userName;
            ctx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }
                using var ctx = new EmeraldBotContext();
                ctx.Users.Attach(user);
                ctx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                ctx.SaveChanges();
                return Task.FromResult(IdentityResult.Success);
            } catch (Exception e)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "FailedUpdate", Description = $"User update: {e.Message}" }));
            }
        }
    }
}