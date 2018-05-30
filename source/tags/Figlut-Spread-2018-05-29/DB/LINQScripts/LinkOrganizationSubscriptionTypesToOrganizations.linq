<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

OrganizationSubscriptionType subscription = OrganizationSubscriptionTypes.Where (ost => ost.Name == "Free").FirstOrDefault();

List<Organization> organizations = Organizations.ToList();
foreach (var o in organizations)
{
	o.OrganizationSubscriptionTypeId = subscription.OrganizationSubscriptionTypeId;
	o.OrganizationSubscriptionEnabled = true;
}
SubmitChanges();
Organizations.Dump();