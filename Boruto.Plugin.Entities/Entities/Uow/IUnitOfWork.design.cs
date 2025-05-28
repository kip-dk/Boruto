namespace Boruto.Plugin.Entities
{
	public partial interface IUnitOfWork
	{
		IRepository<Account> Accounts { get; }
		IRepository<Lead> Leads { get; }
		IRepository<bor_plugindemo> Plugindemos { get; }
		IRepository<bor_demoviews> Demoviewss { get; }
		IRepository<SystemUser> SystemUsers { get; }
		IRepository<Contact> Contacts { get; }
	}
}
