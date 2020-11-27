# just-eat-sdet-test

## Prerequisite
Developed on MacOS with VSCode, therefore .Net Core (v5.0) for Mac is required.

## Dev notes
create project with $ dotnet new console
add additonal packages $ add package NewtonSoft.Json

## Build project
$ dotnet restore

(this will add the obj folder)

## Run tests
$ dotnet run

(this will add the bin folder)

## Notes
I'm using a different postcode to reduce the size of the response data, in order for the tests to run faster. 
You can of course edit the postcode and rerun the tests.

The 3 more requirement tests I added are:

1. Validate that Name Exists
2. Cuisine Types check
3. If IsDelivery is true, then DeliveryStartTime should exist