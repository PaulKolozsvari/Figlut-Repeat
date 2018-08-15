namespace Figlut.Repeat.Processors.Schedule
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Data;
    using System.IO;

    #endregion //Using Directives

    public class ScheduleEntriesDailyProcessor : IntervalProcessor
    {
        #region Constructors

        public ScheduleEntriesDailyProcessor(
            string dailyScheduleEntriesEmailDirectory,
            string dailyScheduleEntriesEmailFilesDirectory,
            string homePageUrl,
            Guid processorId,
            int executionInterval, 
            bool startImmediately, 
            string organizationIdentifierIndicator, 
            string subscriberNameIndicator, 
            EmailSender emailSender) : 
            base(processorId, executionInterval, startImmediately, organizationIdentifierIndicator, subscriberNameIndicator, emailSender)
        {
            if (string.IsNullOrEmpty(homePageUrl))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<ScheduleEntriesDailyProcessor>.GetPropertyName(p => p.HomePageUrl, false),
                    this.GetType().Name));
            }
            if (string.IsNullOrEmpty(dailyScheduleEntriesEmailDirectory))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<ScheduleEntriesDailyProcessor>.GetPropertyName(p => p.DailyScheduleEntriesEmailDirectory, false),
                    this.GetType().Name));
            }
            FileSystemHelper.ValidateDirectoryExists(dailyScheduleEntriesEmailDirectory);
            if (string.IsNullOrEmpty(dailyScheduleEntriesEmailFilesDirectory))
            {
                throw new ArgumentNullException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<ScheduleEntriesDailyProcessor>.GetPropertyName(p => p.DailyScheduleEntriesFilesDirectory, false),
                    this.GetType().Name));
            }
            FileSystemHelper.ValidateDirectoryExists(dailyScheduleEntriesEmailFilesDirectory);
            _homePageUrl = homePageUrl;
            _dailyScheduleEntriesEmailDirectory = dailyScheduleEntriesEmailDirectory;
            _dailyScheduleEntriesEmailFilesDirectory = dailyScheduleEntriesEmailFilesDirectory;
        }

        #endregion //Constructors

        #region Fields

        private string _homePageUrl;
        string _dailyScheduleEntriesEmailDirectory;
        string _dailyScheduleEntriesEmailFilesDirectory;

        #endregion //Fields

        #region Properties

        public string HomePageUrl
        {
            get { return _homePageUrl; }
        }

        public string DailyScheduleEntriesEmailDirectory
        {
            get { return _dailyScheduleEntriesEmailDirectory; }
        }

        public string DailyScheduleEntriesFilesDirectory
        {
            get { return _dailyScheduleEntriesEmailFilesDirectory; }
        }

        #endregion //Properties

        #region Methods

        protected override bool ProcessNextItemInQueue(
            RepeatEntityContext context, 
            Guid processorId, 
            string organizationIdentifierIndicator, 
            string subscriberIdentifierIndicator)
        {
            DateTime currentDateTime = DateTime.Now;
            string logMessage = null;
            List<Organization> organizations = context.GetOrganizationsByFilter(null);
            foreach (Organization organization in organizations)
            {
                List<User> users = null;
                if (!IsOrganizationReadyForEmails(processorId, context, currentDateTime, organization, out users) || users == null)
                {
                    continue;
                }
                List<ScheduleEntryView> entries = context.GetScheduleEntryViewsForOrganizationByFilter(null, organization.OrganizationId, currentDateTime);

                StringBuilder auditM = new StringBuilder();
                auditM.AppendLine(string.Format("Sending Daily Schedule Entries emails to {0} {1} users:", typeof(Organization).Name, organization.Name));
                users.ForEach(p => auditM.AppendLine(string.Format("{0} ({1})", p.UserName, p.EmailAddress)));
                logMessage = auditM.ToString();
                GOC.Instance.Logger.LogMessage(new LogMessage(logMessage, LogMessageType.Information, LoggingLevel.Maximum));
                context.LogProcesorAction(processorId, logMessage, LogMessageType.Information.ToString());

                if (_emailSender.SendScheduleEntriesListEmailHtml(
                    organization,
                    users,
                    currentDateTime,
                    entries,
                    _homePageUrl,
                    _dailyScheduleEntriesEmailDirectory,
                    _dailyScheduleEntriesEmailFilesDirectory))
                {
                    StringBuilder successMessage = new StringBuilder();
                    successMessage.AppendLine(string.Format("Successfully sent Daily Schedule Entries emails to {0} {1} users:", typeof(Organization).Name, organization.Name));
                    users.ForEach(p => successMessage.AppendLine(string.Format("{0} ({1})", p.UserName, p.EmailAddress)));
                    logMessage = successMessage.ToString();
                    GOC.Instance.Logger.LogMessage(new LogMessage(logMessage, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                    context.LogProcesorAction(processorId, logMessage, LogMessageType.SuccessAudit.ToString());
                }
            }
            return false; //Nothing in the queue, hence the process will run again at its next interval instead of immediately to process another item on the queue.
        }

        private bool IsOrganizationReadyForEmails(
            Guid processorId, 
            RepeatEntityContext context, 
            DateTime currentDateTime, 
            Organization organization, 
            out List<User> organizationUsers)
        {
            organizationUsers = null;
            string logMessage = null;
            if (!organization.EnableEmailNotifications || !organization.EnableDailyScheduleEntriesEmailNotifications)
            {
                return false;
            }
            if (currentDateTime.TimeOfDay < organization.DailyScheduleEntriesEmailNotificationTime) //Current time has not exceeded the organization's scheduled time for emails to be sent. Hence will wait for time to elapse before sending the emails.
            {
                //logMessage = string.Format("Current time {0} has not exceeded the {1} of {2} for {3} {4}.",
                //    currentDateTime.TimeOfDay.ToString(),
                //    EntityReader<Organization>.GetPropertyName(p => p.DailyScheduleEntriesEmailNotificationTime, true),
                //    organization.DailyScheduleEntriesEmailNotificationTime.ToString(),
                //    typeof(Organization).Name,
                //    organization.Name);
                //GOC.Instance.Logger.LogMessage(new LogMessage(logMessage, LogMessageType.Information, LoggingLevel.Maximum));
                //context.LogProcesorAction(processorId, logMessage, LogMessageType.Information.ToString());
                return false;
            }
            List<EmailLog> emailsSentToday = context.GetEmailLogForOrganizationForDay(organization.OrganizationId, currentDateTime, EmailCategory.DailyScheduleEntries.ToString());
            if (emailsSentToday.Count > 0) //Emails have already been sent today.
            {
                //logMessage = string.Format("Daily Schedule Entries emails have already been sent to {0} {1} for today {2}.",
                //    typeof(Organization).Name,
                //    organization.Name,
                //    DataShaper.GetDefaultDateString(currentDateTime));
                //GOC.Instance.Logger.LogMessage(new LogMessage(logMessage, LogMessageType.Information, LoggingLevel.Maximum));
                //context.LogProcesorAction(processorId, logMessage, LogMessageType.Information.ToString());
                return false;
            }
            organizationUsers = context.GetUsersOfRole(UserRole.OrganizationAdmin, organization.OrganizationId, true);
            if (organizationUsers.Count < 1) //There are no users with with the correct role for the email to be sent to them.
            {
                logMessage = string.Format("{0} {1} does not have any users with the role of {2} for {3} to send emails to them.",
                    typeof(Organization).Name,
                    organization.Name,
                    UserRole.OrganizationAdmin.ToString(),
                    typeof(ScheduleEntriesDailyProcessor).Name);
                context.LogProcesorAction(processorId, logMessage, LogMessageType.Warning.ToString());
                GOC.Instance.Logger.LogMessage(new LogMessage(logMessage, LogMessageType.Warning, LoggingLevel.Maximum));
                return false;
            }
            return true;
        }

        #endregion //Methods
    }
}
