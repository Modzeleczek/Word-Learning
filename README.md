# Word Learning
Word Learning is a desktop application for learning English vocabulary. It was created using .NET Framework and Windows Presentation Foundation (WPF) for graphical user interface. In Word Learning, every user must firstly create an account with which a vocabulary database is associated. An user's data is encrypted with AES so others cannot see his or her progress. The application consists of 4 main views described below.

## Application views

### Learning
In `Learning` view, you locally browse your words and their details. Additionally, you can download words in order to extend your database. Everytime you click the download button, Word Learning gets a number of new words from [Random Word API (accessed 18.03.2023)](https://random-word-api.herokuapp.com/home). For every generated word, the application fetches details (i.e. name of the part of speech, definition, synonyms and example of use) from [Free Dictionary API (accessed 18.03.2023)](https://dictionaryapi.dev/).

---
### Quiz
Everytime you open `Quiz` view, if there are words in the database, a quiz attempt is generated. It consists of a word definition and four answers psuedorandomly selected from your database. Correct answers increase the learning progress of a particular word depicted by the emoticon associated with the word in `Learning` view. A grinning emoji near a word indicates its maximal expertise. An incorrect answer resets the progress of a word. 

---
### Quiz history
In this view, you can browse your past quiz attempts. Each attempt saves its details which are: the question, possible answers, your answer and the correct answer.

---
### Statistics
This view presents 4 counters:
- Words downloaded - the number of words currently stored in your database.
- Words learned - the number of words in which you have reached maximal progress.
- Total definition quiz attempts - the total number of correctly or incorrectly solved quiz attempts.
- Correct definition matches - the number of correctly solved quiz attempts.

Additionally, `Statistics` view contains a trash bin button which you can use to reset your progress and delete all words from your database.

---
## Running on Windows
1. Download and install .NET Framework Runtime 4.7.2 from Microsoft's [page (accessed 18.03.2023)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472).

2. Download and unpack the latest release from this repository.

3. Run `Word Learning.exe`. After creating the first user account, the application creates `storage.json` file containing all users' data. Therefore, do not edit that file by hand nor delete it.

---
## Presentation
Click the image below to watch a presentation video on YouTube.  
[<img src="https://i.imgur.com/5pIWoWN.png" width="500"/>](https://youtu.be/usnewtZVziQ)
