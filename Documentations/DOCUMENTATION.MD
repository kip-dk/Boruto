# Boruto.Dataverse.Plugin

## updated: 2025-08-17 11:15

1. Build plugin for dataverse using SOLID software princips
1. Keep you plugin registration in dataverse in sync. with your code, using a simple plugin self-deployment solution

For documentation:
[Please go to the official boruto dataverse project documentation page to see documentation](https://kipon.dk/borutoplugin)


## DISCLAIMER
We are still in process of stabalizing the concept and improving documentation. We will make an expicit RELEASED note when the concept is ready for adoption.

This software is provided as-is, and is used on your own risk. Kipon ApS does not take any responsibility of damages directly or indirectly caused by the
use of this software.

## LICENSE
The software is licensed under MIT, and can be used and distributed for free. It is prohibited to sell the software under any circumstances. 
The software can however be include in any commercial or non-commercial solution and redistrubuted without any restrictions. 

## Boruto

## History:  Boruto <- Kipon.Solid.Plugin
This library is a migration of the Kipon.Solid.Plugin framework, with the purpose of support builing Plugins for the dataverse platform using the software princip SOLID.

The Kipon.Solid.Plugin and the Boruto plugin frameworks has a lot a similarities, however also have important differences in the following areas:

1. It is using the nuget plugin deployment model
1. It is using pac model build to create strongly typed entities
1. Only online environments are supported.
1. No support for workflows (the nuget deployment model do not support it)
1. Less restrictive in terms of always using interfaces for injection. 
1. No boruto specific code generation
1. No tools in the nuget package, guides for setting up a automated deployment and step registration process will soon be added to the documentation page. (2025-07-05)

## Changelog
### 1.9.2.39 2025-08-17 Extension method to reduce an entity for update to only contain fields that was actually changed, according to a full image of same record (ReduceToUnchanged)

### 1.9.2.38 2025-08-17 OrganizationRequest output parameters of type Entity and EntityCollection are converted from stronly type entity to Microsoft.Xrm.Sdk.Entity to avoid serialization exception on strongly typed entity

### 1.9.2.37 2025-08-16 Inject Microsoft.Xrm.Sdk.Query.ColumnSet into plugin method, to allow only fetch relevant columns on virtual entities providers

### 1.9.2.36 2025-08-07 Handle no arguments in method

### 1.9.2.35 2025-08-05 Added Messages for List Member add/remove handle, to overcome wrong nameing in standard messages from the SDK

### 1.0.2.34 2025-08-05 Resolve missing SdkMessage by fetching one at a time if needed. Report missing message, and ignore step registration, instead of fail with fatal error

### 1.0.2.32-33 2025-08-01 [Admin] was not set correctly on all method parameter and service constructor injection types

### 1.0.2.31 2025-07-26 Tools to generate OptionSetValue _[SchemaName] properties, to make migration of old projects easier

### 1.0.2.30 2025-07-23 Boruto.BasePlugin.FilterTargetOnCreate, override this setting on your plugin if you wish to supress filtering on attributes for create message
default is true, and will inforce filtering on target attributes, even for Create message, while false, target attributes filtering will only be applied for update message


### 1.0.2.29 2025-07-23 ctx.IsTargetAttribute allow parsing additional fields for the check

### 1.0.2.28 2025-07-22 ParentEntityId and DirectChildOf extension method on plugin context

### 1.0.2.27 2025-07-22 ValueOf extension method on OrganizationRequest, to make it easy to extract nullable values. The generated proxy classes are not nullable.

### 1.0.2.26 2025-07-22 GetSafe added to Generics extensions

### 1.0.2.25 2025-07-22 added ZeroAsNull extension methods to typeconverters

### 1.0.2.24 2025-07-21 Extensionmethods to handle easy convert of enum types and optionsetvalues

### 1.0.2.23 2025-07-21 Extensionmethods to easy set active/inactive state with default statuscode

### 1.0.2.22 2025-07-17 new extension method namespace Ctx adding extension methods on the pluginexecutioncontext without having to inject it

### 1.0.2.19 2025-07-17 tools version only generate uow prevents dub names on servicesname generation

### 1.0.2.16 2025-07-09 added filter attributes from Kipon Solid

### 1.0.2.15 2025-07-08 extension method to set entityreference = EmptyGuid to null in entity payload

### 1.0.2.14 2025-07-08 extension method to prevent create/update action if manadatory fields are not set on create, or updated to null on update

### 1.0.2.13 2025-07-08 Added Pages etension methods to Generics extensions

### 1.0.2.12 2025-07-08 Added extension method to extract target payload of parent context

### 1.0.2.11 2025-07-05 Added parent process look via plugin context from kipon solid plugin. Use with care.Due to design on the plugin pipeline, these method do not give a 100% consistent result

### 1.0.2.10 2025-06-24: extension method to direct access to the target entity of a plugin

### 1.0.2.9 2025-06-24: Added extension method to validate if a setup of attributes is only part of target payload (IsOnlyPayload from Kipon.Solid.Plugin)

### 1.0.2.8 2025-06-20: QueryExpression extention method to find and extract a ParentLink and corresponding id of the parent.

### 1.0.2.7 2025-06-18: Microsoft.Xrm.Sdk.Client.OrganizationServiceContext all attached entities are detached between each method invocation to avoid unpredictable caching. Boruto.IPipeline can be impl. by the custom service provider,  to add similar behavior in custom impl. 

### 1.0.2.6 2025-06-13: No platform error logging if plugins is throwing invalidpluginexecutionexception.

### 1.0.2.5 2025-06-13: InvalidPluginExecutionException thrown by a plugin did not make it to the UI, result in bad error message to the user

### 1.0.2.4 2025-05-29: Added tool to generate IUnitOfWork

### 1.0.2.3 2025-05-26: Fixed to many image mising logs in tracelog

### 1.0.2.1 2025-05-24: Deployer tool is now using the boruto service for dependency injection and dependency on System.ComponentModel has been removed

### 1.0.1.22 2025.05.07  Version 1.0.1.22 is the first version considered stable. Full documentation is still pending, so a final formal release i still awaiting.

### 1.0.1.6 - 1.0.1.21 , - A significant number of bugfixes has been added.

### 1.0.1.5 2025-05-01 Services with more than one parameter in constructor, multi registration og images

### 1.0.1.1 2025-04-30: Boruto.BasePlugin.ServiceProvider change from a property to a method, taking Boruto.ServiceAPI.IServiceContext as argument to allow custom service provider to use Dataverse service directly or, provided by the Boruto platform

### 1.0.0.4 2025-04-30: Plugin tool is not using early bound entities anymore, to avoid conlict with other libraries

### 1.0.0.3 2025-04-27: Added extensions methods from Kipon.Solid

### 1.0.0.2 2025-04-26: Initial release

## Copyright
(c) Boruto.Dataverse.Plugin: Copyright Kipon ApS 2022-2025