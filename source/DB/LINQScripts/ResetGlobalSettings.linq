<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

GlobalSettings.DeleteAllOnSubmit(GlobalSettings);
SubmitChanges();

GlobalSetting figlutPhone = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "FiglutPhoneNumber",
	SettingValue = "+27 (87) 240 5099",
	Description = "Main contact number for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(figlutPhone);
GlobalSetting supportEmail = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "FiglutSupportEmailAddress",
	SettingValue = "team@figlut.com",
	Description = "Support email address for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(supportEmail);
GlobalSetting marketingEmail = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "FiglutMarketingEmailAddress",
	SettingValue = "team@figlut.com",
	Description = "Marketting email address for company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(marketingEmail);
GlobalSetting generalEmail = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "FiglutGeneralEmailAddress",
	SettingValue = "team@figlut.com",
	Description = "Main email address for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(generalEmail);
GlobalSetting figlutAddress = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "FiglutAddress",
	SettingValue = "P.O. Box 9508, Kempton Gate, Edleen, Kempton Park",
	Description = "Address of this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(figlutAddress);
GlobalSetting smsDaysToList = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsDaysToDisplay",
	SettingValue = "1",
	Description = "The number of days to display SMS' when first loading a grid.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(smsDaysToList);

GlobalSetting smsProcessorLogDaysToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsProcessorLogDaysToDisplay",
	SettingValue = "1",
	Description = "The number of days to display SMS Processor Log messages when first loading a grid.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(smsProcessorLogDaysToDisplay);

GlobalSetting webRequestActivityDaysToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "WebRequestActivityDaysToDisplay",
	SettingValue = "1",
	Description = "The number of days to display Web Request Activities when first loading a grid.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(webRequestActivityDaysToDisplay);

//To add the web site to be managed.
GlobalSetting disableScreenScalingForMobileDevices = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "DisableScreenScalingForMobileDevices",
	SettingValue = false.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to disable scaling of HTML screens for mobile devices."
};
GlobalSettings.InsertOnSubmit(disableScreenScalingForMobileDevices);
GlobalSetting createPersistentAuthenticationCookie = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "CreatePersistentAuthenticationCookie",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to keep a user logged across browser sessions."
};
GlobalSettings.InsertOnSubmit(createPersistentAuthenticationCookie);
GlobalSetting logAllHttpHeaders = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "LogAllHttpHeaders",
	SettingValue = false.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to write all HTTP headers to the event log."
};
GlobalSettings.InsertOnSubmit(logAllHttpHeaders);
GlobalSetting defaultCurrencySymbol = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "DefaultCurrencySymbol",
	SettingValue = "R",
	DateCreated = DateTime.Now,
	Description = "The default currency symbol to use across the system."
};
GlobalSettings.InsertOnSubmit(defaultCurrencySymbol);

//Web Request Activity
GlobalSetting logGetWebRequestActivity = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "LogGetWebRequestActivity",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to log Web Request Activies that have the GET verb."
};
GlobalSettings.InsertOnSubmit(logGetWebRequestActivity);
GlobalSetting logPostWebRequestActivity = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "LogPostWebRequestActivity",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to log Web Request Activies that have the POST verb."
};
GlobalSettings.InsertOnSubmit(logPostWebRequestActivity);
GlobalSetting logPutWebRequestActivity = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "LogPutWebRequestActivity",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to log Web Request Activies that have the PUT verb."
};
GlobalSettings.InsertOnSubmit(logPutWebRequestActivity);
GlobalSetting logDeleteWebRequestActivity = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "LogDeleteWebRequestActivity",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to log Web Request Activies that have the Delete verb."
};
GlobalSettings.InsertOnSubmit(logDeleteWebRequestActivity);
GlobalSetting webRequestActivityUserAgentsToExclude = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "WebRequestActivityUserAgentsToExclude",
	SettingValue = "monitis",
	DateCreated = DateTime.Now,
	Description = "CSV strings to search for in User Agents that should not be logged as Web Request Activities."
};
GlobalSettings.InsertOnSubmit(webRequestActivityUserAgentsToExclude);

//WHO IS Web Request
GlobalSetting enableWhoIsWebServiceQuery = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "EnableWhoIsWebServiceQuery",
	SettingValue = false.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not perform a WHO IS query to a WHO IS web service on each web request activity."
};
GlobalSettings.InsertOnSubmit(enableWhoIsWebServiceQuery);

