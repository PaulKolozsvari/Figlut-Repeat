namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class CreateSmsCampaignModel : SmsCampaignModel
    {
        #region Properties

        /// It is the search text specified by the user. To be used when confirming for multiple entities. i.e. used to find the entities.
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// To be used when confirming for multiple entities in a date range.
        /// </summary>
        public Nullable<DateTime> StartDate { get; set; }

        /// <summary>
        /// To be used when confirming for multiple entities in a date range.
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }

        /// <summary>
        /// The number of subscriptions to send the SMS to.
        /// </summary>
        public long CampaignSmsCount { get; set; }

        /// <summary>
        /// The maximum length of an SMS be be sent by the system.
        /// </summary>
        public int MaxSmsSendMessageLength { get; set; }

        #endregion //Properties
    }
}