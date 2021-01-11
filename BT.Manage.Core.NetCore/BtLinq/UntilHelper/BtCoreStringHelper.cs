using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace BT.Manage.Core
{
    internal class BtCoreStringHelper
    {
        public static string ToPlural(string word)
        {
            var service = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en-us"));
            if (service.IsPlural(word))
            {
                return word;
            }
            return service.Pluralize(word);
        }

        public static string ToSingular(string word)
        {
            var service = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en-us"));
            if (service.IsSingular(word))
            {
                return word;
            }
            return service.Singularize(word);
        }
    }
}