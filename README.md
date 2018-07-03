# RoLaMoDS
RoLaMoDS is the web-system of recognize lanscapes and modeling dangerous situations

## Subsystems
- Main subsystem (completed) - frontend and backend part of system. Manages database and file storage and use functions of other subsystem. Writed on Asp .Net Core 2.1. 
- Recognition subsytem (not completed) - library for recognition of satellite's photos and topographic's maps. Have submodule to train models NN and create new models NN. Used CNTK. 
- Modeling subsytem (only planned) - library for modeling dangerous situations. Must contains regulations for cellular automaton.

## Main subsystem
Main subsystem is the interface between user and libraries. It`s a client-server application bilding on Asp .Net Core MVC.
This subsystem receive data from user, prepares them for using in libraries (such as create base state of cellular automaton or divide image on cells for recognation), call libraries functions, receive result from libraries and display result for user.
In subsystem realised internal and external authorize (now only Google)

### Before runing
Befor running main subsystem for the first time you must:
- open appsettings.json, setup connection string to database and use command `dotnet ef database update` for update database schema.
- configure secrets for GoogleApi (using `dotnet user-secrets set "Google:Authentication:Client_Id" "value"` and `dotnet user-secrets set "Google:Authentication:Client_Secret" "value"`)

### Dependency 
- Microsoft.AspNetCore.App
- CoreCompat.System.Drawing.v2
- Newtonsoft.Json
- SharpZipLib

### Example interface
<p align="center">
  <img src="https://user-images.githubusercontent.com/24876822/42230098-d0df1234-7ef0-11e8-9d4a-17d9cb781a15.PNG" width="500px"/>
</p>
