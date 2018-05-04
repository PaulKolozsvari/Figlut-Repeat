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

    #endregion //Using Directives

    public class RepeatScheduleModel
    {
        #region Properties

        #region Repeat Schedule Properties

        public Guid RepeatScheduleId { get; set; }

        public Guid SubscriptionId { get; set; }

        public string NotificationMessage { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string Notes { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Repeat Schedule Properties

        #region Subscription Properties

        public Guid OrganizationId { get; set; }

        public Guid SubscriberId { get; set; }

        public bool SubscriptionEnabled { get; set; }

        public string CustomerFullName { get; set; }

        public string CustomerIdentifier { get; set; }

        public string CustomerPhysicalAddress { get; set; }

        public string CustomerNotes { get; set; }

        public DateTime SubscriptionDateCreated { get; set; }

        #endregion //Subscription Properties

        #region Subscriber Properties

        public string CellPhoneNumber { get; set; }

        public string SubscriberName { get; set; }

        public bool SubscriberEnabled { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.RepeatScheduleId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.RepeatScheduleId, true));
            }
            if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            if (string.IsNullOrEmpty(this.NotificationMessage))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.NotificationMessage, true));
            }
            if (string.IsNullOrEmpty(this.ScheduleName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.ScheduleName, true));
            }
            if (this.DaysRepeatInterval == 0)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.DaysRepeatInterval, true));
            }
            if (string.IsNullOrEmpty(this.Notes))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.Notes, true));
            }
            //Subscription
            if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            if (this.SubscriberId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            if (string.IsNullOrEmpty(this.CustomerFullName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.CustomerFullName, true));
            }
            if (string.IsNullOrEmpty(this.CustomerIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.CustomerIdentifier, true));
            }
            if (string.IsNullOrEmpty(this.CustomerPhysicalAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.CustomerPhysicalAddress, true));
            }
            if (string.IsNullOrEmpty(this.CustomerNotes))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.CustomerNotes, true));
            }
            //Subscriber
            if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            if (string.IsNullOrEmpty(this.SubscriberName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.SubscriberName, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromRepeatScheduleView(RepeatScheduleView view)
        {
            this.RepeatScheduleId = view.RepeatScheduleId;
            this.SubscriptionId = view.SubscriptionId;
            this.NotificationMessage = view.NotificationMessage;
            this.ScheduleName = view.ScheduleName;
            this.Quantity = view.Quantity;
            this.UnitOfMeasure = view.UnitOfMeasure;
            this.DaysRepeatInterval = view.DaysRepeatInterval;
            this.Notes = view.Notes;
            this.DateCreated = view.DateCreated;

            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.SubscriptionEnabled = view.SubscriptionEnabled;
            this.CustomerFullName = view.CustomerFullName;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerNotes;
            this.SubscriptionDateCreated = view.SubscriptionDateCreated;

            this.CellPhoneNumber = view.CellPhoneNumber;
            this.SubscriberName = view.SubscriberName;
            this.SubscriberEnabled = view.SubscriberEnabled;
        }

        public void CopyPropertiesToRepeatScheduleView(RepeatScheduleView view)
        {
            view.RepeatScheduleId = this.RepeatScheduleId;
            view.SubscriptionId = this.SubscriptionId;
            view.NotificationMessage = this.NotificationMessage;
            view.ScheduleName = this.ScheduleName;
            view.Quantity = this.Quantity;
            view.UnitOfMeasure = this.UnitOfMeasure;
            view.DaysRepeatInterval = this.DaysRepeatInterval;
            view.Notes = this.Notes;
            view.DateCreated = this.DateCreated;

            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.SubscriptionEnabled = this.SubscriptionEnabled;
            view.CustomerFullName = this.CustomerFullName;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerNotes;
            view.SubscriptionDateCreated = this.SubscriptionDateCreated;

            view.CellPhoneNumber = this.CellPhoneNumber;
            view.SubscriberName = this.SubscriberName;
            view.SubscriberEnabled = this.SubscriberEnabled;
        }

        public void CopyPropertiesToRepeatSchedule(RepeatSchedule repeatSchedule)
        {
            repeatSchedule.RepeatScheduleId = this.RepeatScheduleId;
            repeatSchedule.SubscriptionId = this.SubscriptionId;
            repeatSchedule.NotificationMessage = this.NotificationMessage;
            repeatSchedule.ScheduleName = this.ScheduleName;
            repeatSchedule.Quantity = this.Quantity;
            repeatSchedule.UnitOfMeasure = this.UnitOfMeasure;
            repeatSchedule.DaysRepeatInterval = this.DaysRepeatInterval;
            repeatSchedule.Notes = this.Notes;
            repeatSchedule.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}