GlobalSetting whoIsWebServiceUrl = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "WhoIsWebServiceUrl",
	SettingValue = "http://ip-api.com/xml",
	DateCreated = DateTime.Now,
	Description = "The url of the WHO IS web service to query."
};
GlobalSettings.InsertOnSubmit(whoIsWebServiceUrl);

GlobalSetting whoIsWebServiceRequestTimeout = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "WhoIsWebServiceRequestTimeout",
	SettingValue = "30000",
	DateCreated = DateTime.Now,
	Description = "The url of the WHO IS web service to query."
};
GlobalSettings.InsertOnSubmit(whoIsWebServiceRequestTimeout);

GlobalSetting enableGoogleAnalytics = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "EnableGoogleAnalytics",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to enable sending web analytics data to Google Analytics."
};
GlobalSettings.InsertOnSubmit(enableGoogleAnalytics);

//Grid
GlobalSetting smsPerPagePageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsPerPagePageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of SMS' to display per grid page."
};
GlobalSettings.InsertOnSubmit(smsPerPagePageToDisplay);

GlobalSetting organizationsPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "OrganizationsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Organnizations to display per grid page."
};
GlobalSettings.InsertOnSubmit(organizationsPerPageToDisplay);

GlobalSetting subscribersPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SubscribersPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Subscribers to display per grid page."
};
GlobalSettings.InsertOnSubmit(subscribersPerPageToDisplay);

GlobalSetting usersPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "UsersPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Users to display per grid page."
};
GlobalSettings.InsertOnSubmit(usersPerPageToDisplay);

GlobalSetting subscriptionsPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SubscriptionsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Subsriptions to display per grid page."
};
GlobalSettings.InsertOnSubmit(subscriptionsPerPageToDisplay);

GlobalSetting smsProcessorPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsProcessorsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of SMS Processors to display per grid page."
};
GlobalSettings.InsertOnSubmit(smsProcessorPerPageToDisplay);

GlobalSetting smsProcessorLogPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsProcessorLogsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of SMS Processors Log messages to display per grid page."
};
GlobalSettings.InsertOnSubmit(smsProcessorLogPerPageToDisplay);

GlobalSetting webRequestActivityPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "WebRequestActivityPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Web Request Activities to display per grid page."
};
GlobalSettings.InsertOnSubmit(webRequestActivityPerPageToDisplay);

GlobalSetting smsCampaignsPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsCampaignsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Global Settings to display per grid page."
};
GlobalSettings.InsertOnSubmit(smsCampaignsPerPageToDisplay);

GlobalSetting countriesPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "CountriesPerPagePageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Countries to display per grid page."
};
GlobalSettings.InsertOnSubmit(countriesPerPageToDisplay);

GlobalSetting publicHolidaysPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "PublicHolidaysPerPagePageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Public Holidays to display per grid page."
};
GlobalSettings.InsertOnSubmit(publicHolidaysPerPageToDisplay);

GlobalSetting smsMessageTemplatesPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsMessageTemplatesPerPagePageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of SMS Message Templates to display per grid page."
};
GlobalSettings.InsertOnSubmit(smsMessageTemplatesPerPageToDisplay);

GlobalSetting repeatSchedulesPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "RepeatSchedulesPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Repeat Schedules to display per grid page."
};
GlobalSettings.InsertOnSubmit(repeatSchedulesPerPageToDisplay);

GlobalSetting repeatScheduleEntriesPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "RepeatScheduleEntriesPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Repeat Schedule Entries to display per grid page."
};
GlobalSettings.InsertOnSubmit(repeatScheduleEntriesPerPageToDisplay);

GlobalSetting globalSettingsPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "GlobalSettingsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Global Settings to display per grid page."
};
GlobalSettings.InsertOnSubmit(globalSettingsPerPageToDisplay);

