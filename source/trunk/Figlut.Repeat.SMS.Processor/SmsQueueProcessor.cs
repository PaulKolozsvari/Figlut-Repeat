namespace Figlut.Repeat.SMS.Processor
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public abstract class SmsQueueProcessor
    {
        #region Constructors

        public SmsQueueProcessor(
            Guid smsProcessorId,
            int executionInterval,
            bool startImmediately,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator,
            EmailSender emailSender)
        {
            if (smsProcessorId == Guid.Empty)
            {
                throw new ArgumentException(string.Format("{0} may not be empty when constructing a {1}.",
                    EntityReader<SmsQueueProcessor>.GetPropertyName(p => p.SmsProcessorId, false),
                    this.GetType().Name));
            }
            if (executionInterval < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} may not be less than 0 when constructing a {1}.",
                    EntityReader<SmsQueueProcessor>.GetPropertyName(p => p.ExecutionInterval, false),
                    this.GetType().Name));
            }
            if (string.IsNullOrEmpty(organizationIdentifierIndicator))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<SmsQueueProcessor>.GetPropertyName(p => p.OrganizationIdentifierIndicator, false),
                    this.GetType().Name));
            }
            if (string.IsNullOrEmpty(subscriberNameIndicator))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<SmsQueueProcessor>.GetPropertyName(p => p.SubscriberIdentifierIndicator, false),
                    this.GetType().Name));
            }
            if (emailSender == null)
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<SmsQueueProcessor>.GetPropertyName(p => p.EmailSender, false),
                    this.GetType().Name));
            }
            _smsProcessorId = smsProcessorId;
            _organizationIdentifierIndicator = organizationIdentifierIndicator;
            _subscriberIdentifierIndicator = subscriberNameIndicator;
            _emailSender = emailSender;

            _executionInterval = executionInterval;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
            ChangeExecutionInterval(_executionInterval);
            if (startImmediately)
            {
                StartProcessor();
            }
        }

        #endregion //Constructors

        #region Fields

        protected Guid _smsProcessorId;

        protected int _executionInterval;
        protected bool _currentlyProcessing;
        protected System.Timers.Timer _timer;

        protected string _organizationIdentifierIndicator;
        protected string _subscriberIdentifierIndicator;

        protected EmailSender _emailSender;

        protected readonly object _lockObject = new object();

        #endregion //Fields

        #region Properties

        public Guid SmsProcessorId
        {
            get { return _smsProcessorId; }
        }

        public int ExecutionInterval
        {
            get { return _executionInterval; }
        }

        public bool CurrentlyProcessing
        {
            get { return _currentlyProcessing; }
        }

        public string OrganizationIdentifierIndicator
        {
            get { return _organizationIdentifierIndicator; }
        }

        public string SubscriberIdentifierIndicator
        {
            get { return _subscriberIdentifierIndicator; }

        }

        public EmailSender EmailSender
        {
            get { return _emailSender; }
        }

        #endregion //Properties

        #region Methods

        public void SetCurrentlyProcessingFlag(bool currentlyProcessing)
        {
            _currentlyProcessing = currentlyProcessing;
        }

        public void StartProcessor()
        {
            _timer.Start();
        }

        internal void StopProcessor()
        {
            _timer.Stop();
        }

        internal bool IsEnabled()
        {
            return _timer.Enabled;
        }

        public bool ChangeExecutionInterval(int executionInterval)
        {
            if (_currentlyProcessing)
            {
                return false;
            }
            _executionInterval = executionInterval;
            _timer.Interval = Convert.ToDouble(_executionInterval);
            return true;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StopProcessor();
            try
            {
                ProcessQueue(this);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            finally
            {
                if (!this.IsEnabled())
                {
                    this.StartProcessor();
                }
            }
        }

        protected void ProcessQueue(SmsQueueProcessor processor)
        {
            if (processor.CurrentlyProcessing)
            {
                return;
            }
            lock (_lockObject)
            {
                try
                {
                    processor.SetCurrentlyProcessingFlag(true);
                    Guid smsProcessorId = processor.SmsProcessorId;
                    RepeatEntityContext context = RepeatEntityContext.Create();
                    SmsProcessor smsProcessor = context.GetSmsProcessor(smsProcessorId, true);
                    GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Starting execution of {0} ...", DataShaper.ShapeCamelCaseString(smsProcessor.Name)), LogMessageType.Information, LoggingLevel.Maximum));
                    smsProcessor.LastExecutionDate = DateTime.Now;
                    context.DB.SubmitChanges();
                    string organizationIdentifierIndicator = processor.OrganizationIdentifierIndicator;
                    string subscriberIdentifierIndicator = processor.SubscriberIdentifierIndicator;
                    while (true)
                    {
                        try
                        {
                            if (!ProcessNextItemInQueue(context, smsProcessorId, organizationIdentifierIndicator, subscriberIdentifierIndicator))
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.HandleException(ex);
                            context.LogSmsProcesorAction(smsProcessorId, ex.Message, LogMessageType.Error.ToString());
                        }
                    }
                    GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Executed {0} successfully.", DataShaper.ShapeCamelCaseString(smsProcessor.Name)), LogMessageType.SuccessAudit, LoggingLevel.Maximum));
                }
                finally
                {
                    processor.SetCurrentlyProcessingFlag(false);
                }
            }
        }

        protected abstract bool ProcessNextItemInQueue(RepeatEntityContext context, Guid smsProcessorId, string organizationIdentifierIndicator, string subscriberIdentifierIndicator);

        #endregion //Methods
    }
}