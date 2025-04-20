# Boruto.Dataverse.Plugin

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

The Kipon.Solid.Plugin and the Boruto plugin frameworks has a lot a similarities, however also have important differences in the following important areas:

1. It is using the nuget plugin deployment model
1. It is using pac model build to create strongly typed entities
1. Only online environments are supported.
1. No support for workflows (the nuget deployment model do not support it)

## Copyright
(c) Boruto.Dataverse.Plugin: Copyright Kipon ApS 2022-2025