//Other grid settings
GlobalSetting smsContentsTrimOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsContentsTrimOnGrid",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to trim the SMS contents on the grid."
};
GlobalSettings.InsertOnSubmit(smsContentsTrimOnGrid);
GlobalSetting smsContentsTrimLengthOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsContentsTrimLengthOnGrid",
	SettingValue = "10",
	DateCreated = DateTime.Now,
	Description = "Number of characters to display of the SMS contents on the grid."
};
GlobalSettings.InsertOnSubmit(smsContentsTrimLengthOnGrid);
GlobalSetting cellPhoneNumberTrimOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "CellPhoneNumberTrimOnGrid",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to trim the cell phone on the grid.",
};
GlobalSettings.InsertOnSubmit(cellPhoneNumberTrimOnGrid);
GlobalSetting cellPhoneNumberTrimLengthOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "CellPhoneNumberTrimLengthOnGrid",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of characters to display of the cell phone number on the grid."
};
GlobalSettings.InsertOnSubmit(cellPhoneNumberTrimLengthOnGrid);

GlobalSetting maximumSmsDateRangeDaysToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "MaximumSmsDateRangeDaysToDisplay",
	SettingValue = "31",
	DateCreated = DateTime.Now,
	Description = "Maximum number days of SMS' that regular users can load into the grid."
};
GlobalSettings.InsertOnSubmit(maximumSmsDateRangeDaysToDisplay);

GlobalSetting smsProcessorMessageTrimOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsProcessorMessageTrimOnGrid",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to trim the SMS Processor messages on the grid."
};
GlobalSettings.InsertOnSubmit(smsProcessorMessageTrimOnGrid);
GlobalSetting smsProcessorMessageTrimLengthOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsProcessorMessageTrimLengthOnGrid",
	SettingValue = "15",
	DateCreated = DateTime.Now,
	Description = "Number of characters to display of the SMS Processor messages on the grid."
};
GlobalSettings.InsertOnSubmit(smsProcessorMessageTrimLengthOnGrid);

GlobalSetting smsErrorTrimLengthOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SmsErrorTrimLengthOnGrid",
	SettingValue = "10",
	DateCreated = DateTime.Now,
	Description = "Number of characters to display of the SMS error message on the grid.",
};
GlobalSettings.InsertOnSubmit(smsErrorTrimLengthOnGrid);

GlobalSetting organizationIdentifierIndicator = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "SMS Processor",
	SettingName = "OrganizationIdentifierIndicator",
	SettingValue = "#",
	DateCreated = DateTime.Now,
	Description = "The character prefix that signifies the organization identifier in a received SMS.",
};
GlobalSettings.InsertOnSubmit(organizationIdentifierIndicator);

GlobalSetting subscriberNameIndicator = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "SMS Processor",
	SettingName = "SubscriberNameIndicator",
	SettingValue = "*",
	DateCreated = DateTime.Now,
	Description = "The character prefix that signifies the subscriber name in a received SMS."
};
GlobalSettings.InsertOnSubmit(subscriberNameIndicator);

GlobalSetting maxSmsSentMessageLength = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "SMS Processor",
	SettingName = "MaxSmsSendMessageLength",
	SettingValue = "130",
	DateCreated = DateTime.Now,
	Description = "The maximum length of an SMS be be sent by the system."
};
GlobalSettings.InsertOnSubmit(maxSmsSentMessageLength);

GlobalSetting smsSendMessageSuffix = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "SMS Processor",
	SettingName = "SmsSendMessageSuffix",
	SettingValue = " Include {0} in reply.", //System will substiture {0} with the Organization identifier that is sending the SMS.
	DateCreated = DateTime.Now,
	Description = "The suffix message to append to every SMS that is sent out by the system."
};
GlobalSettings.InsertOnSubmit(smsSendMessageSuffix);

GlobalSetting organizationIdentifierMaxLength = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "SMS Processor",
	SettingName = "OrganizationIdentifierMaxLength",
	SettingValue = "10",
	DateCreated = DateTime.Now,
	Description = "The maximum length of an organization identifier, which will be included in every message sent by the system."
};
GlobalSettings.InsertOnSubmit(organizationIdentifierMaxLength);

GlobalSetting defaultDaysRepeatInterval = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Repeat Schedule",
	SettingName = "DefaultRepeatDaysInterval",
	SettingValue = "30",
	DateCreated = DateTime.Now,
	Description = "The default number of days displayed when creating a Repeat Schedule."
};
GlobalSettings.InsertOnSubmit(defaultDaysRepeatInterval);

SubmitChanges();
GlobalSettings.OrderBy (gs => gs.SettingName).Dump();