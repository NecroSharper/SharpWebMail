  SharpWebMail - ASP.NET Web Mail application written in C#
    v0.5b     - http://anmar.eu.org/projects/sharpwebmail/

Introduction
--------------------------------
SharpWebMail is an ASP.NET Web Mail application that is written in C#.
It uses a POP3 or IMAP servers as the mailstore and sends mail through a SMTP server.
It is very simple to configure (Only a few settings in the web.config file).
You can compose HTML messages, search your inbox, read complex mime messages
and much more.

Features
--------------------------------

Very simple to configure (only a few settings in web.config)

Multiple POP3 and IMAP servers can be configured to read mail from.

Tries to minimize the queries to the email server caching message info

Authentication is handled by the POP3 server

Sends mail through a SMTP server

HTML editor (FCKeditor) for composing mails

Download and send multiple attachments per message

It is possible to attach a file that we have received in other message without downloading it 

Reads MIME messages

Is possible to sort the inbox elements by number, sender, subject, date and size

Search the inbox by subject and sender

Multilingual user interface

XP look & feel

Tested with IE6 and Mozilla FireFox 0.9, but it should work with other versions

Installation requisites
--------------------------------

Requisites on installation machine:
 - Microsoft .NET Framework 1.1
 - An ASP.NET capable server

Other requisites:
 - At least one SMTP server in order to send mails
 - At least one POP3 or IMAP server in order to read mails

Installation notes
--------------------------------

Copy the contents of asp.net folder into destination folder and
tweak web.config settings.

Building
--------------------------------

Dependencies:
 - Microsoft .NET framework 1.1  - http://msdn.microsoft.com/netframework/
 - Log4net - http://logging.apache.org/log4net/
 - FCKeditor - http://www.fckeditor.net/
 - SharpMimeTools - For now distributed only with SharpWebMail

Project files provided for:
 - SharpDevelop - http://www.icsharpcode.net/

Authors
--------------------------------

 - Angel Marin <anmar at gmx.net> - http://anmar.eu.org/


Licence
--------------------------------

 - SharpWebMail code is under GPL v2 or later, read license.txt

History
--------------------------------

08/11/2004 - version 0.5b   - Released milestone 0.5
07/21/2004 - version 0.4b   - Released milestone 0.4
05/29/2004 - version 0.3b   - Released milestone 0.3
05/15/2004 - version 0.2b   - Released milestone 0.2
05/03/2004 - version 0.1b   - Released the first public beta


--------------------------------
Angel Marin 2003 - 2004 - http://anmar.eu.org/
