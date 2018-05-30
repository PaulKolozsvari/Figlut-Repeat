<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

SmsCampaigns.DeleteAllOnSubmit(SmsCampaigns);
Subscriptions.DeleteAllOnSubmit(Subscriptions);
Subscribers.DeleteAllOnSubmit(Subscribers);
Users.DeleteAllOnSubmit(Users);
Organizations.DeleteAllOnSubmit(Organizations);
SubmitChanges();

//Organizations
Organization orgFiglut = new Organization()
{
	OrganizationId = Guid.NewGuid(),
	Name = "Figlut",
	Identifier = "Figlut",
	EmailAddress = "paulkolo@figlut.com",
	Address = "15 Lewerik crescent, Birch Acres, Kempton Park, 1619",
	DateCreated = DateTime.Now
};
Organizations.InsertOnSubmit(orgFiglut);

Organization orgBinaryChef = new Organization()
{
	OrganizationId = Guid.NewGuid(),
	Name = "Binary Chef",
	Identifier = "BinaryChef",
	EmailAddress = "paul.kolozsvari@binarychef.com",
	Address = "P.O. Box 9508 Kempton Gate, Kempton Park",
	DateCreated = DateTime.Now
};
Organizations.InsertOnSubmit(orgBinaryChef);

Organization orgStandardPharmacy = new Organization()
{
	OrganizationId = Guid.NewGuid(),
	Name = "Standard Pharmacy",
	Identifier = "StandardPharmacy",
	EmailAddress = "standardpharmacy@telkomsa.net",
	Address = "Standard Bank Building, Johannesburg CBD, South Africa",
	DateCreated = DateTime.Now
};
Organizations.InsertOnSubmit(orgStandardPharmacy);

//Users
User userSystemAdmin = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "paulk",
	EmailAddress = "paulkolo@figlut.com",
	Password = "27830529",
	OrganizationId = orgFiglut.OrganizationId,
	RoleId = 7,
	DateCreated = DateTime.Now
};
Users.InsertOnSubmit(userSystemAdmin);

User paulFiglutUser = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "paulkolo",
	EmailAddress = "paul.kolozsvari@figlut.com",
	Password = "27830529",
	OrganizationId = orgFiglut.OrganizationId,
	RoleId = 2,
	DateCreated = DateTime.Now
};
Users.InsertOnSubmit(paulFiglutUser);

User userKate = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "kate",
	EmailAddress = "kate@gmail.com",
	Password = "27830529",
	OrganizationId = orgBinaryChef.OrganizationId,
	RoleId = 4,
	DateCreated = DateTime.Now
};
Users.InsertOnSubmit(userKate);

User userPaul = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "paul.kolozsvari",
	EmailAddress = "paul.kolozsvari@binarychef.com",
	Password = "27830529",
	OrganizationId = orgBinaryChef.OrganizationId,
	RoleId = 2,
	DateCreated = DateTime.Now
};
Users.InsertOnSubmit(userPaul);

User userWarren = new User()
{
	UserId = Guid.NewGuid(),
	UserName = "StandardPharmacy",
	EmailAddress = "standardpharmacy@telkomsa.net",
	Password = "$t@nd@Rd",
	OrganizationId = orgStandardPharmacy.OrganizationId,
	RoleId = 2,
	DateCreated = DateTime.Now
};
Users.InsertOnSubmit(userWarren);

//Subscribers
Subscriber subscriberPaul = new Subscriber()
{
	SubscriberId = Guid.NewGuid(),
	CellPhoneNumber = "0833958283",
	Name = "PaulK",
	IsEnabled = true,
	DateCreated = DateTime.Now
};
Subscribers.InsertOnSubmit(subscriberPaul);
Subscriber subscriberJosif = new Subscriber()
{
	SubscriberId = Guid.NewGuid(),
	CellPhoneNumber = "0826577102",
	Name = "JosifK",
	IsEnabled = false,
	DateCreated = DateTime.Now
};
Subscribers.InsertOnSubmit(subscriberJosif);
Subscriber subscriberKate = new Subscriber()
{
	SubscriberId = Guid.NewGuid(),
	CellPhoneNumber = "0831234567",
	Name = "Kate",
	IsEnabled = false,
	DateCreated = DateTime.Now
};
Subscribers.InsertOnSubmit(subscriberKate);

//Subscriptions
Subscription orgSubPaulBinaryChef = new Subscription()
{
	SubscriptionId = Guid.NewGuid(),
	OrganizationId = orgBinaryChef.OrganizationId,
	SubscriberId = subscriberPaul.SubscriberId,
	Enabled = true,
	DateCreated = DateTime.Now
};
Subscriptions.InsertOnSubmit(orgSubPaulBinaryChef);
Subscription orgSubKateBinaryChef = new Subscription()
{
	SubscriptionId = Guid.NewGuid(),
	OrganizationId = orgBinaryChef.OrganizationId,
	SubscriberId = subscriberKate.SubscriberId,
	Enabled = false,
	DateCreated = DateTime.Now
};
Subscriptions.InsertOnSubmit(orgSubKateBinaryChef);
Subscription orgSubJosifFiglut = new Subscription()
{
	SubscriptionId = Guid.NewGuid(),
	OrganizationId = orgFiglut.OrganizationId,
	SubscriberId = subscriberJosif.SubscriberId,
	Enabled = false,
	DateCreated = DateTime.Now
};
Subscriptions.InsertOnSubmit(orgSubJosifFiglut);
Subscription orgSubKateFiglut = new Subscription()
{
	SubscriptionId = Guid.NewGuid(),
	OrganizationId = orgFiglut.OrganizationId,
	SubscriberId = subscriberKate.SubscriberId,
	Enabled = true,
	DateCreated = DateTime.Now
};
Subscriptions.InsertOnSubmit(orgSubKateFiglut);

//SMS Campaigns
SmsCampaign campaign1 = new SmsCampaign()
{
	SmsCampaignId = Guid.NewGuid(),
	Name = "Free Coffee Campaign",
	MessageContents = "Come to Blue Coffee this Saturday (18 March 2017) and get free coffee by presenting this code: ZA112.",
	OrganizationSelectedCode = "ZA112",
	OrganizationId = orgFiglut.OrganizationId,
	DateCreated = DateTime.Now
};
SmsCampaigns.InsertOnSubmit(campaign1);

SmsCampaign campaign2 = new SmsCampaign()
{
	SmsCampaignId = Guid.NewGuid(),
	Name = "Free Donut campaign",
	MessageContents = "Come to Blue Coffee this Sunday (19 March 2017) and get free Donut by presenting this code: ABC123.",
	OrganizationSelectedCode = "ABC123",
	OrganizationId = orgFiglut.OrganizationId,
	DateCreated = DateTime.Now
};
SmsCampaigns.InsertOnSubmit(campaign2);

SmsCampaign campaign3 = new SmsCampaign()
{
	SmsCampaignId = Guid.NewGuid(),
	Name = "Free Donut campaign",
	MessageContents = "Come to Binary Chef this Sunday (17 March 2017) and get free hard drive by presenting this code: H5678.",
	OrganizationSelectedCode = "H5678",
	OrganizationId = orgBinaryChef.OrganizationId,
	DateCreated = DateTime.Now
};
SmsCampaigns.InsertOnSubmit(campaign3);

SubmitChanges();
Organizations.Dump();
Users.Dump();
Subscribers.Dump();
Subscriptions.Dump();