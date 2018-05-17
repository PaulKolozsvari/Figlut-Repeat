namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    using Figlut.Server.Toolkit.Data;
    using System.ComponentModel.DataAnnotations;

    #endregion //Using Directives

    public class RepeatScheduleEntryModel
    {
        #region Properties

        #region Repeat Schedule Entry Properties

        public Guid RepeatScheduleEntryId { get; set; }

        public Guid RepeatScheduleId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RepeatDate { get; set; }

        public string RepeatDateFormatted { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.RepeatScheduleEntryId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleEntryModel>.GetPropertyName(p => p.RepeatScheduleEntryId, true));
            }
            if (this.RepeatScheduleId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleEntryModel>.GetPropertyName(p => p.RepeatScheduleId, true));
            }
            //Repeat Schedule
            if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleEntryModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            if (string.IsNullOrEmpty(this.NotificationMessage))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleEntryModel>.GetPropertyName(p => p.NotificationMessage, true));
            }
            if (string.IsNullOrEmpty(this.ScheduleName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleEntryModel>.GetPropertyName(p => p.ScheduleName, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromRepeatScheduleEntryView(RepeatScheduleEntryView view)
        {
            this.RepeatScheduleEntryId = view.RepeatScheduleEntryId;
            this.RepeatScheduleId = view.RepeatScheduleId;
            this.RepeatDate = view.RepeatDate;
            this.RepeatDateFormatted = view.RepeatDateFormatted;
            this.NotificationDate = view.NotificationDate;
            this.NotificationDateFormatted = view.NotificationDateFormatted;
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
        }

        public void CopyPropertiesToRepeatScheduleEntry(RepeatScheduleEntry repeatScheduleEntry)
        {
            repeatScheduleEntry.RepeatScheduleEntryId = this.RepeatScheduleEntryId;
            repeatScheduleEntry.RepeatScheduleId = this.RepeatScheduleId;
            repeatScheduleEntry.RepeatDate = this.RepeatDate;
            repeatScheduleEntry.RepeatDateFormatted = DataShaper.GetDefaultDateString(this.RepeatDate);
            repeatScheduleEntry.NotificationDate = this.NotificationDate;
            repeatScheduleEntry.NotificationDateFormatted = DataShaper.GetDefaultDateString(this.NotificationDate);
            repeatScheduleEntry.SMSNotificationSent = this.SMSNotificationSent;
            repeatScheduleEntry.SMSMessageId = this.SMSMessageId;
            repeatScheduleEntry.SMSDateSent = this.SMSDateSent;
            repeatScheduleEntry.SmsSentLogId = this.SmsSentLogId;
            repeatScheduleEntry.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}