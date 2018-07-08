namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Web.Client.IP_API;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Web Request Activity

        /// <summary>
        /// This method database operation does not use a Transaction because it slows it down and is not necessary.
        /// </summary>
        public WebRequestActivity LogWebRequestActivity(
            bool logWebRequestActivity,
            bool logUserLastActivityDate,
            string requestVerb,
            string requestUrl,
            string requestReferrerUrl,
            string userAgent,
            string userHostAddress,
            string userHostName,
            string actionName,
            string controllerName,
            string clientType,
            bool isCrawler,
            bool isMobileDevice,
            string mobileDeviceManufacturer,
            string mobileDeviceModel,
            string platform,
            string currentUserName,
            string source,
            string comments,
            query whoIsQuery,
            DateTime requestDate)
        {
            WebRequestActivity result = null;
            try
            {
                User currentUser = null;
                if (!string.IsNullOrEmpty(currentUserName))
                {
                    currentUser = GetUserByIdentifier(currentUserName, false);
                }
                if (logWebRequestActivity)
                {
                    result = new WebRequestActivity()
                    {
                        WebRequestActivityId = Guid.NewGuid(),
                        RequestVerb = requestVerb,
                        RequestUrl = requestUrl,
                        RequestReferrerUrl = requestReferrerUrl,
                        Controller = controllerName,
                        Action = actionName,
                        UserAgent = userAgent,
                        UserHostAddress = userHostAddress,
                        UserHostName = userHostName,
                        ClientType = clientType,
                        IsCrawler = isCrawler,
                        IsMobileDevice = isMobileDevice,
                        MobileDeviceManufacturer = mobileDeviceManufacturer,
                        MobileDeviceModel = mobileDeviceModel,
                        Platform = platform,
                        SourceApplication = source,
                        Comments = comments,
                        DateCreated = DateTime.Now
                    };
                    if (whoIsQuery != null)
                    {
                        result.WhoIsStatus = whoIsQuery.status;
                        result.WhoIsCountry = whoIsQuery.country;
                        result.WhoIsCountryCode = whoIsQuery.countryCode;
                        result.WhoIsRegion = whoIsQuery.region;
                        result.WhoIsRegionName = whoIsQuery.regionName;
                        result.WhoIsCity = whoIsQuery.city;
                        result.WhoIsZip = whoIsQuery.zip;
                        result.WhoIsLatitude = whoIsQuery.lat;
                        result.WhoIsLongitude = whoIsQuery.lon;
                        result.WhoIsTimeZone = whoIsQuery.timezone;
                        result.WhoIsISP = whoIsQuery.isp;
                        result.WhoIsOrg = whoIsQuery.org;
                    }
                }
                if (currentUser != null)
                {
                    result.UserId = currentUser.UserId;
                    result.UserName = currentUser.UserName;
                }
                if (logWebRequestActivity)
                {
                    DB.GetTable<WebRequestActivity>().InsertOnSubmit(result);
                    DB.SubmitChanges(); //This is a simply insert, hence it should not cause any concurrency conflicts.
                }
                if (currentUser != null && logUserLastActivityDate)
                {
                    currentUser.LastActivityDate = requestDate;
                    DB.SubmitChanges(ConflictMode.ContinueOnConflict); //This submit causes concurrency conflicts, hence it should be a separate submit from the WebRequestActivity insert.
                }
            }
            catch (ChangeConflictException cEx)
            {
                DB.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues);
            }
            return result;
        }

        public WebRequestActivity GetWebRequestActivity(Guid webRequestActivityId, bool throwExceptionOnNotFound)
        {
            List<WebRequestActivity> q = (from w in DB.GetTable<WebRequestActivity>()
                                          where w.WebRequestActivityId == webRequestActivityId
                                          select w).ToList();
            WebRequestActivity result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(ProcessorLog).Name,
                    EntityReader<WebRequestActivity>.GetPropertyName(p => p.WebRequestActivityId, false),
                    webRequestActivityId.ToString()));
            }
            return result;
        }

        public long GetAllWebRequestActivitiesCount()
        {
            return DB.GetTable<WebRequestActivity>().LongCount();
        }

        public List<WebRequestActivity> GetWebRequestActivityByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> userId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<WebRequestActivity> result = null;
            if (userId.HasValue) //For specified user only.
            {
                result = (from w in DB.GetTable<WebRequestActivity>()
                          where (w.UserId.HasValue && w.UserId == userId.Value) &&
                          (w.DateCreated.Date >= startDate.Date && w.DateCreated.Date <= endDate.Date) &&
                          ((w.RequestVerb != null && w.RequestVerb.ToLower().Contains(searchFilterLower)) ||
                          (w.RequestUrl != null && w.RequestUrl.ToLower().Contains(searchFilterLower)) ||
                          (w.RequestReferrerUrl != null && w.RequestReferrerUrl.ToString().ToLower().Contains(searchFilterLower)) ||
                          (w.Controller != null && w.Controller.ToLower().Contains(searchFilterLower)) ||
                          (w.Action != null && w.Action.ToLower().Contains(searchFilterLower)) ||
                          (w.IsCrawler.HasValue && w.IsCrawler.Value.ToString().Trim().ToLower().Contains(searchFilterLower)))
                          orderby w.DateCreated descending
                          select w).ToList();
            }
            else //For all users.
            {
                result = (from w in DB.GetTable<WebRequestActivity>()
                          where (w.DateCreated.Date >= startDate.Date && w.DateCreated.Date <= endDate.Date) &&
                          ((w.RequestVerb != null && w.RequestVerb.ToLower().Contains(searchFilterLower)) ||
                          (w.RequestUrl != null && w.RequestUrl.ToLower().Contains(searchFilterLower)) ||
                          (w.RequestReferrerUrl != null && w.RequestReferrerUrl.ToString().ToLower().Contains(searchFilterLower)) ||
                          (w.Controller != null && w.Controller.ToLower().Contains(searchFilterLower)) ||
                          (w.Action != null && w.Action.ToLower().Contains(searchFilterLower)) ||
                          (w.IsCrawler.HasValue && w.IsCrawler.Value.ToString().Trim().ToLower().Contains(searchFilterLower)))
                          orderby w.DateCreated descending
                          select w).ToList();
            }
            return result;
        }

        public void DeleteWebRequestActivityByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> userId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<WebRequestActivity> q = GetWebRequestActivityByFilter(searchFilter, startDate, endDate, userId);
                DB.GetTable<WebRequestActivity>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Web Request Activity
    }
}