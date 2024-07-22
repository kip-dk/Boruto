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
	/// Kalendertype, f.eks. en kalender for brugerarbejdstimer eller en kalender for kundeservicetimer.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public enum calendar_type
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Standard = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Kundeservice = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Helligdagsplan = 2,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Indrekalendertype = -1,
	}
	
	/// <summary>
	/// En kalender, der bruges i planlægningssystemet til at definere, hvornår en aftale eller en aktivitet finder sted.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("calendar")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class Calendar : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the calendar entity
		/// </summary>
		public partial class Fields
		{
			public const string BusinessUnitId = "businessunitid";
			public const string BusinessUnitIdName = "businessunitidname";
			public const string CalendarId = "calendarid";
			public const string Id = "calendarid";
			public const string CreatedBy = "createdby";
			public const string CreatedByName = "createdbyname";
			public const string CreatedByYomiName = "createdbyyominame";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string CreatedOnBehalfByName = "createdonbehalfbyname";
			public const string CreatedOnBehalfByYomiName = "createdonbehalfbyyominame";
			public const string Description = "description";
			public const string HolidayScheduleCalendarId = "holidayschedulecalendarid";
			public const string HolidayScheduleCalendarIdName = "holidayschedulecalendaridname";
			public const string IsShared = "isshared";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedByName = "modifiedbyname";
			public const string ModifiedByYomiName = "modifiedbyyominame";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ModifiedOnBehalfByName = "modifiedonbehalfbyname";
			public const string ModifiedOnBehalfByYomiName = "modifiedonbehalfbyyominame";
			public const string Name = "name";
			public const string OrganizationId = "organizationid";
			public const string OrganizationIdName = "organizationidname";
			public const string PrimaryUserId = "primaryuserid";
			public const string Type = "type";
			public const string TypeName = "typename";
			public const string VersionNumber = "versionnumber";
			public const string BusinessUnit_Calendar = "BusinessUnit_Calendar";
			public const string CalendarRules = "calendarrules";
			public const string Referencedcalendar_customercalendar_holidaycalendar = "Referencedcalendar_customercalendar_holidaycalendar";
			public const string calendar_system_users = "calendar_system_users";
			public const string business_unit_calendars = "business_unit_calendars";
			public const string Referencingcalendar_customercalendar_holidaycalendar = "calendar_customercalendar_holidaycalendar";
			public const string lk_calendar_createdby = "lk_calendar_createdby";
			public const string lk_calendar_createdonbehalfby = "lk_calendar_createdonbehalfby";
			public const string lk_calendar_modifiedby = "lk_calendar_modifiedby";
			public const string lk_calendar_modifiedonbehalfby = "lk_calendar_modifiedonbehalfby";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Calendar() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "calendar";
		
		public const string EntityLogicalCollectionName = "calendars";
		
		public const string EntitySetName = "calendars";
		
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
		/// Entydigt id for den afdeling, som kalenderen er tilknyttet.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitid")]
		public Microsoft.Xrm.Sdk.EntityReference BusinessUnitId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("businessunitid");
			}
			set
			{
				this.OnPropertyChanging("BusinessUnitId");
				this.SetAttributeValue("businessunitid", value);
				this.OnPropertyChanged("BusinessUnitId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitidname")]
		public string BusinessUnitIdName
		{
			get
			{
				if (this.FormattedValues.Contains("businessunitid"))
				{
					return this.FormattedValues["businessunitid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Entydigt id for kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("calendarid")]
		public System.Nullable<System.Guid> CalendarId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("calendarid");
			}
			set
			{
				this.OnPropertyChanging("CalendarId");
				this.SetAttributeValue("calendarid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("CalendarId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("calendarid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.CalendarId = value;
			}
		}
		
		/// <summary>
		/// Entydigt id for den bruger, der oprettede kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyname")]
		public string CreatedByName
		{
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyyominame")]
		public string CreatedByYomiName
		{
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Dato og klokkeslæt for oprettelse af kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
		}
		
		/// <summary>
		/// Entydigt id for den stedfortrædende bruger, der oprettede kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyname")]
		public string CreatedOnBehalfByName
		{
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyyominame")]
		public string CreatedOnBehalfByYomiName
		{
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// En kalender, der bruges i planlægningssystemet til at definere, hvornår en aftale eller en aktivitet finder sted.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("description")]
		public string Description
		{
			get
			{
				return this.GetAttributeValue<string>("description");
			}
			set
			{
				this.OnPropertyChanging("Description");
				this.SetAttributeValue("description", value);
				this.OnPropertyChanged("Description");
			}
		}
		
		/// <summary>
		/// Kalender-id for helligdagsplan
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("holidayschedulecalendarid")]
		public Microsoft.Xrm.Sdk.EntityReference HolidayScheduleCalendarId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("holidayschedulecalendarid");
			}
			set
			{
				this.OnPropertyChanging("HolidayScheduleCalendarId");
				this.SetAttributeValue("holidayschedulecalendarid", value);
				this.OnPropertyChanged("HolidayScheduleCalendarId");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("holidayschedulecalendaridname")]
		public string HolidayScheduleCalendarIdName
		{
			get
			{
				if (this.FormattedValues.Contains("holidayschedulecalendarid"))
				{
					return this.FormattedValues["holidayschedulecalendarid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Kalenderen deles af andre kalendere, f.eks. organisationskalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isshared")]
		public System.Nullable<bool> IsShared
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("isshared");
			}
			set
			{
				this.OnPropertyChanging("IsShared");
				this.SetAttributeValue("isshared", value);
				this.OnPropertyChanged("IsShared");
			}
		}
		
		/// <summary>
		/// Entydigt id for den bruger, der sidst ændrede kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyname")]
		public string ModifiedByName
		{
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyyominame")]
		public string ModifiedByYomiName
		{
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Dato og klokkeslæt for den seneste ændring af kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}
		
		/// <summary>
		/// Entydigt id for den stedfortrædende bruger, der senest ændrede kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyname")]
		public string ModifiedOnBehalfByName
		{
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyyominame")]
		public string ModifiedOnBehalfByYomiName
		{
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Navnet på kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("name")]
		public string Name
		{
			get
			{
				return this.GetAttributeValue<string>("name");
			}
			set
			{
				this.OnPropertyChanging("Name");
				this.SetAttributeValue("name", value);
				this.OnPropertyChanged("Name");
			}
		}
		
		/// <summary>
		/// Entydigt id for den organisation, som kalenderen er tilknyttet.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationid")]
		public Microsoft.Xrm.Sdk.EntityReference OrganizationId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("organizationid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationidname")]
		public string OrganizationIdName
		{
			get
			{
				if (this.FormattedValues.Contains("organizationid"))
				{
					return this.FormattedValues["organizationid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Entydigt id for den primære bruger af kalenderen.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("primaryuserid")]
		public System.Nullable<System.Guid> PrimaryUserId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("primaryuserid");
			}
			set
			{
				this.OnPropertyChanging("PrimaryUserId");
				this.SetAttributeValue("primaryuserid", value);
				this.OnPropertyChanged("PrimaryUserId");
			}
		}
		
		/// <summary>
		/// Kalendertype, f.eks. en kalender for brugerarbejdstimer eller en kalender for kundeservicetimer.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("type")]
		public virtual calendar_type? Type
		{
			get
			{
				return ((calendar_type?)(EntityOptionSetEnum.GetEnum(this, "type")));
			}
			set
			{
				this.OnPropertyChanging("Type");
				this.SetAttributeValue("type", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
				this.OnPropertyChanged("Type");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("typename")]
		public string TypeName
		{
			get
			{
				if (this.FormattedValues.Contains("type"))
				{
					return this.FormattedValues["type"];
				}
				else
				{
					return default(string);
				}
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
		/// 1:N BusinessUnit_Calendar
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("BusinessUnit_Calendar")]
		public System.Collections.Generic.IEnumerable<Boruto.Plugin.Entities.BusinessUnit> BusinessUnit_Calendar
		{
			get
			{
				return this.GetRelatedEntities<Boruto.Plugin.Entities.BusinessUnit>("BusinessUnit_Calendar", null);
			}
			set
			{
				this.OnPropertyChanging("BusinessUnit_Calendar");
				this.SetRelatedEntities<Boruto.Plugin.Entities.BusinessUnit>("BusinessUnit_Calendar", null, value);
				this.OnPropertyChanged("BusinessUnit_Calendar");
			}
		}
		
		/// <summary>
		/// 1:N calendar_calendar_rules
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("calendarrules")]
		public System.Collections.Generic.IEnumerable<Boruto.Plugin.Entities.CalendarRule> CalendarRules
		{
			get
			{
				Microsoft.Xrm.Sdk.EntityCollection collection = this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityCollection>("calendarrules");
				if (((collection != null) 
							&& (collection.Entities != null)))
				{
					return System.Linq.Enumerable.Cast<Boruto.Plugin.Entities.CalendarRule>(collection.Entities);
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.OnPropertyChanging("CalendarRules");
				if ((value == null))
				{
					this.SetAttributeValue("calendarrules", value);
				}
				else
				{
					this.SetAttributeValue("calendarrules", new Microsoft.Xrm.Sdk.EntityCollection(new System.Collections.Generic.List<Microsoft.Xrm.Sdk.Entity>(value)));
				}
				this.OnPropertyChanged("CalendarRules");
			}
		}
		
		/// <summary>
		/// 1:N calendar_customercalendar_holidaycalendar
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("calendar_customercalendar_holidaycalendar", Microsoft.Xrm.Sdk.EntityRole.Referenced)]
		public System.Collections.Generic.IEnumerable<Boruto.Plugin.Entities.Calendar> Referencedcalendar_customercalendar_holidaycalendar
		{
			get
			{
				return this.GetRelatedEntities<Boruto.Plugin.Entities.Calendar>("calendar_customercalendar_holidaycalendar", Microsoft.Xrm.Sdk.EntityRole.Referenced);
			}
			set
			{
				this.OnPropertyChanging("Referencedcalendar_customercalendar_holidaycalendar");
				this.SetRelatedEntities<Boruto.Plugin.Entities.Calendar>("calendar_customercalendar_holidaycalendar", Microsoft.Xrm.Sdk.EntityRole.Referenced, value);
				this.OnPropertyChanged("Referencedcalendar_customercalendar_holidaycalendar");
			}
		}
		
		/// <summary>
		/// 1:N calendar_system_users
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("calendar_system_users")]
		public System.Collections.Generic.IEnumerable<Boruto.Plugin.Entities.SystemUser> calendar_system_users
		{
			get
			{
				return this.GetRelatedEntities<Boruto.Plugin.Entities.SystemUser>("calendar_system_users", null);
			}
			set
			{
				this.OnPropertyChanging("calendar_system_users");
				this.SetRelatedEntities<Boruto.Plugin.Entities.SystemUser>("calendar_system_users", null, value);
				this.OnPropertyChanged("calendar_system_users");
			}
		}
		
		/// <summary>
		/// N:1 business_unit_calendars
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("business_unit_calendars")]
		public Boruto.Plugin.Entities.BusinessUnit business_unit_calendars
		{
			get
			{
				return this.GetRelatedEntity<Boruto.Plugin.Entities.BusinessUnit>("business_unit_calendars", null);
			}
			set
			{
				this.OnPropertyChanging("business_unit_calendars");
				this.SetRelatedEntity<Boruto.Plugin.Entities.BusinessUnit>("business_unit_calendars", null, value);
				this.OnPropertyChanged("business_unit_calendars");
			}
		}
		
		/// <summary>
		/// N:1 calendar_customercalendar_holidaycalendar
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("holidayschedulecalendarid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("calendar_customercalendar_holidaycalendar", Microsoft.Xrm.Sdk.EntityRole.Referencing)]
		public Boruto.Plugin.Entities.Calendar Referencingcalendar_customercalendar_holidaycalendar
		{
			get
			{
				return this.GetRelatedEntity<Boruto.Plugin.Entities.Calendar>("calendar_customercalendar_holidaycalendar", Microsoft.Xrm.Sdk.EntityRole.Referencing);
			}
			set
			{
				this.OnPropertyChanging("Referencingcalendar_customercalendar_holidaycalendar");
				this.SetRelatedEntity<Boruto.Plugin.Entities.Calendar>("calendar_customercalendar_holidaycalendar", Microsoft.Xrm.Sdk.EntityRole.Referencing, value);
				this.OnPropertyChanged("Referencingcalendar_customercalendar_holidaycalendar");
			}
		}
		
		/// <summary>
		/// N:1 lk_calendar_createdby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_calendar_createdby")]
		public Boruto.Plugin.Entities.SystemUser lk_calendar_createdby
		{
			get
			{
				return this.GetRelatedEntity<Boruto.Plugin.Entities.SystemUser>("lk_calendar_createdby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_calendar_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_calendar_createdonbehalfby")]
		public Boruto.Plugin.Entities.SystemUser lk_calendar_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Boruto.Plugin.Entities.SystemUser>("lk_calendar_createdonbehalfby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_calendar_modifiedby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_calendar_modifiedby")]
		public Boruto.Plugin.Entities.SystemUser lk_calendar_modifiedby
		{
			get
			{
				return this.GetRelatedEntity<Boruto.Plugin.Entities.SystemUser>("lk_calendar_modifiedby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_calendar_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_calendar_modifiedonbehalfby")]
		public Boruto.Plugin.Entities.SystemUser lk_calendar_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Boruto.Plugin.Entities.SystemUser>("lk_calendar_modifiedonbehalfby", null);
			}
		}
	}
}
#pragma warning restore CS1591
