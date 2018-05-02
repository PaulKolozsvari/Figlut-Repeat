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
                                        from sub in set
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
                          select p).ToList();
            }
            else
            {
                result = (from p in DB.GetTable<PublicHoliday>()
                          where p.DateIdentifier.ToLower().Contains(searchFilterLower) ||
                          p.EventName.ToLower().Contains(searchFilterLower)
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

        #endregion //Public Holiday
    }
}