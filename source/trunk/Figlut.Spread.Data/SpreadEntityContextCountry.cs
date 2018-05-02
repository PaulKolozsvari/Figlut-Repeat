﻿namespace Figlut.Spread.Data
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
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
