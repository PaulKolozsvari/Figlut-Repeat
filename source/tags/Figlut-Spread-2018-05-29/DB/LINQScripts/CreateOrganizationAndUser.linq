<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

Organization organization = null;
string organizationName = "Park Village Auctioneers";
string organizationIdentifier = "parkvillage";
organization = Organizations.Where (o => o.Identifier == organizationIdentifier).FirstOrDefault();
if(organization != null)
{
	Organizations.DeleteOnSubmit(organization);
	SubmitChanges();
}
organization = new Organization()
{
	OrganizationId = Guid.NewGuid(),
	Name = organizationName,
	Identifier = organizationIdentifier,
	EmailAddress = "nhurwitz@parkvillage.co.za",
	Address = "Edenvale",
	DateCreated = DateTime.Now
};
Organizations.InsertOnSubmit(organization);
SubmitChanges();

string userName = "NormanH";
string password = "parkvillage";

User u = Users.Where (us => us.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
if (u != null)
{
	Users.DeleteOnSubmit(u);
	SubmitChanges();
}
Users.InsertOnSubmit(new User()
{
	UserId = Guid.NewGuid(),
	UserName = userName,
	EmailAddress = "nhurwitz@parkvillage.co.za",
	Password = password,
	OrganizationId = organization.OrganizationId,
	RoleId = 2,
	DateCreated = DateTime.Now,
});
SubmitChanges();

Users.Dump();