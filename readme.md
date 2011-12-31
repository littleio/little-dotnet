# little.io .NET Library
This is a .NET driver for the <http://little.io> services.

# Installation
This package is avaiable at <http://nuget.org/packages/little-dotnet>

# Configuration
You'll first need to create an instance of the little driver by supplying your api key and secret. The drivre is thread safe, but also lightweight. So you can either keep an instance around or create it as needed:

	var driver = new Little.Driver("KEY", "SECRET");

# Errors
Most errors will be raised as an instance of `Little.LittleException`

# Signing
The driver will automatically include the signature. However, signing helpers are available for use with the [https://github.com/littleio/little-javascript](JavaScript driver). 

# Usage
Once configured the services can be invoked via various methods:

	driver.Attempt.Create("leto", "1.2.3.4", false)