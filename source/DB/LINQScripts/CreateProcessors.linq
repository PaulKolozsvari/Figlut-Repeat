<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutRepeat</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

Processors.DeleteAllOnSubmit(Processors);
SubmitChanges();

Processor smsReceivedQueueProcessor = new Processor()
{
	ProcessorId = Guid.Parse("999ED78E-1CB6-4A56-89AD-AEA9DA63A035"),
	Name = "Sms Received Queue Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
Processors.InsertOnSubmit(smsReceivedQueueProcessor);
Processor smsSentQueueProcessor = new Processor()
{
	ProcessorId = Guid.Parse("232E0992-CA64-4C1E-B960-A79469927D5A"),
	Name = "Sms Sent Queue Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
Processors.InsertOnSubmit(smsSentQueueProcessor);

Processor smsDeliveryReportQueueProcessor = new Processor()
{
	ProcessorId = Guid.Parse("BA82D2D2-B6BD-4281-B396-D327FE94CE9D"),
	Name = "Sms Delivery Report Queue Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
Processors.InsertOnSubmit(smsDeliveryReportQueueProcessor);

Processor scheduleProcessor = new Processor()
{
	ProcessorId = Guid.Parse("5259E5FA-D001-494A-97AC-DE2897ABAC08"),
	Name = "Schedule Processor",
	ExecutionInterval = 10000,
	DateCreated = DateTime.Now
};
Processors.InsertOnSubmit(scheduleProcessor);

SubmitChanges();
Processors.Dump();