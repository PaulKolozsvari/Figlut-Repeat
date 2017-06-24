<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

SmsReceivedLogs.DeleteAllOnSubmit(SmsReceivedLogs);
SubmitChanges();

SmsReceivedLog s1 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27833958283",
	MessageId = "46350981",
	MessageContents = "BLUECOFFEE John",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(1, 0, 0, 0))
};
SmsReceivedLog s2 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350980",
	MessageContents = "BLUECOFFEE Jane 1",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(2, 0, 0, 0))
};
SmsReceivedLog s3 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350982",
	MessageContents = "BLUECOFFEE Jane 2",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now
};
SmsReceivedLog s4 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350983",
	MessageContents = "BLUECOFFEE Jane 3",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(3, 0, 0, 0))
};
SmsReceivedLog s5 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350984",
	MessageContents = "BLUECOFFEE Jane 4",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(3, 0, 0, 0))
};
SmsReceivedLog s6 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350985",
	MessageContents = "BLUECOFFEE Jane 5",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(4, 0, 0, 0))
};
SmsReceivedLog s7 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350986",
	MessageContents = "BLUECOFFEE Jane 6",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(4, 0, 0, 0))
};
SmsReceivedLog s8 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350987",
	MessageContents = "BLUECOFFEE Jane 7",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(5, 0, 0, 0))
};
SmsReceivedLog s9 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350988",
	MessageContents = "BLUECOFFEE Jane 8",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(6, 0, 0, 0))
};
SmsReceivedLog s10 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350989",
	MessageContents = "BLUECOFFEE Jane 9",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(7, 0, 0, 0))
};
SmsReceivedLog s11 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350990",
	MessageContents = "BLUECOFFEE Jane 10",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(8, 0, 0, 0))
};
SmsReceivedLog s12 = new SmsReceivedLog()
{
	SmsReceivedLogId = Guid.NewGuid(),
	CellPhoneNumber = "+27826577102",
	MessageId = "46350991",
	MessageContents = "BLUECOFFEE Jane 11",
	DateReceivedOriginalFormat = "201604031950",
	Campaign = null,
	DataField = null,
	Nonce = "3651b367-c0ec-4880-a97f-f2e478da7e83",
	NonceDateOriginalFormat = "20160403195530",
	Checksum = "cb6097a1473f307997c6b713be50a2a97a1fb4e5",
	SmsProviderCode = 0,
	DateCreated = DateTime.Now.Subtract(new System.TimeSpan(9, 0, 0, 0))
};
SmsReceivedLogs.InsertOnSubmit(s1);
SmsReceivedLogs.InsertOnSubmit(s2);
SmsReceivedLogs.InsertOnSubmit(s3);
SmsReceivedLogs.InsertOnSubmit(s4);
SmsReceivedLogs.InsertOnSubmit(s5);
SmsReceivedLogs.InsertOnSubmit(s6);
SmsReceivedLogs.InsertOnSubmit(s7);
SmsReceivedLogs.InsertOnSubmit(s8);
SmsReceivedLogs.InsertOnSubmit(s9);
SmsReceivedLogs.InsertOnSubmit(s10);
SmsReceivedLogs.InsertOnSubmit(s11);
SmsReceivedLogs.InsertOnSubmit(s12);

SubmitChanges();
SmsReceivedLogs.Count().Dump();
SmsReceivedLogs.Dump();