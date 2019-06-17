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
    public class RoleStore : IRoleStore<Role>, IRoleClaimStore<Role>
    {
        private bool _disposed;
        public Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Roles.Attach(role);
            if (!role.Claims.Any(x => x.ClaimType.Equals(claim.Type) && x.ClaimType.Equals(claim.Value)))
            {
                role.Claims.Add(new RoleClaim() { Role = role, ClaimType = claim.Type, ClaimValue = claim.Value });
            }
            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            using var ctx = new EmeraldBotContext();
            if (ctx.Users.SingleOrDefault(x => x.UserName.Equals(role.Name)) != null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "DuplicateRoleName", Description = "There is already a role with that name." }));
            ctx.Roles.Add(role);
            ctx.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // Not done? Needs done?
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public Task<Role> FindByIdAsync(int roleId, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            var res = ctx.Roles.SingleOrDefault(x => x.ID == roleId);
            return Task.FromResult(res);

        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (roleId == null)
            {
                throw new ArgumentNullException(nameof(roleId));
            }
            using var ctx = new EmeraldBotContext();
            var res = ctx.Roles.SingleOrDefault(x => x.Name.Equals(roleId));
            return Task.FromResult(res);
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            var res = ctx.Roles.SingleOrDefault(x => x.NormalizedRoleName.Equals(normalizedRoleName));
            return Task.FromResult(res);
        }

        public Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Roles.Attach(role);
            IList<Claim> res = new List<Claim>();
            foreach (var rc in role.Claims) res.Add(rc.ToClaim());
            return Task.FromResult(res);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedRoleName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Name);
        }

        public Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Roles.Attach(role);
            var existing = role.Claims.SingleOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            if (existing != null) role.Claims.Remove(existing);
            ctx.SaveChanges();
            return Task.CompletedTask;
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            using var ctx = new EmeraldBotContext();
            ctx.Roles.Attach(role);
            role.NormalizedRoleName = normalizedName;
            ctx.Entry(role).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            ctx.SaveChanges();
            return Task.FromResult<object>(null);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            using var ctx = new EmeraldBotContext();
            ctx.Roles.Attach(role);
            role.Name = roleName;
            ctx.Entry(role).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            ctx.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                if (role == null)
                {
                    throw new ArgumentNullException(nameof(role));
                }
                using var ctx = new EmeraldBotContext();
                ctx.Roles.Attach(role);
                ctx.Entry(role).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                ctx.SaveChanges();
                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception e)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "FailedUpdate", Description = $"Role update: {e.Message}" }));
            }
        }
    }
}
