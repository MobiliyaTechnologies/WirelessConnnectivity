# Wireless Connnector Service

Fetches data from Sensor Tag server and pushes it on to the Azure SQL Server.

---

## Setup

* Open App.config file in the source code/bin folder
* Update the values of the Key value pairs listed under **appSettings** section as below.
    * **sql:ConnectionString** : Mention the details about the SQL server where you want to dump the data. There are placeholders given in the value field for various credentials like server address, username, password etc.
    * **ClientId** : Client ID received from the device manufacturer in order to authenticate to the manufacturer's APIs.
    * **Client Secret** : Client Secret received from the device manufacturer in order to authenticate to the manufacturer's APIs.
    * **Client Code** : Client Code received from the device manufacturer in order to authenticate to the manufacturer's APIs.
* Done

## Installation

The service could be hosted in Azure as a web job, else could be run as a console app on local machine or a VM

### Steps

* Build the source code.
* A **bin** folder would be created in the project directory.
* Run the **\<ProjectName>**.exe executable to start the console app. 
