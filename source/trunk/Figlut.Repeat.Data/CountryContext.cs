namespace Figlut.Repeat.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Country

        public Country GetCountry(Guid countryId, bool throwExceptionOnNotFound)
        {
            Country result = (from c in DB.GetTable<Country>()
                              where c.CountryId == countryId
                              select c).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Country).Name,
                    EntityReader<Country>.GetPropertyName(p => p.CountryId, false),
                    countryId.ToString()));
            }
            return result;
        }

        public Country GetCountryByCountryCode(string countryCode, bool throwExceptionOnNotFound)
        {
            Country result = (from c in DB.GetTable<Country>()
                              where c.CountryCode.ToLower() == countryCode
                              select c).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Country).Name,
                    EntityReader<Country>.GetPropertyName(p => p.CountryCode, false),
                    countryCode.ToString()));
            }
            return result;
        }

        public long GetAllCountryCount()
        {
            return DB.GetTable<Country>().LongCount();
        }

        public List<Country> GetCountriesByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Country> result = (from c in DB.GetTable<Country>()
                                    where c.CountryName.ToLower().Contains(searchFilterLower) ||
                                    c.CountryCode.ToLower().Contains(searchFilterLower)
                                    orderby c.CountryName
                                    select c).ToList();
            return result;
        }

        public void DeleteCountriesByFilter(string searchFilter)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Country> countries = GetCountriesByFilter(searchFilter);
                DB.GetTable<Country>().DeleteAllOnSubmit(countries);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Country
    }
}
