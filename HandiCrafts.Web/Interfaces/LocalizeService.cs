
using System.Threading.Tasks;

namespace HandiCrafts.Web.Interfaces
{
    public class LocalizeService: ILocalizeService
    {
        #region Fields

        #endregion

        #region Constructors

        public LocalizeService()
        {
        }

        #endregion

        #region Methods

        public Task<string> Localize(string text)
        {
            return Task.FromResult(text);
        }
        
        #endregion

        #region Utilities

       
        #endregion

    }
}
