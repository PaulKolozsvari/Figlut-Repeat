<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutSpread</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

User standardUser = Users.Where (u => u.UserName == "StandardPharmacy").FirstOrDefault();
standardUser.Dump();
Organization standardOrg = standardUser.Organization;
standardOrg.Dump();

List<SmsSentLog> smsSentList = SmsSentLogs.ToList();
smsSentList.Count().Dump();
//foreach (var sms in smsSentList)
//{
//	sms.SenderUserId = standardUser.UserId;
//	sms.OrganizationId = standardOrg.OrganizationId;
//}

Dictionary<string, SmsSentLog> subscriberCellPhoneNumbers = new Dictionary<string, SmsSentLog>(); //Create a dictionary of unique cell phone numbers.
foreach (var sms in smsSentList)
{
	if (!subscriberCellPhoneNumbers.ContainsKey(sms.CellPhoneNumber))
	{
		subscriberCellPhoneNumbers.Add(sms.CellPhoneNumber, sms);
	}
}
subscriberCellPhoneNumbers.Count.Dump();

Subscribers.DeleteAllOnSubmit(Subscribers);
SubmitChanges();
Subscriptions.DeleteAllOnSubmit(Subscriptions);
SubmitChanges();
foreach (KeyValuePair<string, SmsSentLog> cellPhoneNumber in subscriberCellPhoneNumbers)
{
	string cellPhoneNumberTrimmed = cellPhoneNumber.Key.Trim();
	Subscriber subscriber = Subscribers.Where (s => s.CellPhoneNumber.Trim() == cellPhoneNumberTrimmed).FirstOrDefault();
	if (subscriber == null)
	{
		subscriber = new Subscriber()
		{ 
			SubscriberId = Guid.NewGuid(),
			CellPhoneNumber = cellPhoneNumber.Key.Trim(),
			IsEnabled = true,
			Name = null,
			DateCreated = DateTime.Now
		};
		Subscribers.InsertOnSubmit(subscriber);
		SubmitChanges();
	}
	Subscription subscription = Subscriptions.Where (s => s.OrganizationId == standardOrg.OrganizationId && s.SubscriberId == subscriber.SubscriberId).FirstOrDefault();
	if (subscription == null)
	{
		subscription = new Subscription()
		{
			SubscriptionId = Guid.NewGuid(),
			OrganizationId = standardOrg.OrganizationId,
			SubscriberId = subscriber.SubscriberId,
			Enabled = true,
			DateCreated = DateTime.Now
		};
		Subscriptions.InsertOnSubmit(subscription);
		SubmitChanges();
	}
}

SubmitChanges();

Subscribers.Dump();
Subscriptions.Dump();

Subscriber paul = Subscribers.Where (s => s.CellPhoneNumber.Contains("0833958283")).FirstOrDefault();
paul.Dump();
paul.CellPhoneNumber = "+27833958283";
SubmitChanges();


//Link the Received SMS'
List<SmsReceivedLog> smsReceivedLogs = SmsReceivedLogs.ToList();
foreach (var sms in smsReceivedLogs)
{
	sms.OrganizationId = standardOrg.OrganizationId;
	SubmitChanges();
}
