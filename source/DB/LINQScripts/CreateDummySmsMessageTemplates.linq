<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

Organization standardPharmacy = Organizations.Where (o => o.Name.ToLower() == "standard pharmacy").FirstOrDefault();
standardPharmacy.Dump();

SmsMessageTemplates.DeleteAllOnSubmit(SmsMessageTemplates);
SubmitChanges();

SmsMessageTemplate m1 = new SmsMessageTemplate()
{
	SmsMessageTemplateId = Guid.NewGuid(),
	OrganizationId = standardPharmacy.OrganizationId,
	Message = @"Good morning. Yet another month has flown by. We will send your repeats to you within the next 24-48 hours. Standard Pharmacy.",
	DateCreated = DateTime.Now
};
SmsMessageTemplates.InsertOnSubmit(m1);

SmsMessageTemplate m2 = new SmsMessageTemplate()
{
	SmsMessageTemplateId = Guid.NewGuid(),
	OrganizationId = standardPharmacy.OrganizationId,
	Message = @"Good morning. Yet another month has flown by. Your repeats are ready for collection. Standard Pharmacy.",
	DateCreated = DateTime.Now
};
SmsMessageTemplates.InsertOnSubmit(m2);
SubmitChanges();

SmsMessageTemplates.Dump();