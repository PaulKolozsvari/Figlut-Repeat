<Query Kind="Statements">
  <Connection>
    <ID>72244da7-af1c-4a14-a9a5-7097dd8b610c</ID>
    <Persist>true</Persist>
    <Server>PAULKOLOZSV38D1\MSSQLSERVER2012</Server>
    <Database>FiglutRepeat</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

InvoiceItems.DeleteAllOnSubmit(InvoiceItems);
Invoices.DeleteAllOnSubmit(Invoices);
SubmitChanges();

Organization organization = Organizations.Where (o => o.Name == "Binary Chef").FirstOrDefault();
User user = Users.Where (u => u.UserName == "paulk").FirstOrDefault();

Invoice invoice1 = new Invoice()
{
	InvoiceId = Guid.NewGuid(),
	TotalExcludingVat = 750,
	TotalVat = 105,
	TotalIncludingVat = 855,
	OrganizationName = organization.Name,
	OrganizationAttention = "Paul Kolozsvari",
	OrganizationAddress = "9B Sarel van der Walt street",
	PaymentDetailsBankName = "Standard Bank",
	PaymentDetailsAccountType = "Cheque Account",
	PaymentDetailsAccountNumber = "02-280-185-5",
	PaymentDetailsBranchCode = "051001",
	PaymentDetailsBranchName = "Standard Bank Universal Branch Code",
	OrganizationOutstandingBalance = 855,
	OrderId = null,
	CreatedByUserId = user.UserId,
	CreatedByUserName = user.UserName,
	OrganizationId = organization.OrganizationId,
	DateCreated = DateTime.Now,
};
Invoices.InsertOnSubmit(invoice1);
InvoiceItem line1 = new InvoiceItem()
{
	InvoiceItemId = Guid.NewGuid(),
	InvoiceId = invoice1.InvoiceId,
	ProductName = "Subscription Renewal",
	LineNumber = 1,
	Quantity = 1,
	UnitPriceExcludingVat = 500,
	UnitPriceVat = 70,
	UnitPriceIncludingVat = 570,
	TotalLineItemPriceExcludingVat = 500,
	TotalLineItemPriceVat = 70,
	TotalLineItemPriceIncludingVat = 570,
	DateCreated = DateTime.Now
};
InvoiceItems.InsertOnSubmit(line1);
InvoiceItem line2 = new InvoiceItem()
{
	InvoiceItemId = Guid.NewGuid(),
	InvoiceId = invoice1.InvoiceId,
	ProductName = "SMS Unit",
	LineNumber = 2,
	Quantity = 1000,
	UnitPriceExcludingVat = 0.25M,
	UnitPriceVat = 0.04M,
	UnitPriceIncludingVat = 570,
	TotalLineItemPriceExcludingVat = 250,
	TotalLineItemPriceVat = 35,
	TotalLineItemPriceIncludingVat = 285,
	DateCreated = DateTime.Now
};
InvoiceItems.InsertOnSubmit(line2);

SubmitChanges();

Invoices.Dump();
InvoiceItems.Dump();