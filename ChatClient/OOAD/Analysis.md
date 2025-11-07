# Analysis
Text om syfte, målgrupp, användningsfall


* __Chat.Client/__          #Raylib GUI och användarinteraktion
    * Program.cs
  * __UI/__
    * ChatWindow.cs
    * LoginScreen.cs
      * InputHandler.cs
    * __Networking/__
      * ClientConnection.cs
      * MessageReceiver.cs
    * __Models/__
      * ChatMessage.cs
      * UserAccount.cs
* __DiscordChat.Server/__ #Simulerad server (eller socketserver?) _@CHRISTIAN, svar:_
  * Program.cs
  * __Core/__
    * ServerManager.cs
    * UserManager.cs
    * MessageManager.cs
  * __Data/__
    * Database.cs
* __DiscordChat.Shared/__          #Gemensamma modeller och protokoll
  * __DTO/__     #DataTransferObjects
    * MessageDTO.cs
    * LoginDTO.cs
  * __Enums/__
    * MessageType.cs
  * __Interfaces/__
    * INetworkSerializable.cs
         
       
* __DiscordChat.Tests/__           #xUnit-testprojekt
  * NetworkingTests.cs
  * UserManagerTests.cs
  * MessageManagerTests.cs
  * UITests.cs