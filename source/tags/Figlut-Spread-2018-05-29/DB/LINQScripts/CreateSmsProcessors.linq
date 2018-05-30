<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

SmsProcessors.DeleteAllOnSubmit(SmsProcessors);
SubmitChanges();

SmsProcessor smsReceivedQueueProcessor = new SmsProcessor()
{
	SmsProcessorId = Guid.Parse("999ED78E-1CB6-4A56-89AD-AEA9DA63A035"),
	Name = "Sms Received Queue Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
SmsProcessors.InsertOnSubmit(smsReceivedQueueProcessor);
SmsProcessor smsSentQueueProcessor = new SmsProcessor()
{
	SmsProcessorId = Guid.Parse("232E0992-CA64-4C1E-B960-A79469927D5A"),
	Name = "Sms Sent Queue Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
SmsProcessors.InsertOnSubmit(smsSentQueueProcessor);

SmsProcessor smsDeliveryReportQueueProcessor = new SmsProcessor()
{
	SmsProcessorId = Guid.Parse("BA82D2D2-B6BD-4281-B396-D327FE94CE9D"),
	Name = "Sms Delivery Report Queue Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
SmsProcessors.InsertOnSubmit(smsDeliveryReportQueueProcessor);

SubmitChanges();
SmsProcessors.Dump();