// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2005 Angel Marin
// 
//   This file is part of SharpWebMail.
//
//   SharpWebMail is free software; you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation; either version 2 of the License, or
//   (at your option) any later version.
//
//   SharpWebMail is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with SharpWebMail; if not, write to the Free Software
//   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// -----------------------------------------------------------------------

using System;

namespace anmar.SharpWebMail
{
	internal abstract class SimpleEmailClient : anmar.SharpWebMail.IEmailClient {
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private anmar.SharpWebMail.CTNSimpleTCPClient client;
		/// <summary>
		/// 
		/// </summary>
		protected System.Boolean connected;
		/// <summary>
		/// 
		/// </summary>
		protected System.Int32 portnumber;
		/// <summary>
		/// 
		/// </summary>
		protected System.String hostname;
		/// <summary>
		/// 
		/// </summary>
		protected System.String lastResponse;
		/// <summary>
		/// 
		/// </summary>
		protected System.String password;
		/// <summary>
		/// 
		/// </summary>
		protected System.String username;
		/// <summary>
		/// 
		/// </summary>
		protected System.String commandEnd;
		/// <summary>
		/// 
		/// </summary>
		protected bool responseEndOnEnd;
		/// <summary>
		/// 
		/// </summary>
		protected System.String responseEnd;
		/// <summary>
		/// 
		/// </summary>
		protected System.String responseEndSL;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		public SimpleEmailClient( System.String host, System.Int32 port, System.String user, System.String pass ) {
			client = new anmar.SharpWebMail.CTNSimpleTCPClient();
			this.hostname = host;
			this.password = pass;
			this.portnumber = port;
			this.username = user;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <param name="timeout"></param>
		public SimpleEmailClient( System.String host, System.Int32 port, System.String user, System.String pass, System.Double timeout ) {
			client = new anmar.SharpWebMail.CTNSimpleTCPClient(timeout);
			this.hostname = host;
			this.password = pass;
			this.portnumber = port;
			this.username = user;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected abstract System.String buildcommand ( anmar.SharpWebMail.EmailClientCommand cmd, params System.Object[] args );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected abstract bool commandResponseTypeIsSL ( anmar.SharpWebMail.EmailClientCommand cmd, params System.Object[] args );
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool connect () {
			bool error = false;
			System.String response = null;

			if (!this.connected) {
				// Connect to email server
				error = !client.connect( this.hostname, this.portnumber );
				
				if (!error) {
					this.connected = true;
					error = !client.readResponse( ref response, responseEndSL, true );
					error = (error)?true:!this.evaluateresponse( response );
					this.lastResponse = response;
				}
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected virtual bool delete ( int mindex ) {
			bool error = false;

			// Send delete command for mindex message
			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.Delete, mindex);
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Delete, cmd );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		protected virtual bool deletemessages ( System.Data.DataRow[] result ) {
			bool error = false;
			foreach ( System.Data.DataRow msg in result )
				error = (error)?error:!this.delete ( (int)msg[1] );
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool disconnect () {
			bool error = false;

			if (this.connected) {
				// Disconnect from email server
				error = !client.closeConnection();
				this.connected = false;
			}

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		protected abstract bool evaluateresponse ( System.String response );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inbox"></param>
		/// <param name="npage"></param>
		/// <param name="npagesize"></param>
		/// <param name="askserver"></param>
		/// <returns></returns>
		public virtual bool getInboxIndex ( anmar.SharpWebMail.CTNInbox inbox, int npage, int npagesize, bool askserver ) {
			bool error = false;
			int total = 0;
			int totalbytes = 0;
			System.Collections.Hashtable list = new System.Collections.Hashtable();

			if ( !askserver ) {
				error = !inbox.buildMessageList ( list, npage, npagesize );
				askserver = (!error&&list.Count>0)?!askserver:askserver;
			}
			if ( askserver ) {
				System.Collections.Hashtable messages = new System.Collections.Hashtable();
				error = !this.connect();
				error = (error)?error:!this.login ( this.username, this.password );
				error = (error)?error:!this.status ( ref total, ref totalbytes );
				
				error = (error)?error:!this.getListToIndex ( list, total, inbox, npage, npagesize );

				if ( !error && total>0 && list.Count>0 ) {
					System.IO.MemoryStream header = null;
					foreach ( System.Collections.DictionaryEntry msg in list ) {
						error = (error)?error:!this.getMessageHeader ( out header, (int) msg.Key );
						if ( !error )
							messages.Add(msg.Value, header);
					}
				}
				this.quit();
				foreach ( System.Collections.DictionaryEntry item in messages ) {
					System.IO.MemoryStream stream = this.getStreamDataPortion(item.Value as System.IO.MemoryStream);
					anmar.SharpMimeTools.SharpMimeHeader header = new anmar.SharpMimeTools.SharpMimeHeader( stream, stream.Position );
					header.Close();
					inbox.newMessage ( item.Key.ToString(), header );
				}
			}
			return !error;
		}
		private bool getListToIndex ( System.Collections.Hashtable msgs, int total, anmar.SharpWebMail.CTNInbox inbox, int npage, int npagesize ) {
			bool error = false;

			if ( total>0 ){
				System.Int32[] list = new System.Int32[total];
				System.String[] uidlist = new System.String[total];

				// Get uid list
				error = (error)?error:!this.uidl ( uidlist, 0);
				//Get messages list
				error = (error)?error:!this.list ( list );

				// Prepare message table with new messages
				error = (error)?error:!inbox.buildMessageTable ( list, uidlist );

				list = null;
				uidlist =  null;

				//Determine what messages we have to index
				error = (error)?error:!inbox.buildMessageList ( msgs, npage, npagesize );
			} else {
				inbox = new anmar.SharpWebMail.CTNInbox ();
			}

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Message"></param>
		/// <param name="mindex"></param>
		/// <param name="uidl"></param>
		/// <returns></returns>
		public bool getMessage ( System.IO.MemoryStream message, int mindex, System.String uid ) {
			bool error = false;

			error = !this.connect();
			error = (error)?true:!this.login ( this.username, this.password );
			if ( !error && uid!=null ) {
				System.String[] uidllist = new System.String[mindex];
				error = !this.uidl( uidllist, mindex );
				// Make sure mindex message is there and its UID is uid
				if ( error || uidllist[mindex-1]==null || !uidllist[mindex-1].Equals(uid) ) {
					error = true;
				}
				uidllist=null;
			}
			error = (error)?true:!this.retrieve ( mindex, message );
			if ( !error )
				message = this.getStreamDataPortion(message);
			this.quit();

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Header"></param>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected bool getMessageHeader ( out System.IO.MemoryStream stream, int mindex ) {
			bool error = false;
			stream = new System.IO.MemoryStream ();
			error = (error)?error:!this.header ( mindex, 0, stream );
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected abstract System.IO.MemoryStream getStreamDataPortion ( System.IO.MemoryStream data );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <param name="nlines"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected virtual bool header ( int mindex, ulong nlines, System.IO.MemoryStream response ) {
			bool error = false;

			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.Header, mindex, nlines);
			bool SLResponse = this.commandResponseTypeIsSL(anmar.SharpWebMail.EmailClientCommand.Header, mindex, nlines);
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Header, cmd, response, SLResponse );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		protected virtual bool login ( System.String user, System.String pass ) {
			bool error = false;

			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.Login, user, pass);
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Login, cmd );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		protected virtual bool list ( System.Int32[] list ) {
			bool error = false;
			System.IO.MemoryStream response = new System.IO.MemoryStream();

			// Send LIST and parse response
			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.ListSize);
			bool SLResponse = this.commandResponseTypeIsSL(anmar.SharpWebMail.EmailClientCommand.ListSize);

			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.ListSize, cmd, response, SLResponse );

			//Parse the result
			error = (error)?true:!this.parseListSize(list, response);

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="SLReponse"></param>
		protected virtual void parseLastResponse ( System.IO.MemoryStream response, bool SLReponse ) {
			System.Byte[] readBytes = new System.Byte[3];
			response.Seek(0,0);
			response.Read (readBytes,  0, 3);
			this.lastResponse = System.Text.Encoding.ASCII.GetString(readBytes, 0, 3);
			response.Seek(0,0);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected abstract bool parseListSize ( System.Int32[] list, System.IO.MemoryStream response );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected abstract bool parseListUID ( System.String[] list, System.IO.MemoryStream response, int mindex );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="num"></param>
		/// <param name="numbytes"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected abstract bool parseStatus ( ref int num, ref int numbytes, System.IO.MemoryStream response );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inbox"></param>
		/// <param name="all"></param>
		/// <returns></returns>
		public bool purgeInbox ( anmar.SharpWebMail.CTNInbox inbox, bool all ) {
			bool error = false;
			System.String filter;
			if ( all )
				filter = "";
			else
				filter = "delete=true";
			System.Data.DataRow[] result = inbox.getInbox.Select(filter);
			if ( result.GetLength(0)>0 ) {
				error = !this.connect();
				error = (error)?true:!this.login ( this.username, this.password );
				error = (error)?true:!this.deletemessages(result);
				this.quit();
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected virtual bool retrieve ( int mindex, System.IO.MemoryStream response ) {
			bool error = false;

			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.Message, mindex);
			bool SLResponse = this.commandResponseTypeIsSL(anmar.SharpWebMail.EmailClientCommand.Message, mindex);
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Message, cmd, response, SLResponse );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		protected virtual bool sendCommand ( anmar.SharpWebMail.EmailClientCommand command, System.String cmd ) {
			bool error = false;
			System.String response = null;

			//Send cmd and evaluate response
			if (connected) {
				// Send cmd
				error = !client.sendCommand( cmd, commandEnd );
				// Read Response
				error = (error)?true:!client.readResponse(ref response, responseEndSL, true);
				// Evaluate the result
				error = (error)?true:!this.evaluateresponse(response);
				if ( error || !command.Equals(anmar.SharpWebMail.EmailClientCommand.Logout) ) {
					this.lastResponse = response;
				}
			} else {
				error = true;
			}

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="response"></param>
		/// <param name="SLReponse"></param>
		/// <returns></returns>
		protected virtual bool sendCommand ( anmar.SharpWebMail.EmailClientCommand command, System.String cmd, System.IO.MemoryStream response, bool SLReponse ) {
			bool error = false;

			//Send cmd and evaluate response
			if (connected) {
				// Send cmd
				error = !client.sendCommand( cmd, commandEnd );
				// Read Response
				error = (error)?true:!client.readResponse( response, ((SLReponse)?responseEndSL:responseEnd), (SLReponse||this.responseEndOnEnd));
				// Get the last response string from a multiline response
				this.parseLastResponse( response, SLReponse);
				// Evaluate the result
				error = (error)?true:!this.evaluateresponse( this.lastResponse );
				response.Seek(0, System.IO.SeekOrigin.Begin);
			} else {
				error = true;
			}

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="num"></param>
		/// <param name="numbytes"></param>
		/// <returns></returns>
		protected virtual bool status ( ref int num, ref int numbytes ) {
			bool error = false;
			System.IO.MemoryStream response = new System.IO.MemoryStream();
			// Get status command
			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.Status);
			bool SLResponse = this.commandResponseTypeIsSL(anmar.SharpWebMail.EmailClientCommand.Status);
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Status, cmd, response, SLResponse );

			//Parse the result
			error = (error)?true:!this.parseStatus(ref num, ref numbytes, response);

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected virtual bool uidl ( System.String[] list, int mindex ) {
			bool error = false;
			System.IO.MemoryStream response = new System.IO.MemoryStream();

			// Send ListUID command
			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.ListUID, ((mindex>0)?mindex.ToString():System.String.Empty));
			bool SLResponse = this.commandResponseTypeIsSL(anmar.SharpWebMail.EmailClientCommand.ListUID, ((mindex>0)?mindex.ToString():System.String.Empty));
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.ListUID, cmd, response, SLResponse );

			//Parse the result
			error = (error)?true:!this.parseListUID(list, response, mindex);

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual bool quit () {
			bool error = false;

			// Send Quit and disconnect
			System.String cmd = this.buildcommand(anmar.SharpWebMail.EmailClientCommand.Logout);
			error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Logout, cmd );

			this.disconnect();

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		public System.String errormessage {
			get {
				return client.errormessage;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public System.String lastMessage {
			get {
				return this.lastResponse;
			}
		}
		public System.String Password {
			get {
				return this.password;
			}
		}
		public System.String UserName {
			get {
				return this.username;
			}
		}
	}
	internal enum EmailClientCommand {
		Delete,
		Header,
		Login,
		Logout,
		ListSize,
		ListUID,
		Message,
		Other,
		Status
	}
}
