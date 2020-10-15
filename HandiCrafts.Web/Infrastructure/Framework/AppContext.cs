using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using HandiCrafts.Web.Models;
//using AutoMapper;


namespace HandiCrafts.Web.Infrastructure.Framework
{
    /*public class AppContext : IAppContext
    {
        #region Fields

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager _userManager;
        private readonly IOptions<AppConfig> _appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private User cachedUser = null;

        #endregion

        #region Constructors

        public AppContext(IHttpContextAccessor contextAccessor,
            UserManager userManager,
            IOptions<AppConfig> appConfig,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _appConfig = appConfig;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #endregion

        #region Properties

        public User User
        {
            get
            {
                if (cachedUser == null)
                {
                    cachedUser = Task.Run(() =>
                    {
                        return GetUser();
                    }).Result;
                }

                return cachedUser;
            }
        }
        public SystemSettingsDto SystemSettings
        {
            get
            {
                if (StaticApp.SystemSettings == null)
                {
                    StaticApp.SystemSettings = Task.Run(() =>
                    {
                        return GetSystemSettings();
                    }).Result;
                }

                return StaticApp.SystemSettings;
            }
        }
        public string CultureLanguage
        {
            get
            {
                return GetCultureLanguage();
            }
        }
        
        public string Ip
        {
            get
            {
                return GetIp();
            }
        }
        
        public string SiteBaseUrl
        {
            get
            {
                return $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}";
            }
        }

        #endregion

        #region Methods

        public void ClearCache()
        {
            cachedUser = null;
        }

        public void EnsureGuestCookieRemoved()
        {
            if (_contextAccessor.HttpContext.Request.Cookies.ContainsKey(Constants.GuestUser))
            {
                _contextAccessor.HttpContext.Response.Cookies.Delete(Constants.GuestUser);
            }
        }

        public bool TryGetGuestUserId(out long guestId)
        {
            if (_contextAccessor.HttpContext.Request.Cookies.TryGetValue(Constants.GuestUser, out string guestIdStr) &&
                long.TryParse(guestIdStr, out guestId))
            {
                return true;
            }
            else
            {
                guestId = 0;
                return false;
            }
        }

        public async Task<decimal> ConvertToSystemCurrency(decimal sourceAmount, CurrencyDto source)
        {
            return await ConvertCurrency(sourceAmount, source, SystemSettings.SystemCurrency);
        }

        public async Task<decimal> ConvertCurrency(decimal sourceAmount, CurrencyDto source, CurrencyDto target)
        {
            try
            {

                if (source.Id == target.Id)
                    return sourceAmount;

                if (target.CurrencyRelations != null && target.CurrencyRelations.Any(x => x.TargetCurrencyId == source.Id))
                {
                    return sourceAmount / target.CurrencyRelations.First(x => x.TargetCurrencyId == source.Id).Rate;
                }

                if (source.CurrencyRelations != null && source.CurrencyRelations.Any(x => x.TargetCurrencyId == target.Id))
                {
                    return sourceAmount * source.CurrencyRelations.First(x => x.TargetCurrencyId == target.Id).Rate;
                }

                if (SystemSettings != null && SystemSettings.ReferenceCurrency != null)
                {
                    var relations = await _dbContext.CurrencyRelations.AsNoTracking().Include(x => x.TargetCurrency).Where(x => x.SourceId == SystemSettings.ReferenceCurrency.Id).Select(x => x.TargetCurrency).ToListAsync();
                    if (relations.Any(x => x.Id == target.Id))
                    {
                        return sourceAmount * source.Rate * relations.First(x => x.Id == target.Id).Rate;
                    }
                }

                return target.Rate == 0 ? 0 : sourceAmount * (source.Rate / target.Rate);

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region Utilities
        private async Task<SystemSettingsDto> GetSystemSettings()
        {
            var settings = await _dbContext.SystemSettings.AsNoTracking().Include(x => x.ReferenceCurrency).Include(x => x.SystemCurrency).FirstOrDefaultAsync();
            return _mapper.Map<SystemSettingsDto>(settings);
        }

        private async Task<User> GetUser()
        {
            if (cachedUser != null)
            {
                return cachedUser;
            }

            if (_contextAccessor.HttpContext == null)
            {
                return null;
            }

            var principal = _contextAccessor.HttpContext.User;
            if (principal != null)
            {
                var username = _userManager.NormalizeName(principal.Identity.Name);
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == username);
                if (user != null)
                {
                    await _userManager.ConvertUserRoleToRole(new List<User>() { user });
                    cachedUser = user;
                    return user;
                }
            }
            return null;
        }
        private string GenerateUsername(int maxLength)
        {
            var rng = RandomNumberGenerator.Create();
            // Generate a cryptographic random number
            var buff = new byte[maxLength];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            var code = Convert.ToBase64String(buff);
            var username = string.Empty;

            foreach (var c in code)
            {
                if (char.IsLetterOrDigit(c))
                {
                    username += c.ToString();
                }
            }

            return username;
        }

        private string GetCultureLanguage()
        {
            if (_contextAccessor.HttpContext.Request.Cookies.ContainsKey(Constants.CultureLanguage))
            {
                return _contextAccessor.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == Constants.CultureLanguage).Value;
            }
            return "";
        }
        
        private string GetIp()
        {
            return _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        #endregion
    }*/
}
