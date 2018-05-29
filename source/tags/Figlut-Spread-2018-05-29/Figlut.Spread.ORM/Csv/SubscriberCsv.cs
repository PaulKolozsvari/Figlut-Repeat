namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SubscriberCsv
    {
        #region Properties

        public Guid SubscriberId { get; set; }

        public string CellPhoneNumber { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

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
