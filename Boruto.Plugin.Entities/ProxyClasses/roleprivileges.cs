#pragma warning disable CS1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Boruto.Plugin.Entities
{
	
	
	/// <summary>
	/// En gruppe rettigheder, der bruges til at kategorisere brugere for at give dem adgang til objekter.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("roleprivileges")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class RolePrivileges : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the roleprivileges entity
		/// </summary>
		public partial class Fields
		{
			public const string CanBeDeleted = "canbedeleted";
			public const string ComponentState = "componentstate";
			public const string IsManaged = "ismanaged";
			public const string IsManagedName = "ismanagedname";
			public const string OverwriteTime = "overwritetime";
			public const string PrivilegeDepthMask = "privilegedepthmask";
			public const string PrivilegeId = "privilegeid";
			public const string RecordFilterId = "recordfilterid";
			public const string RecordFilterIdName = "recordfilteridname";
			public const string RoleId = "roleid";
			public const string RolePrivilegeId = "roleprivilegeid";
			public const string Id = "roleprivilegeid";
			public const string RolePrivilegeIdUnique = "roleprivilegeidunique";
			public const string SolutionId = "solutionid";
			public const string VersionNumber = "versionnumber";
			public const string roleprivileges_association = "roleprivileges_association";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public RolePrivileges() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "roleprivileges";
		
		public const string EntityLogicalCollectionName = null;
		
		public const string EntitySetName = "roleprivilegescollection";
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		private void OnPropertyChanged(string propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void OnPropertyChanging(string propertyName)
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
			}
		}
		
		/// <summary>
		/// Angiver, om rollerettigheden kan slettes.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("canbedeleted")]
		public Microsoft.Xrm.Sdk.BooleanManagedProperty CanBeDeleted
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.BooleanManagedProperty>("canbedeleted");
			}
			set
			{
				this.OnPropertyChanging("CanBeDeleted");
				this.SetAttributeValue("canbedeleted", value);
				this.OnPropertyChanged("CanBeDeleted");
			}
		}
		
		/// <summary>
		/// Kun til intern brug.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("componentstate")]
		public virtual componentstate? ComponentState
		{
			get
			{
				return ((componentstate?)(EntityOptionSetEnum.GetEnum(this, "componentstate")));
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ismanaged")]
		public System.Nullable<bool> IsManaged
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ismanaged");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ismanagedname")]
		public string IsManagedName
		{
			get
			{
				if (this.FormattedValues.Contains("ismanaged"))
				{
					return this.FormattedValues["ismanaged"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Kun til intern brug.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overwritetime")]
		public System.Nullable<System.DateTime> OverwriteTime
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overwritetime");
			}
		}
		
		/// <summary>
		/// Systemoprettet attribut, der gemmer de relationer, der er tilknyttet rollen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("privilegedepthmask")]
		public System.Nullable<int> PrivilegeDepthMask
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("privilegedepthmask");
			}
			set
			{
				this.OnPropertyChanging("PrivilegeDepthMask");
				this.SetAttributeValue("privilegedepthmask", value);
				this.OnPropertyChanged("PrivilegeDepthMask");
			}
		}
		
		/// <summary>
		/// Entydigt id for den rettighed, der er tilknyttet rollen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("privilegeid")]
		public System.Nullable<System.Guid> PrivilegeId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("privilegeid");
			}
		}
		
		/// <summary>
		/// Entydigt id for postfilter, der er tilknyttet rollerettigheden.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("recordfilterid")]
		public Microsoft.Xrm.Sdk.EntityReference RecordFilterId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("recordfilterid");
			}
			set
			{
				this.OnPropertyChanging("RecordFilterId");
				this.SetAttributeValue("recordfilterid", value);
				this.OnPropertyChanged("RecordFilterId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("recordfilteridname")]
		public string RecordFilterIdName
		{
			get
			{
				if (this.FormattedValues.Contains("recordfilterid"))
				{
					return this.FormattedValues["recordfilterid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Entydigt id for den rolle, der er tilknyttet rollerettigheden.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("roleid")]
		public System.Nullable<System.Guid> RoleId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("roleid");
			}
		}
		
		/// <summary>
		/// Entydigt id for rollerettigheden.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("roleprivilegeid")]
		public System.Nullable<System.Guid> RolePrivilegeId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("roleprivilegeid");
			}
			set
			{
				this.OnPropertyChanging("RolePrivilegeId");
				this.SetAttributeValue("roleprivilegeid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("RolePrivilegeId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("roleprivilegeid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.RolePrivilegeId = value;
			}
		}
		
		/// <summary>
		/// Kun til intern brug.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("roleprivilegeidunique")]
		public System.Nullable<System.Guid> RolePrivilegeIdUnique
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("roleprivilegeidunique");
			}
		}
		
		/// <summary>
		/// Entydigt id for den tilknyttede løsning.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
		public System.Nullable<System.Guid> SolutionId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("solutionid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
		
		/// <summary>
		/// N:N roleprivileges_association
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("roleprivileges_association")]
		public System.Collections.Generic.IEnumerable<Boruto.Plugin.Entities.Privilege> roleprivileges_association
		{
			get
			{
				return this.GetRelatedEntities<Boruto.Plugin.Entities.Privilege>("roleprivileges_association", null);
			}
			set
			{
				this.OnPropertyChanging("roleprivileges_association");
				this.SetRelatedEntities<Boruto.Plugin.Entities.Privilege>("roleprivileges_association", null, value);
				this.OnPropertyChanged("roleprivileges_association");
			}
		}
	}
}
#pragma warning restore CS1591
