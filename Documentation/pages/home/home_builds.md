---
title: Automated builds
summary: The QuantSA automated builds and release process.
keywords: 
last_updated: January 10, 2018
tags: developers
sidebar: home_sidebar
permalink: home_builds.html
folder: home
---

# Overview

QuantSA has some level of continuous integration.  This currently run in two different places:

1.   <https://travis-ci.org/JamesLTaylor/QuantSA>
2.   <https://jameslatimertaylor.visualstudio.com/QuantSA/_build>

Both of which need private log on details and can't be changed by other developers.  Please contact [me](mailto:James@cogn.co.za) if you would like to be involved in the building process.

## Travis

 * Build the markdown docs using Jekyll
 * Checks that there are no dead links
 * Upload all output to <http://www.quantsa.org/latest>

The Travis build script includes the FTP password which obviously needs to be hidden, see <https://docs.travis-ci.com/user/encryption-keys/> for how to do this. Once everything is set up you need to run:

`travis encrypt FTP_PWD=xxxxxxxx --add`
 
Setting up the travis build was easier than installing Ruby into Visual Studio Team Services.

## Visual Studio

Core library:

* Build QuantSACore.sln
* Run tests

Excel parts:

* Build QuantSACommon.sln
* Build GenerateXLCode.sln
* Build QuantSAExcelAddin.sln (includes excel code generation via GenerateXLCode.exe)
* Zip the dlls (only required because the visual studio `AfterBuild` task does not appear to run properly, see [issue #34](https://github.com/JamesLTaylor/QuantSA/issues/34) )
* Build QuantSAInstaller.sln
* Zip the whole addin directory
* Upload to Azure the addin directory as `QuantSA_build_latest.zip` and `QuantSA_build_[BuildID].zip`.
* Upload to Azure the QuantSA Installer as `QuantSAInstaller_Latest.exe` and `QuantSAInstaller_[BuildID].zip`
 

## Deploy

The deploy steps are manual, for a version X.Y.Z:

* Copy <http://www.QuantSA.org/latest> to <http://www.QuantSA.org/[vX.Y.Z]> and <http://www.QuantSA.org>
  * This can only be done by developers who have an FTP account and password on QuantSA.org
  * Note that with FileZilla you cannot copy files between folders on the server.  They need to be downloaded then uploaded to a new location.
* Make a github release
* Copy latest zip and installer to github release, rename to include version.
* Add link to help menu in github release.
* Add help links to the index page on QuantSA.org.
  * edit `\Documentation\documentation_history.html` and ftp it to the root folder on the QuantSA.org server.



## Further work

* None of the excel functionality is currently tested.  This needs to be added somewhere rather than relying on developer's good behaviour. (The developer can check their own behaviour by using the [PrepareRelease](home_projects.html#preparereleasesln) solution.)
* It would be nice to change the QuantSACore solution projects to be able to run on mono and hence on the travis build.  This could be useful in the future for distributing calculations.  
* Should add python testing to one of the builds.

