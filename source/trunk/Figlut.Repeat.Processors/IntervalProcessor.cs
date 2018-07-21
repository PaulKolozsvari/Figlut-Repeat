namespace Figlut.Repeat.Processors
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.ORM;
    using System;
    using Figlut.Server.Toolkit.Utilities.Jobs;

    #endregion //Using Directives

    public abstract class IntervalProcessor : IntervalJob
    {
        #region Constructors

        public IntervalProcessor(
            Guid processorId,
            int executionInterval,
            bool startImmediately,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator,
            EmailSender emailSender)
            : base(executionInterval, startImmediately)
        {
            if (processorId == Guid.Empty)
            {
                throw new ArgumentException(string.Format("{0} may not be empty when constructing a {1}.",
                    EntityReader<IntervalProcessor>.GetPropertyName(p => p.ProcessorId, false),
                    this.GetType().Name));
            }
            if (string.IsNullOrEmpty(organizationIdentifierIndicator))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<IntervalProcessor>.GetPropertyName(p => p.OrganizationIdentifierIndicator, false),
                    this.GetType().Name));
            }
            if (string.IsNullOrEmpty(subscriberNameIndicator))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<IntervalProcessor>.GetPropertyName(p => p.SubscriberIdentifierIndicator, false),
                    this.GetType().Name));
            }
            if (emailSender == null)
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<IntervalProcessor>.GetPropertyName(p => p.EmailSender, false),
                    this.GetType().Name));
            }
            _processorId = processorId;
            _organizationIdentifierIndicator = organizationIdentifierIndicator;
            _subscriberIdentifierIndicator = subscriberNameIndicator;
            _emailSender = emailSender;
        }

        #endregion //Constructors

        #region Fields

        protected Guid _processorId;
        protected string _organizationIdentifierIndicator;
        protected string _subscriberIdentifierIndicator;
        protected EmailSender _emailSender;

        #endregion //Fields

        #region Properties

        public Guid ProcessorId
        {
            get { return _processorId; }
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

        protected override void ExecuteJob(IntervalJob intervalJob)
        {
            IntervalProcessor repeatProcessor = intervalJob as IntervalProcessor;
            if (repeatProcessor == null)
            {
                throw new Exception(string.Format("Unexpected base {0}. Could not cast {1} to a {2}.", 
                    typeof(IntervalJob).Name,
                    intervalJob.GetType().Name, 
                    typeof(IntervalProcessor).Name));
            }
            Guid processorId = repeatProcessor.ProcessorId;
            RepeatEntityContext context = RepeatEntityContext.Create();
            Processor processor = context.GetProcessor(processorId, true);
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Starting execution of {0} ...", DataShaper.ShapeCamelCaseString(processor.Name)), LogMessageType.Information, LoggingLevel.Maximum));
            processor.LastExecutionDate = DateTime.Now;
            context.DB.SubmitChanges();
            string organizationIdentifierIndicator = repeatProcessor.OrganizationIdentifierIndicator;
            string subscriberIdentifierIndicator = repeatProcessor.SubscriberIdentifierIndicator;
            while (true)
            {
                try
                {
                    if (!ProcessNextItemInQueue(context, processorId, organizationIdentifierIndicator, subscriberIdentifierIndicator))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                    context.LogProcesorAction(processorId, ex.Message, LogMessageType.Error.ToString());
                }
            }
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Executed {0} successfully.", DataShaper.ShapeCamelCaseString(processor.Name)), LogMessageType.SuccessAudit, LoggingLevel.Maximum));
        }

        protected abstract bool ProcessNextItemInQueue(RepeatEntityContext context, Guid processorId, string organizationIdentifierIndicator, string subscriberIdentifierIndicator);

        #endregion //Methods
    }
}