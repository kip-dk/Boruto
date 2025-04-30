# Boruto.Dataverse.Plugin

## updated: 2025-04-27 08:40

1. Build plugin for dataverse using SOLID software princips
1. Keep you plugin registration in dataverse in sync. with your code, using a simple plugin self-deployment solution

For documentation:
[Please go to the official boruto dataverse project documentation page to see documentation](https://kipon.dk/borutoplugin)


## DISCLAIMER
This software is provided as-is, and is used on your own risk. Kipon ApS does not take any responsibility of damages directly or indirectly caused by the
use of this software.

## LICENSE
The software is licensed under MIT, and can be used and distributed for free. It is prohibited to sell the software under any circumstances. 

## Boruto

## History:  Boruto <- Kipon.Solid.Plugin
This library is a migration of the Kipon.Solid.Plugin framework, with the purpose of support builing Plugins for the dataverse platform using the software princip SOLID.

The Kipon.Solid.Plugin and the Boruto plugin frameworks has a lot a similarities, however also have important differences in the following areas:

1. It is using the nuget plugin deployment model
1. It is using pac model build to create strongly typed entities
1. Only online environments are supported.
1. No support for workflows (the nuget deployment model do not support it)
1. Less restrictive in terms of always using interfaces for injection. 

## Changelog

### 1.0.1.1 2025-04-30: Boruto.BasePlugin.ServiceProvider change from a property to a method, taking Boruto.ServiceAPI.IServiceContext as argument to allow custom service provider to use Dataverse service directly or, provided by the Boruto platform

### 1.0.0.4 2025-04-30: Plugin tool is not using early bound entities anymore, to avoid conlict with other libraries

### 1.0.0.3 2025-04-27: Added extensions methods from Kipon.Solid

### 1.0.0.2 2025-04-26: Initial release

## Copyright
(c) Boruto.Dataverse.Plugin: Copyright Kipon ApS 2022-2025