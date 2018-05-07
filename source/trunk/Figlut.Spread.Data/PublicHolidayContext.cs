namespace Figlut.Spread.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Server.Toolkit.Data;
    using System.Transactions;
    using Figlut.Spread.ORM.Views;
    using Figlut.Server.Toolkit.Data.iCalendar;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Public Holiday

        public PublicHoliday GetPublicHoliday(Guid publicHolidayId, bool throwExceptionOnNotFound)
        {
            PublicHoliday result = (from p in DB.GetTable<PublicHoliday>()
                                    where p.PublicHolidayId == publicHolidayId
                                    select p).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(PublicHoliday).Name,
                    EntityReader<PublicHoliday>.GetPropertyName(p => p.PublicHolidayId, false),
                    publicHolidayId.ToString()));
            }
            return result;
        }

        public long GetAllPublicHolidayCount()
        {
            return DB.GetTable<PublicHoliday>().LongCount();
        }

        public PublicHolidayView GetPublicHolidayView(Guid publicHolidayId, bool throwExceptionOnNotFound)
        {
            PublicHolidayView result = (from p in DB.GetTable<PublicHoliday>()
                                        join c in DB.GetTable<Country>() on p.CountryId equals c.CountryId into set
                                        from sub in set.DefaultIfEmpty()
                                        where p.PublicHolidayId == publicHolidayId
                                        select new PublicHolidayView()
                                        {
                                            PublicHolidayId = Guid.NewGuid(),
                                            CountryId = p.CountryId,
                                            EventName = p.EventName,
                                            DateIdentifier = p.DateIdentifier,
                                            Year = p.Year,
                                            Month = p.Month,
                                            Day = p.Day,
                                            HolidayDate = p.HolidayDate,
                                            DateCreated = p.DateCreated,
                                            CountryCode = sub.CountryCode,
                                            CountryName = sub.CountryName
                                        }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(PublicHolidayView).Name,
                    EntityReader<PublicHolidayView>.GetPropertyName(p => p.PublicHolidayId, false),
                    publicHolidayId.ToString()));
            }
            return result;
        }

        public List<PublicHolidayView> GetPublicHolidaysViewByFilter(string searchFilter, Nullable<Guid> countryId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<PublicHolidayView> result = null;
            if (countryId.HasValue)
            {
                result = (from p in DB.GetTable<PublicHoliday>()
                          join c in DB.GetTable<Country>() on p.CountryId equals c.CountryId into set
                          from sub in set
                          where p.CountryId == countryId.Value &&
                          (p.EventName.ToLower().Contains(searchFilterLower) ||
                          p.DateIdentifier.ToLower().Contains(searchFilterLower))
                          orderby p.HolidayDate ascending
                          select new PublicHolidayView()
                          {
                              PublicHolidayId = Guid.NewGuid(),
                              CountryId = p.CountryId,
                              EventName = p.EventName,
                              DateIdentifier = p.DateIdentifier,
                              Year = p.Year,
                              Month = p.Month,
                              Day = p.Day,
                              HolidayDate = p.HolidayDate,
                              DateCreated = p.DateCreated,
                              CountryCode = sub.CountryCode,
                              CountryName = sub.CountryName
                          }).ToList();
            }
            else
            {
                result = (from p in DB.GetTable<PublicHoliday>()
                          join c in DB.GetTable<Country>() on p.CountryId equals c.CountryId into set
                          from sub in set
                          where (p.EventName.ToLower().Contains(searchFilterLower) ||
                          p.DateIdentifier.ToLower().Contains(searchFilterLower))
                          orderby p.HolidayDate ascending
                          select new PublicHolidayView()
                          {
                              PublicHolidayId = Guid.NewGuid(),
                              CountryId = p.CountryId,
                              EventName = p.EventName,
                              DateIdentifier = p.DateIdentifier,
                              Year = p.Year,
                              Month = p.Month,
                              Day = p.Day,
                              HolidayDate = p.HolidayDate,
                              DateCreated = p.DateCreated,
                              CountryCode = sub.CountryCode,
                              CountryName = sub.CountryName
                          }).ToList();
            }
            return result;
        }

        public List<PublicHoliday> GetPublicHolidaysByFilter(string searchFilter, Nullable<Guid> countryId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<PublicHoliday> result = null;
            if (countryId.HasValue)
            {
                result = (from p in DB.GetTable<PublicHoliday>()
                          where p.CountryId == countryId.Value &&
                          (p.DateIdentifier.ToLower().Contains(searchFilterLower) ||
                          p.EventName.ToLower().Contains(searchFilterLower))
                          orderby p.HolidayDate ascending
                          select p).ToList();
            }
            else
            {
                result = (from p in DB.GetTable<PublicHoliday>()
                          where p.DateIdentifier.ToLower().Contains(searchFilterLower) ||
                          p.EventName.ToLower().Contains(searchFilterLower)
                          orderby p.HolidayDate ascending
                          select p).ToList();
            }
            return result;
        }

        public void DeletePublicHolidaysByFilter(string searchFilter, Nullable<Guid> countryId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<PublicHoliday> publicHolidays = GetPublicHolidaysByFilter(searchFilter, countryId);
                DB.GetTable<PublicHoliday>().DeleteAllOnSubmit(publicHolidays);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        private PublicHoliday GetPublicHolidayByCountry(string countryCode, string dateIdentifier, bool throwExceptionOnNotFound)
        {
            PublicHoliday result = (from p in DB.GetTable<PublicHoliday>()
                                    join c in DB.GetTable<Country>() on p.CountryId equals c.CountryId into set
                                    from sub in set
                                    where sub.CountryCode.ToLower() == countryCode && p.DateIdentifier == dateIdentifier
                                    select p).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}' and {3} of '{4}'.",
                    DataShaper.ShapeCamelCaseString(typeof(PublicHoliday).Name),
                    EntityReader<Country>.GetPropertyName(p => p.CountryCode, false),
                    countryCode.ToString(),
                    EntityReader<PublicHoliday>.GetPropertyName(p => p.DateIdentifier, false)));
            }
            return result;
        }

        public void SavePublicHolidaysFromICalCalendar(ICalCalendar calendar)
        {
            using (TransactionScope t = new TransactionScope())
            {
                Country country = GetCountryByCountryCode(calendar.CountryCode, false);
                if (country == null)
                {
                    country = new Country()
                    {
                        CountryId = Guid.NewGuid(),
                        CountryCode = calendar.CountryCode,
                        CountryName = calendar.CountryName,
                        DateCreated = DateTime.Now
                    };
                    DB.GetTable<Country>().InsertOnSubmit(country);
                }
                else
                {
                    country.CountryName = calendar.CountryName;
                }
                DB.SubmitChanges();
                foreach (ICalPublicHoliday p in calendar.PublicHolidays)
                {
                    PublicHoliday publicHoliday = GetPublicHolidayByCountry(calendar.CountryCode, p.DateIdentifier, false);
                    if (publicHoliday == null)
                    {
                        publicHoliday = new PublicHoliday()
                        {
                            PublicHolidayId = Guid.NewGuid(),
                            CountryId = country.CountryId,
                            EventName = p.EventName,
                            DateIdentifier = p.DateIdentifier,
                            Year = p.Year,
                            Month = p.Month,
                            Day = p.Day,
                            HolidayDate = p.GetDate(),
                            DateCreated = DateTime.Now
                        };
                        DB.GetTable<PublicHoliday>().InsertOnSubmit(publicHoliday);
                    }
                    else
                    {
                        publicHoliday.CountryId = country.CountryId;
                        publicHoliday.EventName = p.EventName;
                        publicHoliday.DateIdentifier = p.DateIdentifier;
                        publicHoliday.Year = p.Year;
                        publicHoliday.Month = p.Month;
                        publicHoliday.Day = p.Day;
                        publicHoliday.HolidayDate = p.GetDate();
                    }
                    DB.SubmitChanges();
                }
                t.Complete();
            }
        }

        #endregion //Public Holiday
    }
}