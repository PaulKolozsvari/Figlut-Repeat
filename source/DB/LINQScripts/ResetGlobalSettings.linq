<Query Kind="Statements">
  <Connection>
    <ID>609802f6-b984-4a66-9e38-919e8455af43</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>localhost\SQLEXPRESS2014</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Database>FiglutRepeat</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

GlobalSettings.DeleteAllOnSubmit(GlobalSettings);
SubmitChanges();

GlobalSetting companyPhoneNumber = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "CompanyPhoneNumber",
	SettingValue = "+27 (87) 240 5099",
	Description = "Main contact number for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(companyPhoneNumber);
GlobalSetting companySupportEmailAddress = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "CompanySupportEmailAddress",
	SettingValue = "team@figlut.com",
	Description = "Support email address for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(companySupportEmailAddress);
GlobalSetting marketingEmail = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "CompanyMarketingEmailAddress",
	SettingValue = "team@figlut.com",
	Description = "Marketing email address for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(marketingEmail);
GlobalSetting generalEmail = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "CompanyGeneralEmailAddress",
	SettingValue = "team@figlut.com",
	Description = "Main email address for this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(generalEmail);
GlobalSetting figlutAddress = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "CompanyPostalAddress",
	SettingValue = "P.O. Box 9508, Kempton Gate, Edleen, Kempton Park",
	Description = "Address of this company.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(figlutAddress);
GlobalSetting homePageUrl = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Contact",
	SettingName = "HomePageUrl",
	SettingValue = "http://repeat.figlut.com",
	Description = "The home page URL of the Figlut Repeat website.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(homePageUrl);
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

GlobalSetting processorLogDaysToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "ProcessorLogDaysToDisplay",
	SettingValue = "1",
	Description = "The number of days to display Processor Log messages when first loading a grid.",
	DateCreated = DateTime.Now
};
GlobalSettings.InsertOnSubmit(processorLogDaysToDisplay);

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
GlobalSetting generatedPasswordLength = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "GeneratedPasswordLength",
	SettingValue = "10",
	DateCreated = DateTime.Now,
	Description = "The length of generated passwords for users."
};
GlobalSettings.InsertOnSubmit(generatedPasswordLength);
GlobalSetting generatedPasswordNumberOfNonAlphanumericCharacters = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "GeneratedPasswordNumberOfNonAlphanumericCharacters",
	SettingValue = "1",
	DateCreated = DateTime.Now,
	Description = "The of non-alphanumeric numeric characters in generated passwords for users."
};
GlobalSettings.InsertOnSubmit(generatedPasswordNumberOfNonAlphanumericCharacters);
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
GlobalSetting logUserLastActivityDate = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "System",
	SettingName = "LogUserLastActivityDate",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to log the date of the last activity of users."
};
GlobalSettings.InsertOnSubmit(logUserLastActivityDate);
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
GlobalSetting organizationLeadsPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "OrganizationLeadsPerPageToDisplay",
	SettingValue = "4",
	DateCreated = DateTime.Now,
	Description = "Number of leads to display per grid page."
};
GlobalSettings.InsertOnSubmit(organizationLeadsPerPageToDisplay);
GlobalSetting processorPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "ProcessorsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Processors to display per grid page."
};
GlobalSettings.InsertOnSubmit(processorPerPageToDisplay);

GlobalSetting processorLogPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "ProcessorLogsPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Processors Log messages to display per grid page."
};
GlobalSettings.InsertOnSubmit(processorLogPerPageToDisplay);

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

GlobalSetting schedulesPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "SchedulesPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Schedules to display per grid page."
};
GlobalSettings.InsertOnSubmit(schedulesPerPageToDisplay);

GlobalSetting scheduleEntriesPerPageToDisplay = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "ScheduleEntriesPerPageToDisplay",
	SettingValue = "8",
	DateCreated = DateTime.Now,
	Description = "Number of Schedule Entries to display per grid page."
};
GlobalSettings.InsertOnSubmit(scheduleEntriesPerPageToDisplay);

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

GlobalSetting processorMessageTrimOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "ProcessorMessageTrimOnGrid",
	SettingValue = true.ToString(),
	DateCreated = DateTime.Now,
	Description = "Whether or not to trim the Processor messages on the grid."
};
GlobalSettings.InsertOnSubmit(processorMessageTrimOnGrid);
GlobalSetting processorMessageTrimLengthOnGrid = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Grid",
	SettingName = "ProcessorMessageTrimLengthOnGrid",
	SettingValue = "15",
	DateCreated = DateTime.Now,
	Description = "Number of characters to display of the Processor messages on the grid."
};
GlobalSettings.InsertOnSubmit(processorMessageTrimLengthOnGrid);

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
	Category = "Schedule",
	SettingName = "DefaultRepeatDaysInterval",
	SettingValue = "30",
	DateCreated = DateTime.Now,
	Description = "The default number of days displayed when creating a repeat Schedule."
};
GlobalSettings.InsertOnSubmit(defaultDaysRepeatInterval);

//Bank Deails for receiving payments by customers
GlobalSetting bankName = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Bank",
	SettingName = "BankName",
	SettingValue = "Standard Bank",
	DateCreated = DateTime.Now,
	Description = "The name of the bank where deposits need to be made by customers."
};
GlobalSettings.InsertOnSubmit(bankName);
GlobalSetting bankAccountType = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Bank",
	SettingName = "BankAccountType",
	SettingValue = "Cheque Acccount",
	DateCreated = DateTime.Now,
	Description = "The type of bank account where deposits need to be made to by customers."
};
GlobalSettings.InsertOnSubmit(bankAccountType);
GlobalSetting bankAccountNumber = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Bank",
	SettingName = "BankAccountNumber",
	SettingValue = "02-280-185-5",
	DateCreated = DateTime.Now,
	Description = "The bank account number of the account where deposits need to be made to by customers."
};
GlobalSettings.InsertOnSubmit(bankAccountNumber);
GlobalSetting bankBranchCode = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Bank",
	SettingName = "BankAccountBranchCode",
	SettingValue = "051001",
	DateCreated = DateTime.Now,
	Description = "The bank branch code of the account where deposits need to be made to by customers."
};
GlobalSettings.InsertOnSubmit(bankBranchCode);
GlobalSetting bankBranchName = new GlobalSetting()
{
	GlobalSettingId = Guid.NewGuid(),
	Category = "Bank",
	SettingName = "BankAccountBranchName",
	SettingValue = "Standard Bank Universal Branch Code",
	DateCreated = DateTime.Now,
	Description = "The branch name of the bank account where deposits need to be made to by customers."
};
GlobalSettings.InsertOnSubmit(bankBranchName);

SubmitChanges();
GlobalSettings.OrderBy (gs => gs.SettingName).Dump();