  SharpWebMail - ASP.NET Web Mail application written in C#
    v0.10b     - http://anmar.eu.org/projects/sharpwebmail/

Introduction
--------------------------------
SharpWebMail is an ASP.NET Web Mail application that is written in C#.
It uses a POP3 or IMAP servers as the mailstore and sends mail through a SMTP server.
It is very simple to configure (Only a few settings in the web.config file).
You can compose HTML messages, search your inbox, read complex mime messages, have
multiple address books and much more.

Features
--------------------------------

Very simple to configure (only a few settings in web.config)

Multiple POP3 and IMAP servers can be configured to read mail from.

Tries to minimize the queries to the email server caching message info

Authentication is handled by your email server

Sends mail through a SMTP server

HTML editor (FCKeditor) for composing mails

Download and send multiple attachments per message

It is possible to attach a file that we have received in other message without downloading it 

Reads MIME messages

Multiple Address books based on fully configurable datasources (ldap, oledb, odbc, ...)

Is possible to sort the inbox elements by number, sender, subject, date and size

Search the inbox by subject and sender

Multilingual user interface

XP look & feel

Tested with IE6 and Mozilla FireFox 1.0, but it should work with other versions

For a full set of features visit <http://anmar.eu.org/projects/sharpwebmail/features.html>

Installation requisites
--------------------------------

Requisites on installation machine:
 - Microsoft .NET Framework 1.1 or later or Mono runtime
 - An ASP.NET capable server (IIS or Apache + mod_mono)

Other requisites:
 - At least one SMTP server in order to send mails
 - At least one POP3 or IMAP server in order to read mails

Installation notes
--------------------------------

Copy the contents of asp.net folder into destination folder and
tweak web.config settings.

The asp.net/bin folder has the binary version for MS .NET 1.1 (release build).

If you are using other framework take the binaries from the bin folder of the distribution.
There are builds for the following platforms:
 - Microsoft .NET Framework 1.1
 - Microsoft .NET Framework 2.0 Beta 1
 - Mono 1.0
 - Mono 2.0

In each one you can select between debug and release version. With the debug one you can see more
information about exceptions, but its performance will be worse.

The dependencies must be copied also for the new platform. And the localized dlls must be rebuilt using
the provided controls/bin/resources.cmd

Building
--------------------------------

Full instructions can be found in <http://anmar.eu.org/projects/sharpwebmail/development.html>

Dependencies:
 - Microsoft .NET framework 1.1  - http://msdn.microsoft.com/netframework/
   or Mono - http://www.mono-project.com/
 - Log4net - http://logging.apache.org/log4net/
 - FCKeditor - http://www.fckeditor.net/
 - OpenSmtp - http://sourceforge.net/projects/opensmtp-net/
 - SharpMimeTools - For now distributed only with SharpWebMail

Project files provided for:
 - SharpDevelop - http://www.icsharpcode.net/

Authors
--------------------------------

 - Angel Marin <anmar at gmx.net> - http://anmar.eu.org/

License
--------------------------------

 - SharpWebMail code is under GPL v2 or later, read license.txt

History
--------------------------------

01/02/2005 - version 0.10b  - Released milestone 0.10
11/06/2004 - version 0.9b   - Released milestone 0.9
10/16/2004 - version 0.8b   - Released milestone 0.8
09/13/2004 - version 0.7b   - Released milestone 0.7
08/17/2004 - version 0.6b   - Released milestone 0.6
08/11/2004 - version 0.5b   - Released milestone 0.5
07/21/2004 - version 0.4b   - Released milestone 0.4
05/29/2004 - version 0.3b   - Released milestone 0.3
05/15/2004 - version 0.2b   - Released milestone 0.2
05/03/2004 - version 0.1b   - Released the first public beta


--------------------------------
Angel Marin 2003-2005 - http://anmar.eu.org/
