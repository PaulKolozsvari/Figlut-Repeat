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

        public bool Enabled { get; set; }

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
            int parsedCellPhoneInt = 0;
            if (!int.TryParse(this.CellPhoneNumber, out parsedCellPhoneInt))
            {
                errorMessage = string.Format("{0} must 10 numeric digits.", EntityReader<SubscriberModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            if (this.CellPhoneNumber.Length != 10)
            {
                errorMessage = string.Format("{0} must be 10 digits.", EntityReader<SubscriberModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromSubscriber(Subscriber subscriber)
        {
            this.SubscriberId = subscriber.SubscriberId;
            this.CellPhoneNumber = subscriber.CellPhoneNumber;
            this.Name = subscriber.Name;
            this.Enabled = subscriber.Enabled;
            this.DateCreated = subscriber.DateCreated;
        }

        public void CopyPropertiesToSubsriber(Subscriber subscriber)
        {
            subscriber.SubscriberId = this.SubscriberId;
            subscriber.CellPhoneNumber = this.CellPhoneNumber;
            subscriber.Name = this.Name;
            subscriber.Enabled = this.Enabled;
            subscriber.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}