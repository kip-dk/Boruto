# Boruto.Dataverse.Plugin

## updated: 2026-03-14 09:50

1. Build plugin for dataverse using SOLID software princips
1. Keep you plugin registration in dataverse in sync. with your code, using a simple plugin self-deployment solution


## DISCLAIMER
This software is provided as-is, and is used on your own risk. Kipon ApS does not take any responsibility of damages directly or indirectly caused by the
use of this software.

## LICENSE
The software is licensed under MIT, and can be used and distributed for free. It is prohibited to sell the software under any circumstances. 
The software can however be include in any commercial or non-commercial solution and redistrubuted without any restrictions. 

## Boruto
Boruto.Dataverse.Plugin is mostly a design pattern, created with the purpose of building Dataverse plugin extensions using the software princip SOLID.
This nuget library provides the interfaces and BasePlugin implementation to build simple event driven plugins. It also provides a simple yet powerfull dependency injection implementation that 
allow you to inject services directly into your plugin event hooks.

## Plugin Registration
The Boruto.Dataverse.Plugin library is deployment aware in the sense, that it is able to deploy it self to a Dataverse environment base on the code writting on top of the library, following the design patterns described on this page.
You can see below how to automate that process.

### Boruto basic design pattern
The very basic nature of a Dataverse.Plugn looks like this

Paylaod -> Message and Stage -> Execute Code -> Respond

All Dataverse plugins are basically dataoperation event handlers. A dataverse plugin allows you to hook into all data related operatons such as Create, Update ,Delete ,Retreive, RetreiveMultiple but not limit to this.
Some operations in Dataverse, such as creating an M:M relation between two entity-records is done using a specialized organization request. You can hook into these organizationsrequest as well, using Dataverse Plugins.
Finally you can define you own organizations request, using Dataverse Process Handler concept, and you can implement code behind these handlers with plugin. These are in fact also just organization requests.


### In general
**Be aware of the 2-minutes limit. Performance is crucial. Do not do things that takes more time, and think performance in everything you do, otherwise you will
end up with a system with bad performance and long running request.**

#### Payload
A plugin is always executed by the plaform with a Payload. This is basically the input to the plugin represented by:

* **Target**,  the target data for the operation 
	* Create,Update => Entity
	* Delete => EntityReference 
	* Retrieve => EntityId and EntityLogicalName
	* RetrieveMultiple => EntityLogicalName and Query
	* OrganizationRequest
* **Pre**, information about the target as it looks before this operation
	* Relevant for: Update,Delete
* **Post**, information about the target as it looks after this operation
	* Relevant for: Create,Update		

A plugin is actually hooking directly into a transactional pipeline of an operation, and when building a plugin, you deside in witch Stage in the pipeline you wish to run your code:


#### Stages
* Validate
* Pre
* Post
* Post Async

##### Validate
Registre steps in this stage for validation only. This stage is running in its own transaction. If you do a create/update/delete operation directly in this stage, and the operation failes in a later step. 
These operation will NOT be rolledback. So be carefull. On top, validatestep is not always called as expected. Ex. if you registre a plugin on delete event in validate stage, the code will NOT be executed, if the delete is caused by a delete cascade operation.

##### Pre
Registre steps in this stage to manipulte the target before it hits the database, ex. calculated fields or similar, that cannot be accomplised with Fx or Aggrigation fields. This step is executed BEFORE the operation hits the database. This allow you to put more info into the
target payload. Pre stage is running as part of the primary dataverse transaction, and if anything failes everything fails. 

##### Post
Registre steps in this stage to perform transactional derivative efforts. You will typically use this stage to perform operations on related entities, that needes to be done whenever something is happening on the target record.

##### Post Async
Registre steps in stage Post Async than does not need to be done transactional. This will give the user a better experience, because they do not have to sit an wait for things to be done, that can be done in background.  Before you put things into Async you should carefully confirm that
your derivative efforts does not need to be transactional, and you should carefully consider how you hanle errors. An Async plugin is executed in its own transaction. The main transaction, triggering the code is commited, but if the Async operation fails, whatever that plugin needed to do
might not be done at all, because the async operation failed. You can monitor Async operation in the Dataverse System job list, but it is not always possible to restart an async operation. Also be aware, that even the async plugins are running under the 2-minutes limit. 

### The Boruto Design Pattern
This section will describe the import design patterns that should be used, to ensure a SOLID approach for plugin development, and allow automation of the deployment process.

##### Payload
In most cases a payload is an enttiy represented by an instance of Microsoft.Xrm.Sdk.Entity. This goes for Target in Create/Update operations as well as Pre/Post images in Create/Update/Delete operations. You can access information in such instance using the property logical name on form

```C#
entity["fieldname"] = "a new value for fieldname";
``` 

