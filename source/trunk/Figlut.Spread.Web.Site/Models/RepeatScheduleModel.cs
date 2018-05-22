﻿namespace Figlut.Spread.Web.Site.Models
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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartDate { get; set; }

        public string StartDateFormatted { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> EndDate { get; set; }

        public string EndDateFormatted { get; set; }

        public int EntryCount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExtendDate { get; set; }

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

        #region Settings

        public int MaxSmsSendMessageLength { get; set; }

        #endregion //Settings

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            //Repeat Schedule
            if (this.RepeatScheduleId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.RepeatScheduleId, true));
            }
            else if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            else if (string.IsNullOrEmpty(this.NotificationMessage))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.NotificationMessage, true));
            }
            else if (this.NotificationMessage.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be greater than {1} characters.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.NotificationMessage, true), maxSmsSendMessageLength);
            }
            else if (string.IsNullOrEmpty(this.ScheduleName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.ScheduleName, true));
            }
            else if (this.DaysRepeatInterval <= 0)
            {
                errorMessage = string.Format("{0} may not be less than or equal to 0.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.DaysRepeatInterval, true));
            }
            else if (this.Quantity.HasValue && this.Quantity.Value < 0.0)
            {
                errorMessage = string.Format("{0} may not be less than or equal to 0.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.Quantity, true));
            }
            //Subscription
            else if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            else if (this.SubscriberId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            //Subscriber
            else if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.CellPhoneNumber, true));
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
            this.StartDate = view.StartDate;
            if (this.StartDate.HasValue)
            {
                this.StartDateFormatted = DataShaper.GetDefaultDateString(view.StartDate.Value);
            }
            this.EndDate = view.EndDate;
            if (this.EndDate.HasValue)
            {
                this.EndDateFormatted = DataShaper.GetDefaultDateString(view.EndDate.Value);
            }
            this.EntryCount = view.EntryCount;

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
            view.StartDate = this.StartDate;
            if (view.StartDate.HasValue)
            {
                view.StartDateFormatted = DataShaper.GetDefaultDateString(view.StartDate.Value);
            }
            view.EndDate = this.EndDate;
            if (view.EndDate.HasValue)
            {
                view.EndDateFormatted = DataShaper.GetDefaultDateString(view.EndDate.Value);
            }
            view.EntryCount = this.EntryCount;

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