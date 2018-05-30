<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

OrganizationSubscriptionTypes.DeleteAllOnSubmit(OrganizationSubscriptionTypes);
SubmitChanges();


Guid freeId = Guid.Parse("26186414-77E5-49C7-BFBC-9D9D52D35DF4");
Guid bronzeId = Guid.Parse("91FBE78E-6161-45CA-AA58-E227AAA132B1");
Guid silverId = Guid.Parse("446FEEB7-1332-4607-9FAF-BD737A107434");
Guid goldId = Guid.Parse("40454E83-0D2C-4ED8-AFAA-2C8F69B322B6");
Guid platinumId = Guid.Parse("4F0C76AB-1473-4116-86FC-B0EAE6C62214");

OrganizationSubscriptionType free = new OrganizationSubscriptionType()
{
	OrganizationSubscriptionTypeId = freeId,
	Name = "Free",
	MonthlySubscriptionPrice = 0,
	SmsUnitPrice = 0.49M,
	DateCreated = DateTime.Now
};
OrganizationSubscriptionTypes.InsertOnSubmit(free);

OrganizationSubscriptionType bronze = new OrganizationSubscriptionType()
{
	OrganizationSubscriptionTypeId = bronzeId,
	Name = "Bronze",
	MonthlySubscriptionPrice = 99.00M,
	SmsUnitPrice = 0.29M,
	DateCreated = DateTime.Now
};
OrganizationSubscriptionTypes.InsertOnSubmit(bronze);

OrganizationSubscriptionType silver = new OrganizationSubscriptionType()
{
	OrganizationSubscriptionTypeId = silverId,
	Name = "Silver",
	MonthlySubscriptionPrice = 199.00M,
	SmsUnitPrice = 0.25M,
	DateCreated = DateTime.Now
};
OrganizationSubscriptionTypes.InsertOnSubmit(silver);

OrganizationSubscriptionType gold = new OrganizationSubscriptionType()
{
	OrganizationSubscriptionTypeId = goldId,
	Name = "Gold",
	MonthlySubscriptionPrice = 399.00M,
	SmsUnitPrice = 0.23M,
	DateCreated = DateTime.Now
};
OrganizationSubscriptionTypes.InsertOnSubmit(gold);

OrganizationSubscriptionType platinum = new OrganizationSubscriptionType()
{
	OrganizationSubscriptionTypeId = platinumId,
	Name = "Platinum",
	MonthlySubscriptionPrice = 999.00M,
	SmsUnitPrice = 0.21M,
	DateCreated = DateTime.Now
};
OrganizationSubscriptionTypes.InsertOnSubmit(platinum);

SubmitChanges();

OrganizationSubscriptionTypes.Dump();