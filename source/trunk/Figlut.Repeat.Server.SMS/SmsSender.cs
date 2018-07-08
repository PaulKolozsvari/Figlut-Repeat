namespace Figlut.Repeat.SMS
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public abstract class SmsSender
    {
        #region Constructors

        public SmsSender(SmsProvider smsProvider)
        {
            _smsProvider = smsProvider;
        }

        #endregion //Constructors

        #region Fields

        protected bool _smsNotificationsEnabled;
        protected SmsProvider _smsProvider;

        #endregion //Fields

        #region Properties

        public bool SmsNotificationsEnabled
        {
            get { return _smsNotificationsEnabled; }
            set { _smsNotificationsEnabled = value; }
        }

        public SmsProvider SmsProvider
        {
            get { return _smsProvider; }
        }

        #endregion //Properties

        #region Methods

        public abstract SmsResponse SendSms(SmsRequest request);

        #endregion //Methods
    }
}
