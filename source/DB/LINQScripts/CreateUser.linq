<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

string organization = "Figlut";
string userName = "monitis";
string password = "m0N1Ti$-T3sT";

User u = Users.Where (us => us.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
if (u != null)
{
	Users.DeleteOnSubmit(u);
	SubmitChanges();
}
Organization o = Organizations.Where (or => or.Identifier == organization).FirstOrDefault();
Users.InsertOnSubmit(new User()
{
	UserId = Guid.NewGuid(),
	UserName = userName,
	EmailAddress = "teamfiglut.com",
	Password = password,
	OrganizationId = o.OrganizationId,
	RoleId = 2,
	DateCreated = DateTime.Now,
});
SubmitChanges();

Users.Dump();