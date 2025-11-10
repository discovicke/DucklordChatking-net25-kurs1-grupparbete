# Möte för att breifa och se över arbetsflöde.
---- 
## Punkter för dagens protokoll.
- Hur ser programemt ut.
- Ui
- Gå igenom nuvarande struktur.
- Ändiringar som ska göras för struktur.
- Hur ligger vi till med servern.
- Servern vad funkar och vad funkar inte. 
- skapa en  ToDo lista. ev lägga in den i programmet?
- Genomgång av vilka klasser vi använder oss av.
- 
---

## Genomgång av programmet .
- Hur fungerar klasserna.
- Olika metoder.
- funktioner av knappar
- funktioner av textfält ( användarnamn och lösenord).
- muslogik i programmet.
- interaktioner med fält i programmet.

---
#### Genomgång av main program
- Mus logikt alltid först innan anropar ex MainMenu.Run()
- Main() ska anropa olika klasser och metoder för att hålla tydlig struktur
---
#### Register.cs
- Hanterar user properties
- validerar metod 
- if-statser för null check mm
- bool checkar

---
#### Login.cs
- Hantering av användere
- checkar om anändare finns i serven
- bool checkar
- om inloggnings försök fungerar så tas vidare till Chat.Run();

---
#### ChatScreen.cs
- SendMessage(); till server
- GetMassage(); tar emot text meddelande, datestamps, andvändare som skrivit.
- Chatt ruta för att skriva text i.
- knapp för att skicka meddelande. ( ska mäven gå att ttrycka på Enter)


---
#### UI 
Få ut en test version för att se om kopplingarna fungerar.
Funktionerna som vi vill implementera ska funka så ser vi över UI senare.

### Struktur:
#### Data mapp: 
- alla klasser som skickar tar emot data från serven ska ligga i den.

#### Ui mapp:
- alla klasser som har med UI att göra.

##### Cofigirations mapp:
- Muslogik
- Textlogik för interaktioner med ex chatt rutor
- klass för färger
- knapplogik
- Flytta in int variablar för knapplogik till Cofigirations mapp med klassen button
- 
## TODO! 
- user.Register(); metod för send to DTO
- user.LogIn(); metod för send to DTO
- 