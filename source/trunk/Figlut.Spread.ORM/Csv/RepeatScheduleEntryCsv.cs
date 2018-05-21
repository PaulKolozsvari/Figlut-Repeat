namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Spread.ORM.Views;

    #endregion //Using Directives

    public class RepeatScheduleEntryCsv
    {
        #region Properties

        #region Repeat Schedule Entry Properties

        public Guid RepeatScheduleEntryId { get; set; }

        public Guid RepeatScheduleId { get; set; }

        public DateTime RepeatDate { get; set; }

        public string RepeatDateFormatted { get; set; }

        public string RepeatDateDayOfWeek { get; set; }

        public DateTime NotificationDate { get; set; }

        public string NotificationDateFormatted { get; set; }

        public string NotificationDateDayOfWeek { get; set; }

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

        #region Subscriber Properties

        public string CellPhoneNumber { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromRepeatScheduleEntryView(RepeatScheduleEntryView view)
        {
            this.RepeatScheduleEntryId = view.RepeatScheduleEntryId;
            this.RepeatScheduleId = view.RepeatScheduleId;
            this.RepeatDate = view.RepeatDate;
            this.RepeatDateFormatted = view.RepeatDateFormatted;
            this.RepeatDateDayOfWeek = view.RepeatDateDayOfWeek;
            this.NotificationDate = view.NotificationDate;
            this.NotificationDateFormatted = view.NotificationDateFormatted;
            this.NotificationDateDayOfWeek = view.NotificationDateDayOfWeek;
            this.SMSNotificationSent = view.SMSNotificationSent;
            this.SMSMessageId = view.SMSMessageId;
            this.SMSDateSent = view.SMSDateSent;
            this.SmsSentLogId = view.SmsSentLogId;
            this.DateCreated = view.DateCreated;

            this.RepeatScheduleId = view.RepeatScheduleId;
            this.SubscriptionId = view.SubscriptionId;
            this.NotificationMessage = view.NotificationMessage;
            this.ScheduleName = view.ScheduleName;
            this.Quantity = view.Quantity;
            this.UnitOfMeasure = view.UnitOfMeasure;
            this.DaysRepeatInterval = view.DaysRepeatInterval;
            this.RepeatScheduleNotes = view.RepeatScheduleNotes;
            this.DateCreated = view.DateCreated;

            this.CellPhoneNumber = view.CellPhoneNumber;
        }

        public void CopyPropertiesToRepeatScheduleEntryView(RepeatScheduleEntryView view)
        {
            view.RepeatScheduleEntryId = this.RepeatScheduleId;
            view.RepeatScheduleId = this.RepeatScheduleId;
            view.RepeatDate = this.RepeatDate;
            view.RepeatDateFormatted = this.RepeatDateFormatted;
            view.NotificationDate = this.NotificationDate;
            view.NotificationDateFormatted = this.NotificationDateFormatted;
            view.SMSNotificationSent = this.SMSNotificationSent;
            view.SMSMessageId = this.SMSMessageId;
            view.SMSDateSent = this.SMSDateSent;
            view.SmsSentLogId = this.SmsSentLogId;
            view.DateCreated = this.DateCreated;

            view.RepeatScheduleId = this.RepeatScheduleId;
            view.SubscriptionId = this.SubscriptionId;
            view.NotificationMessage = this.NotificationMessage;
            view.ScheduleName = this.ScheduleName;
            view.Quantity = this.Quantity;
            view.UnitOfMeasure = this.UnitOfMeasure;
            view.DaysRepeatInterval = this.DaysRepeatInterval;
            view.RepeatScheduleNotes = this.RepeatScheduleNotes;
            view.DateCreated = this.DateCreated;

            view.CellPhoneNumber = this.CellPhoneNumber;
        }

        #endregion //Methods
    }
}