namespace Boruto.Plugin.Entities
{
	public partial class CrmUnitOfWork
	{
		public IRepository<Account> Accounts => GetRepository<Account>();
		public IRepository<bor_plugindemo> Plugindemos => GetRepository<bor_plugindemo>();
		public IRepository<bor_demoviews> Demoviewss => GetRepository<bor_demoviews>();
		public IRepository<SystemUser> SystemUsers => GetRepository<SystemUser>();
		public IRepository<Contact> Contacts => GetRepository<Contact>();
	}
}
