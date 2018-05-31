namespace Figlut.Spread.ORM.Helpers
{
    public enum UserRole
    {
        None = 0,
        Subscriber = 1,
        OrganizationUser = 2,
        OrganizationAdmin = 4,
        AccountManager = 8,
        Administrator = 15
    }
}