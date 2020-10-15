using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HandiCrafts.Core.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HandiCrafts.Web.Infrastructure.Security
{
    public class UserManager : UserManager<User>
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager _signInManager;
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            RoleManager<Role> roleManager, SignInManager signInManager, AppDbContext context
            ) :

            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IdentityResult> Insert(User user, string password, List<string> roleNames)
        {
            var result = string.IsNullOrEmpty(password) ? await CreateAsync(user) : await CreateAsync(user, password);
            if (result.Succeeded)
            {
                await AddToRolesAsync(user, roleNames);
            }

            return result;
        }
        public async Task<IdentityResult> Update(User user, List<string> roles, bool updateSensitiveFields)
        {
            IdentityResult result = null;
            var mainUser = await GetByIdWithRoles(user.Id);
            if (mainUser == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound"
                });
            }
            /*using (var trans = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted))
            {
                mainUser.Email = user.Email;
                mainUser.FirstName = user.FirstName;
                mainUser.LastName = user.LastName;
                mainUser.Gender = user.Gender;
                mainUser.Address = user.Address;
                mainUser.PhoneNumber =user.PhoneNumber;
                mainUser.UserName = user.UserName;

                if (updateSensitiveFields)
                {    
                    mainUser.IsActive = user.IsActive;

                    //update user roles    
                    var prevRoles = await GetRolesAsync(mainUser);
                    foreach (var role in prevRoles)
                    {
                        await RemoveFromRoleAsync(mainUser, role);
                    }
                    foreach (var role in roles)
                    {
                        await AddToRoleAsync(mainUser, role);
                    }
                }
                result = await UpdateAsync(mainUser);
                if (result.Succeeded)
                {
                    result = await UpdateSecurityStampAsync(mainUser);
                }

                trans.Commit();
            }*/

            return result;
        }

        public async Task UpdateWalletCredit(User user, decimal credit)
        {
            user.WalletCredit = credit;
            await Update(user, null, false);
        }


        public async Task<IdentityResult> ChangePasswordAndCookie(User user, string oldPassword, string newPassword)
        {
            var result = await ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                // reflect the changes in the Identity cookie
                await UpdateSecurityStampAsync(user).ConfigureAwait(false);
                await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            }
            return result;
        }

        public async Task<List<User>> GetAll()
        {
            var query = Users.Where(m => m.UserName.ToLower() != "admin");
            var users = await query.ToListAsync();
            await ConvertUserRoleToRole(users);
            return users;
        }
        public async Task<List<User>> GetAllUser()
        {
            var users = await Users.ToListAsync();
            return users;
        }

        public async Task<User> GetById(long id)
        {
            var user = await Users.FirstOrDefaultAsync(m => m.Id == id);
            return user;
        }

        public async Task<User> GetByIdWithRoles(long id)
        {
            var user = await GetById(id);
            await ConvertUserRoleToRole(new List<User>() { user });
            return user;
        }

        public async Task ConvertUserRoleToRole(List<User> users)
        {
            foreach (var user in users)
            {
                user.UserRoleNames = await GetRolesAsync(user) as List<string>;
            }
        }

    }

    public class AppDbContext :
    Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<
        User,
        Role,
        long,
        IdentityUserClaim<long>,
        IdentityUserRole<long>,
        IdentityUserLogin<long>,
        IdentityRoleClaim<long>,
        IdentityUserToken<long>>
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

}
