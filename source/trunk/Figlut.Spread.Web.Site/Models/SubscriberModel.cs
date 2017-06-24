namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class SubscriberModel
    {
        #region Properties

        public Guid SubscriberId { get; set; }

        public string CellPhoneNumber { get; set; }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromSubscriber(Subscriber subscriber)
        {
            this.SubscriberId = subscriber.SubscriberId;
            this.CellPhoneNumber = subscriber.CellPhoneNumber;
            this.Name = subscriber.Name;
            this.IsEnabled = subscriber.IsEnabled;
            this.DateCreated = subscriber.DateCreated;
        }

        public void CopyPropertiesToSubsriber(Subscriber subscriber)
        {
            subscriber.SubscriberId = this.SubscriberId;
            subscriber.CellPhoneNumber = this.CellPhoneNumber;
            subscriber.Name = this.Name;
            subscriber.IsEnabled = this.IsEnabled;
            subscriber.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}