This approach is generic but also very troublesome and error-prone, and your C# code IDE does not provide much help. For that reason you will typically use a tool to generate early bound entities.  Payloads in boruto is typically defined by interfaces, implemented by early bound entities.

###### Early bound entities

An early bound entity is a class that extends Microsoft.Xrm.Sdk.Entity, typically generated by a tool. Such typed entity allow you to access the properties of the entity in a more natural way;

```C#
entity.Fieldname = "a new value for fieldname";
``` 

You can use whatever tool you like, to generate early bound entities, but we strongly recommend that you use the standard tool provided by Microsoft, pac model build. You can find the tool and the documentation here:

[Microsoft pac model builder](https://learn.microsoft.com/en-us/power-platform/developer/cli/reference/modelbuilder)

The early bound entity tool generator must decorate entity properties with the [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute] to work with the Boruto solution. Pac model build does so, and crmsvcutil.exe also makes this decoration. We strongly recomend that you do NOT use the later (crmsvcutil.exe).

The library hosting your datamodel does not need to be the same as the library where you write your plugin code. If you choose to separate the libraries, remember to bootstrap both the entity library and your final plugin library, using the pac plugin init tool. This ensure
compatibility with the Dataverse Nuget package deployment model where you create a dependency from your plugin library to your entity library. You can also choose to keep both in same library.

##### Early bound entities as payload.
You can use the early bound entities as the payload in your plugin methods directly, simply by using a naming convention on the parameter. You should however be aware that it is considered bad pratcise to listen on all properties on a target entity for the Update event, and it is also considered bad
pratcise to request all properties in a Pre og Post images for any event. It is supported, both by the boruto patterns and by the underlying Dataverse plugin infrastructure by Microsoft, but it is a bad idear that might cause bad performance.

#### Define payloads as interfaces
The Boruto platform allow you to describe payloads by interfaces. You create an interface, extending one of the four payload interfaces of boruto, and then you make sure that each early bound entity that should support your operation, implement that interface.

Boruto define four basic payload interfaces:
* Boruto.ITarget
* Boruto.IPreimage
* Boruto.IMerged
* Boruto.Postimage

The naming of the interface should give sufficient hint on what they represent, Boruto.IMerged as the exception.

A payload interface extends one of above, depending on the requirement, and typically have a number of properties, corresponding to the same properties of the implementing early bound entity. This way, the payload interface describe the exact part of the early bound entity that is needed for
a specialized usecase.

##### Boruto.IMerged
This interface represent a combination of a Target and a Preimage. The entity will first be populated with information from the preimage, and after that overridden with all values of the target. The end result we be an enttiy record that has values from the target, if such value is include, otherwise it has the value from the preimage.
This is convinient if you need to do operations where one or more fields can be in the target, while even more fields are needed to have the full needed picture. In validate stage and Pre stage, any change to a writable property in the merged image will drain into the target, and become a part of the target payload. This allow you to
have setter methods on such image, and ensure that any operation, setting a value will also go back to the database. Remember this is only the case in Validate and Pre state. It does not make sence to update target properties in post or post async, because the value will not go into the database anyway.


##### Payload example

```C#
/// Payload interface
public interface INameChanged : Boruto.ITarget 
{
	string Name { get; }
}

/// Early bound entity, implementing payload interface
public partial class Account : INameChanged 
{
}

/// Plugin code
public class AccountPlugin : BasePlugin 
{
	public void OnPreUpdate(INameChanged target) 
	{
		// do something with the name;
	}
}
```

The above example demonstrate the basic use of payloads in a plugin. 

* First of all, you define an interface that describes the "situation",  INameChanged. This interface has a single property Name, and it expose the getter method of the property. What we are waiting for is basically in this case that someone change the name property of the implementing entity.
* Secondly we let our early bound entity for account implement the interface. This is simple, just state that account is implementing, because the code generator already created a property "Name" for the account.
* Finally we can write a plugin, that listen for an event (Pre Update), and in the parameters, ask for the payload interface.

The above example is a "cut-to-the-bone" example of what the Boruto dataverse design pattern is about. 

* Describe the event you need to handle by an inteface
* Implement that interface for one or more relevant early bound entities, that can trigger the situation.
* Inject the payload into a plugin method, that captures the payload, and perform whatever is needed to be done.


#### BasePlugin
the boruto platform provides a base plugin (Boruto.BasePlugin). It is defined abstract, and your plugins must be an extension of this plugin. You should however not extend it directly in each of your plugins. Instead, within your own plugin library, you should create you own BasePlugin that extends the Boruto.BasePlugin, and implements the very basic
abstract methods of that class:

```C#
using System;
using System.Reflection;

namespace My.Plugins
{
    public abstract class MyBasePlugin : Boruto.BasePlugin
    {
        private static readonly Assembly[] assms = new Assembly[]
        {
            typeof(My.Entities.Account).Assembly,
            typeof(My.Plugins.BasePlugin).Assembly
        };

        public MyBasePlugin() : base()
        {
        }

        public MyBasePlugin(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override IServiceProvider ServiceProvider(Boruto.ServiceAPI.IServiceContext ctx) 
        {
            return null;
        }

        protected override Assembly[] ServiceAssemblies => assms;
        protected override bool FilterTargetOnCreate => false;
    }
}

```
The above example is a base plugin.

* **Constructors**, you only need to add thouse that you need. If you do not plan to use plugin settings, you can ommit the constructor taking unsecure and secure string
* **ServiceProvider**. If you have classed that cannot be resolved by the Boruto class resolver mechanism, you can return a service provider that can be used to resolve such type
* **ServiceAssemblies**. You must return an array of assemblies, where the boruto plugin should look for service implementations. 
* **FilterTargetOnCreate** this setting is default true. This means that if you inject a target interface on a create event, and none of the properties in the interface is within a create target payload, such method will not be called. Setting this property to false, filtering on target attributes on created event is ignored.

#### Plugin
```C#
namespace My.Plugins
{
    public class AccountPlugin : MyBasePlugin
    {
        public void OnPreCreate(Entities.INameChanged target) 
        {
            this.NameHandler(target);
        }

        public void OnPreUpdate(Entities.INameChanged target) 
        {
            this.NameHandler(target);
        }

        private void NameHandler(Entities.INameChanged target) 
        {
            // do something, related to name change event
        }
    }
}

```
Finally we created a plugin that listen to the defined payload "INameChanged" for the pre create and pre update event.

#### Services
The above pattern is of little use, if you only have payloads to work with. Therefore the Boruto plugin support injecting services directly into you plugin methods

```C#
namespace My.Plugins
{
    public class AccountPlugin : MyBasePlugin
    {
        public void OnPreCreate(Entities.INameChanged target, .... any number of relevant services) 
        {
            this.NameHandler(target);
        }

        public void OnPreUpdate(Entities.INameChanged target, ... any number fo relevant services) 
        {
            this.NameHandler(target);
        }

        private void NameHandler(Entities.INameChanged target, ...) 
        {
            // do something, related to name change event
        }
    }
}
```

##### Microsoft Standard Services
You can inject microsoft standard services. Some of these services can be quite hard to mock for unit-test, so consider twise before you spred injection of thouse in too many places.
The list of supported standard services that can be injected:

* Microsoft.Xrm.Sdk.ITraceService
* Microsoft.Xrm.Sdk.IOrganizationService (be carefull of overusing this. There are better ways to inject CRUD cababilities in your plugin than using this service directly)
* Microsoft.Xrm.Sdk.IPluginExecutionContext (be carefull of overusing this, it is hard to mock for unit-test)
* Microsoft.Xrm.Sdk.IServiceEndpointNotificationService (we have never used it)
* System.IServiceProvider (you do not need to acces this. Boruto allow you to inject directly whatever you can get out of this, but it provides the standard service provider for the plugin)
* IOrganizationServiceFactory (you do not need to access this, you can inject the IOrganizationService directly, and add the [Admin]IOrganizationService flag if you need to run with priviliges
* System.Linq.IQueryable<EarlyBoundEntityType>, this allow you to inject an object where you can query entities of type "EarlyBoundEntityType". This is the preferred approach to allow query data, over injecting IOrganizationService. If you need to query with priviliges, add the [Admin] decorator the to parameter.

On top of the standard interfaces provided by the Microsoft platform, Boruto is adding a few services to the plate

* Boruto.IRepository<EarlyBoundEntityType>, this allow you to do CRUD and query operation on the "EarlyBoundEntityType". This is the preferred approach to allow CRUD and query operation on an entity over injecting IOrganisationService. If you need to perform operation with priviliges, add the [Admin] decorator to the parameter
* Boruto.ServiceAPI.IMetadataService, a simple service that allow you to work with Dataverse Entity metadata in an efficient way within your plugin.
* Boruto.ServiceAPI.INamingService, a simple service that allow you to get the name of a related entity in an efficient way.

Finally, you can create you own services, and inject these as well. For the boruto plugin to be able to resolve such service, both the interface and the service must be defined within one of the assemblies returned by the "ServiceAssemblies" method in your BasePlugin.

It is best pratcise to define an interface and inject that into plugin methods or other services, (SOL**ID**) ... all dependencies should be based on abstractions ... but while we encourage such pattern, you do not have to follow the rule. It is strictly your choice. It will help you a lot when creating unit test if you stick to the rule.

Services that should be resolved by boruto must use a public constructor with needed parameters to inject dependencies. (dependency injection by constructor)

```C#
namespace My.Plugins.Services
{
    public interface IMyService 
    {
        void doNamingStuff(INameService target);
    }

    public class MyService : IMyService
    {
        private readonly System.Linq.IQueryable<Account> accountQuery;

        public MyService(System.Linq.IQueryable<Account> accountQuery) 
        {
            this.accountQuery = accountQuery;
        }

        public interface IMyService 
        {
            public void doNamingStuff(INameService target)
            {
                if (string.IsNullOrEmpty(target.Name)) 
                {
                    throw new InvalidPluginExecutionException($"Name must have a valu");
                }

                var other = (from a in accountQuery where a.Name == target.Name && a.AccountId != target.Id select a.AccountId).FirstOrDefault();
                if (other != null) 
                {
                    throw new InvalidPluginExecutionException($"We already have an account named { target.Name }");
                }
            }
        }
    }

}

namespace My.Plugins
{
    public class AccountPlugin : MyBasePlugin
    {
        public void OnPreCreate(Entities.INameChanged target, Services.IMyService service) 
        {
            service.doNamingStuff(target);
        }

        public void OnPreUpdate(Entities.INameChanged target,  Services.IMyService service) 
        {
            service.doNamingStuff(target);
        }
    }
}
```

The example is provided only to show how to create an build a service and inject something "relevant" into the service. If you need to ensure uniqueness on a name of an entity, don't use a plugin for that. Simply add an index to the column. Dataverse index are by definition unique.


### Deployment
The boruto nuget package has all the code needed to deploy the code into Dataverse. It reflects on the code, and identify the patterns describe, and that way it knows witch steps should be registred on witch entities, and what target, pre, post images is needed. Microsoft has provided a manual tool
for this operation (The pluginregistration tool), but you do not need this tool for deployment. Instead you can write a C# console app with a few lines of codes, and let that APP perform the deployment:

* Create a console application. C#, ex .NET 4.8.
* Add a nuget dependency to the app to the Boruto.Dataverse.Plguin library (remember this lib. is .NET 4.6.2 due to constrain on what can be deployed as plugin into Dataverse)
* Add a dependency to the nuget package Microsoft.PowerPlatform.Dataverse.Client
* Add a deployment json configuration file to the root of you plugin project

deploy.json
```json
{
  "solution": "MySolution",
  "plugin": {
    "name": "My.Plugins",
    "path": ".\\bin\\debug\\",
    "package": "MyPlugin.Example",
    "resolvelibraries": [
      "My.Plugins",
      "My.Entities"
    ]
  }
}
```

* Finaly add this snipper of code to your new C# main command method line tool

```C#
var configString = System.Configuration.ConfigurationManager.ConnectionStrings["XRM"].ConnectionString;
using (var instance = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(configString))
{
    var deployer = new Boruto.Deployment.Deployer(instance);
    deployer.Deploy();
}

```

Compile the program, and run it from a prompt where you are in the root folder of the plugin.


## History:  Boruto <- Kipon.Solid.Plugin
This library is a migration of the Kipon.Solid.Plugin framework, however thie Boruto framework has been simplified and streamlined to use Microsoft standard development tools where applicable.

The Kipon.Solid.Plugin and the Boruto plugin frameworks has a lot a similarities, however also have important differences in the following areas:

1. It is using the nuget plugin deployment model
1. It is using pac model build to create strongly typed entities
1. Only online environments are supported.
1. No support for workflows (the nuget deployment model do not support it)
1. Less restrictive in terms of always using interfaces for injection. 
1. No boruto specific code generation
1. No tools in the nuget package

## Changelog
### 1.0.5.2  EntityReferenceIdEqualFilter, do not return EmptyGuid if that is what it resolves to. Instead return null. It is proberly a query comming from the make UI designer. Records in modern datavere cannot have value empty guid
### 1.0.5.1  IsChildOf no longer tries to find parent Operation in ExecuteTransaction. It was unstable on unrelyable, so we have given up on support of it
### 1.0.4.10 Made IsChildOf context extension more robust
### 1.0.4.8  IOrganizationService extension to allow boruto services to be created outside the boruto dependency injection pipeline
### 1.0.4.7  DateTimeExtension EndOfDay andEndOfYear, removed 999 milliseconds, to avoid dataverse rounding up
### 1.0.4.6  added protected virtual bool TraceCalls => false;, let your plugin return true from this if you need detail on call stack on ignored call reasons
### 1.0.4.5
### 1.0.4.4  Added datetime extension helpers
### 1.0.4.3  Solved rare null pointer en plugin method resolver
### 1.0.4.2  RLEASED

## Copyright
(c) Boruto.Dataverse.Plugin: Copyright Kipon ApS 2022-2026