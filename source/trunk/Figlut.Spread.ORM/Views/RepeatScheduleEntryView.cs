namespace Figlut.Spread.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class RepeatScheduleEntryView
    {
        #region Properties

        #region Repeat Schedule Entry Properties

        public Guid RepeatScheduleEntryId { get; set; }

        public Guid RepeatScheduleId { get; set; }

        public DateTime RepeatDate { get; set; }

        public string RepeatDateFormatted { get; set; }

        public DateTime NotificationDate { get; set; }

        public string NotificationDateFormatted { get; set; }

        public bool SMSNotificationSent { get; set; }

        public string SMSMessageId { get; set; }

        public Nullable<DateTime> SMSDateSent { get; set; }

        public Nullable<Guid> SmsSentLogId { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Repeat Schedule Entry Properties

        #region Repeat Schedule Properties

        public Guid SubscriptionId { get; set; }

        public string NotificationMessage { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string RepeatScheduleNotes { get; set; }

        public DateTime RepeatScheduleDateCreated { get; set; }

        #endregion //Repeat Schedule Properties

        #endregion //Properties
    }
}