
<head>
<H1> Development Notes</H1>
<p>This application and the code attached was made for profit purposes. Unatherized use and plagerism will be charged to the fuillest extent permitted by law.
</p>

<p>This document was created to log and assist in the development of the attached application and contains the following resources
</p>

* Change_logs.
* Dev_Journal.
* Bug_logging.
* Dev_Logs.
* Third Party Licences & Credits.
</head>
<body>

<h2> Change Logs: </h2>

* milestone 1
    : Start Date 4/0/2023 : End Date: 5/17/23
        - Features:
            - Created Menu Assets
            - Created Game Assets
            - Created Server
            - Created Client   
* Milestone 2
    : StartDate: 5/17/2023 :
        - BugFixes:
        - Changes:
            - Added GameAsset attribute to Games.xml ServerSide and the gameData object client side
            - Seperated the ProfileHeadboard and game Select from MainMenu
            - Interface.cs Is now in charge of all UI Menu Assets
        - features:
            - Made gamemanager.cs capible of starting diffrent types of games if they were avalable
            - Added Login Menu

<h2>Dev_Journal:</h1>

* James H. Dudley. [Date:5/17/2023 Time:10pm]
    : Just designed the readme, the document contains Change_logs,Dev_Journals, Dev_logs, and a Third Party Licences and Credits section for ease of access.    The game has gone well so far i just started college on the first of this month so il try to crank out work as i can. additionaly it looks like il need a remote job soon so il start working torward it at the end of the month.

* James H. Dudley. [Date:5/17/2023 Time:10pm]
    : Just made the interface the root of all UI menues. Took a bit of time seperating assets and redoing the code to gothrough interface.cs
    will start focusing on completeing the login Interface aswell as the Profile Headboard interface

<h2>Bug_Logging:</h1>

* **[5/18/23]** **samples snaping ther scale in game preview!**
    - The sample games are resizeing the first time they translate to center node 
        : **Resolved Push to Changelog** 89 SlotMachineSampler Write Code to change Original Scale from vector 2 to float and change min scale in line 149 to be orignalscale?? as long as min scale matches scene scale !!!not the min size max size


<h2>Dev_Logs:</h1>

<h3> Archived Task: </h3>

* [#1]  Make script on roller that randomly changes the textures on roller items    <Completed>
      
       : **Notes:** *rollers are controlled server side.

        Status : Redacted\*
        :   - [x] Complete
            - [ ] Pending     
            - [ ] Active

* [#2]  Need to make string refrence to package assets in program.cs root   <Completed>
      
       : **Notes:** *passing game package name over network, under **gameDataObj.Asset_name**. IO contains dependincy assets for each type of game package. this will allow code to be reusable for **multiple types of games**

        Status : Redacted\*
        :   - [x] Complete
            - [ ] Pending     
            - [ ] Active

* [#4]  Enable Login Window with skip button (temparay)  <Complete>
      
       : **Notes:** 

        Status :
        :   - [X] Complete
            - [ ] Pending     
            - [ ] Active

* [#5]  Make game preview from Indipendent from MainMenu <Complete>
      
       : **Notes:** Still need to instantiate it when its needed and destroy it when its not
       or move it off screen?

        Status :
        :   - [X] Complete
            - [ ] Pending     
            - [ ] Active
<h3> Incomplete Task: </h3>


* [#3]  Pass VIP coin data over UserData, pass slogan over UserData  <Pending>
      
       : **Notes:** 

        Status :
        :   - [ ] Complete
            - [X] Pending     
            - [ ] Active

* [#6]  Make game preview Activily Represent GameData Textures & Name <Pending>
      
       : **Notes:**

        Status :
        :   - [ ] Complete
            - [X] Pending     
            - [ ] Active

* [#7]  Complete Login Menu <Pending>
      
       : **Notes:** adding terms of service as well as server side feedback for failed login attemps
       server responds API.task.Error(api.task.call.GetTermsOFService{}: || api.task.login{"input either username or email and password. if you forgot your password contact jamesdudley1129@gmail.com to reset it"}: || api.task.login{Char Rules or account name or email already exist ect....}:) almost done server side still need to make the new commands client side as well as make the client able to receave new server commands addtionally i will neet to make a popup window for accepting the terms of service after i get the networking stuff done.

        Status :
        :   - [ ] Complete
            - [ ] Pending     
            - [X] Active
* [#7]  Add Push Updater for netework based enumerators and classes <Pending>
      
       : **Notes:** Place Network Classes, Network Enumerators & switch statements using the enumerators in a seperate file
       (same for both the server and the client).
       write a program that can append changes to the classes in the client list
       (do not overwrite data).
       Additionally, append any changes in network enumerators and switch statements required on the client
       (Do not overwrite logic inside switch statements)
       When writing new switch conditions to the client make sure to include a method that calls a print type error
       ex "Server appended switch condition missing logic"
       This will be important for implementing new network data types without breaking the client and having to waste hours debugging.

        Status :
        :   - [ ] Complete
            - [ ] Pending     
            - [X] Active

 <h2>Third Party Credits and Licences:</h1>

 * Software Licence
    : - Godot Engine
 * AI created Artwork? technicly I own the rights to the art, becouse I prompted the design
    : - OpenAI  
</body>

