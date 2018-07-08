namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Server.Toolkit.Data;
    using System.ComponentModel.DataAnnotations;

    #endregion //Using Directives

    public class ScheduleEntryModel
    {
        #region Properties

        #region Schedule Entry Properties

        public Guid ScheduleEntryId { get; set; }

        public Guid ScheduleId { get; set; }

        public string NotificationMessage { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EntryTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EntryDateCreate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EntryDateShift { get; set; }

        public string EntryDateFormatted { get; set; }

        public string EntryDateDayOfWeek { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime NotificationDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime NotificationDateCreate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime NotificationDateShift { get; set; }

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

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.ScheduleEntryId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ScheduleEntryModel>.GetPropertyName(p => p.ScheduleEntryId, true));
            }
            if (this.ScheduleId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ScheduleEntryModel>.GetPropertyName(p => p.ScheduleId, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromScheduleEntryView(ScheduleEntryView view)
        {
            this.ScheduleEntryId = view.ScheduleEntryId;
            this.ScheduleId = view.ScheduleId;
            this.NotificationMessage = view.NotificationMessage;
            this.EntryTime = view.EntryTime;
            this.EntryDate = view.EntryDate;
            this.EntryDateFormatted = view.EntryDateFormatted;
            this.EntryDateDayOfWeek = view.EntryDateDayOfWeek;
            this.NotificationDate = view.NotificationDate;
            this.NotificationDateFormatted = view.NotificationDateFormatted;
            this.NotificationDateDayOfWeek = view.NotificationDateDayOfWeek;
            this.SMSNotificationSent = view.SMSNotificationSent;
            this.SMSMessageId = view.SMSMessageId;
            this.SMSDateSent = view.SMSDateSent;
            this.SmsSentLogId = view.SmsSentLogId;
            this.DateCreated = view.DateCreated;

            this.ScheduleId = view.ScheduleId;
            this.SubscriptionId = view.SubscriptionId;
            this.ScheduleNotificationMessage = view.ScheduleNotificationMessage;
            this.ScheduleName = view.ScheduleName;
            this.Quantity = view.Quantity;
            this.UnitOfMeasure = view.UnitOfMeasure;
            this.DaysRepeatInterval = view.DaysRepeatInterval;
            this.ScheduleNotes = view.ScheduleNotes;
            this.DateCreated = view.DateCreated;

            this.CellPhoneNumber = view.CellPhoneNumber;

            this.CustomerFullName = view.CustomerFullName;
            this.CustomerEmailAddress = view.CustomerEmailAddress;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerNotes;
        }

        public void CopyPropertiesToScheduleEntryView(ScheduleEntryView view)
        {
            view.ScheduleEntryId = this.ScheduleEntryId;
            view.ScheduleId = this.ScheduleId;
            view.NotificationMessage = this.NotificationMessage;
            view.EntryTime = this.EntryTime;
            view.EntryDate = this.EntryDate;
            view.EntryDateFormatted = this.EntryDateFormatted;
            view.EntryDateDayOfWeek = this.EntryDateDayOfWeek;
            view.NotificationDate = this.NotificationDate;
            view.NotificationDateFormatted = this.NotificationDateFormatted;
            view.NotificationDateDayOfWeek = this.NotificationDateDayOfWeek;
            view.SMSNotificationSent = this.SMSNotificationSent;
            view.SMSMessageId = this.SMSMessageId;
            view.SMSDateSent = this.SMSDateSent;
            view.SmsSentLogId = this.SmsSentLogId;
            view.DateCreated = this.DateCreated;

            view.ScheduleId = this.ScheduleId;
            view.SubscriptionId = this.SubscriptionId;
            view.ScheduleNotificationMessage = this.ScheduleNotificationMessage;
            view.ScheduleName = this.ScheduleName;
            view.Quantity = this.Quantity;
            view.UnitOfMeasure = this.UnitOfMeasure;
            view.DaysRepeatInterval = this.DaysRepeatInterval;
            view.ScheduleNotes = this.ScheduleNotes;
            view.DateCreated = this.DateCreated;

            view.CellPhoneNumber = this.CellPhoneNumber;

            view.CustomerFullName = this.CustomerFullName;
            view.CustomerEmailAddress = this.CustomerEmailAddress;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerNotes;
        }

        public void CopyPropertiesToScheduleEntry(ScheduleEntry scheduleEntry)
        {
            scheduleEntry.ScheduleEntryId = this.ScheduleEntryId;
            scheduleEntry.ScheduleId = this.ScheduleId;
            scheduleEntry.NotificationMessage = this.NotificationMessage;
            scheduleEntry.EntryTime = this.EntryTime;
            scheduleEntry.EntryDate = this.EntryDate;
            scheduleEntry.EntryDateFormatted = DataShaper.GetDefaultDateString(this.EntryDate);
            scheduleEntry.EntryDateDayOfWeek = this.EntryDate.DayOfWeek.ToString();
            scheduleEntry.NotificationDate = this.NotificationDate;
            scheduleEntry.NotificationDateFormatted = DataShaper.GetDefaultDateString(this.NotificationDate);
            scheduleEntry.NotificationDateDayOfWeek = this.NotificationDate.DayOfWeek.ToString();
            scheduleEntry.SMSNotificationSent = this.SMSNotificationSent;
            scheduleEntry.SMSMessageId = this.SMSMessageId;
            scheduleEntry.SMSDateSent = this.SMSDateSent;
            scheduleEntry.SmsSentLogId = this.SmsSentLogId;
            scheduleEntry.DateCreated = this.DateCreated;
            if (!scheduleEntry.SMSNotificationSent)
            {
                scheduleEntry.SMSMessageId = null;
                scheduleEntry.SMSDateSent = null;
                scheduleEntry.SmsSentLogId = null;
            }
        }

        #endregion //Methods
    }
}