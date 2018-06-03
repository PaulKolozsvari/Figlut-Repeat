namespace Figlut.Spread.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ScheduleEntryView
    {
        #region Properties

        #region Schedule Entry Properties

        public Guid ScheduleEntryId { get; set; }

        public Guid ScheduleId { get; set; }

        public string NotificationMessage { get; set; }

        public TimeSpan EntryTime { get; set; }

        public DateTime EntryDate { get; set; }

        public string EntryDateFormatted { get; set; }

        public string EntryDateDayOfWeek { get; set; }

        public DateTime NotificationDate { get; set; }

        public string NotificationDateFormatted { get; set; }

        public string NotificationDateDayOfWeek { get; set; }

        public bool SMSNotificationSent { get; set; }

        public string SMSMessageId { get; set; }

        public Nullable<DateTime> SMSDateSent { get; set; }

        public Nullable<Guid> SmsSentLogId { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Schedule Entry Properties

        #region Schedule Properties

        public Guid SubscriptionId { get; set; }

        public string ScheduleNotificationMessage { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string ScheduleNotes { get; set; }

        public DateTime ScheduleDateCreated { get; set; }

        #endregion //Schedule Properties

        #region Subscriber Properties

        public string CellPhoneNumber { get; set; }

        #endregion //Subscriber Properties

        #region Subscription Properties

        public string CustomerFullName { get; set; }

        public string CustomerEmailAddress { get; set; }

        public string CustomerIdentifier { get; set; }

        public string CustomerPhysicalAddress { get; set; }

        public string CustomerNotes { get; set; }

        #endregion //Subscription Properties

        #endregion //Properties
    }
}