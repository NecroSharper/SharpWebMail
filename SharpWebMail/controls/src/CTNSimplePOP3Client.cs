// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2004 Angel Marin
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
	/// <summary>
	/// 
	/// </summary>
	public class CTNSimplePOP3Client {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
		protected readonly static System.String commandEnd = "\r\n";
		/// <summary>
		/// 
		/// </summary>
		protected readonly static System.String responseEnd = "\r\n.\r\n";
		/// <summary>
		/// 
		/// </summary>
		protected readonly static System.String responseEndSL = "\r\n";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		public CTNSimplePOP3Client( System.String host, System.Int32 port, System.String user, System.String pass ) {
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
		public CTNSimplePOP3Client( System.String host, System.Int32 port, System.String user, System.String pass, System.Double timeout ) {
			client = new anmar.SharpWebMail.CTNSimpleTCPClient(timeout);
			this.hostname = host;
			this.password = pass;
			this.portnumber = port;
			this.username = user;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool connect () {
			bool error = false;
			System.String response = null;

			if (!this.connected) {
				// Connect to POP3 Server
				error = !client.connect( this.hostname, this.portnumber );
				
				if (!error) {
					this.connected = true;
					error = !client.readResponse( ref response, responseEndSL );
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
		protected bool dele ( int mindex ) {
			bool error = false;

			// Send DEL for mindex message
			error = !this.sendCommand( "DELE " + mindex );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool disconnect () {
			bool error = false;

			if (this.connected) {
				// Disconnect from POP3 Server
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
		protected bool evaluateresponse ( System.String response ) {
			return response.ToLower().Trim().StartsWith("+ok");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inbox"></param>
		/// <param name="npage"></param>
		/// <param name="npagesize"></param>
		/// <param name="askserver"></param>
		/// <returns></returns>
		public bool getInboxIndex ( anmar.SharpWebMail.CTNInbox inbox, int npage, int npagesize, bool askserver ) {
			bool error = false;
			int total = 0;
			int totalbytes = 0;
			System.Collections.Hashtable list = new System.Collections.Hashtable();

			if ( !askserver ) {
				error = !inbox.buildMessageList ( list, npage, npagesize );
				askserver = (!error&&list.Count>0)?!askserver:askserver;
			}
			if ( askserver ) {
				error = !this.connect();
				error = (error)?error:!this.login ( this.username, this.password );
				error = (error)?error:!this.stat ( ref total, ref totalbytes );
				
				error = (error)?error:!this.getListToIndex ( list, total, inbox, npage, npagesize );

				if ( !error && total>0 && list.Count>0 ) {
					anmar.SharpMimeTools.SharpMimeHeader header = null;
					System.Collections.IDictionaryEnumerator msgenum = list.GetEnumerator();
					while ( !error && msgenum.MoveNext() ) {
						error = (error)?error:!this.getMessageHeader ( ref header, (int) msgenum.Key );
						if ( !error ) {
							inbox.newMessage ( msgenum.Value.ToString(), header );
						}
						header = null;
					}
				}
				this.quit();
			}
			return !error;
		}
		private bool getListToIndex ( System.Collections.Hashtable msgs, int total, anmar.SharpWebMail.CTNInbox inbox, int npage, int npagesize ) {
			bool error = false;

			if ( total>0 ){
				System.Int32[] list = new System.Int32[total];
				System.String[] uidllist = new System.String[total];

				// Get uidl list
				error = (error)?error:!this.uidl ( uidllist, 0);
				//Get messages list
				error = (error)?error:!this.list ( list );

				// Prepare message table with new messages
				error = (error)?error:!inbox.buildMessageTable ( list, uidllist );

				list = null;
				uidllist =  null;

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
		public bool getMessage ( ref System.IO.MemoryStream Message, int mindex, System.String uidl ) {
			bool error = false;

			error = !this.connect();
			error = (error)?true:!this.login ( this.username, this.password );
			if ( !error && uidl!=null ) {
				System.String[] uidllist = new System.String[mindex];
				error = !this.uidl( uidllist, mindex );
				// Make sure mindex message is there and its UIDL is uidl
				if ( error || uidllist[mindex-1]==null || !uidllist[mindex-1].Equals(uidl) ) {
					error = true;
				}
				uidllist=null;
			}
			error = (error)?true:!this.retr ( mindex, ref Message );
			this.quit();

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Header"></param>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected bool getMessageHeader ( ref anmar.SharpMimeTools.SharpMimeHeader Header, int mindex ) {
			bool error = false;
			System.IO.MemoryStream header = new System.IO.MemoryStream ();

			error = (error)?error:!this.top ( mindex, 0, ref header );
			if (!error) {
				Header = new anmar.SharpMimeTools.SharpMimeHeader( header, 0 );
			}

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		protected bool login ( System.String user, System.String pass ) {
			bool error = false;

			// Send USER and PASS and see what happends
			// Send USER Command
			error = !this.sendCommand( System.String.Concat( "USER ", user ) );
			// If USER is accepted send PASS
			error = (error)?true:!this.sendCommand( System.String.Concat( "PASS ", pass ) );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		protected bool list ( System.Int32[] list) {
			bool error = false;
			System.IO.MemoryStream response = new System.IO.MemoryStream();
			System.String tmp;

			// Send LIST and parse response
			// Send LIST Command
			error = !this.sendCommand( "LIST", ref response, false );
			//Parse the result
			if (!error) {
				System.IO.StreamReader resp = new System.IO.StreamReader(response);
				resp.ReadLine();

				for ( tmp=resp.ReadLine() ; tmp != null && tmp != "." ; tmp=resp.ReadLine() ) {
					try {
						String[] values = tmp.Split( null, 2 );
						list[System.Int32.Parse(values[0])-1] = System.Int32.Parse(values[1]);
					} catch (Exception) {
					}
				}
			}
			return !error;
		}
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
				foreach ( System.Data.DataRow msg in result )
					error = (error)?error:!this.dele ( (int)msg[1] );
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
		protected bool retr (int mindex, ref System.IO.MemoryStream response) {
			bool error = false;
			// Send RETR for mindex message
			// Send RETR Command
			error = !this.sendCommand( "RETR " + mindex, ref response, false );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		protected bool sendCommand ( System.String cmd ) {
			bool error = false;
			System.String response = null;

			//Send cmd and evaluate response
			if (connected) {
				// Send cmd
				error = !client.sendCommand( cmd, commandEnd );
				// Read Response
				error = (error)?true:!client.readResponse(ref response, responseEndSL);
				// Evaluate the result
				error = (error)?true:!this.evaluateresponse(response);
				if ( error || !cmd.Equals("QUIT") ) {
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
		protected bool sendCommand ( System.String cmd, ref System.IO.MemoryStream response, bool SLReponse ) {
			bool error = false;

			//Send cmd and evaluate response
			if (connected) {
				// Send cmd
				error = !client.sendCommand( cmd, commandEnd );
				// Read Response
				error = (error)?true:!client.readResponse(ref response, ((SLReponse)?responseEndSL:responseEnd) );

				System.Byte[] readBytes = new System.Byte[3];
				response.Seek(0,0);
				response.Read (readBytes,  0, 3);
				this.lastResponse = System.Text.Encoding.ASCII.GetString(readBytes, 0, 3);
				// Evaluate the result
				error = (error)?true:!this.evaluateresponse( this.lastResponse );
				response.Seek(0,0);
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
		protected bool stat (ref int num, ref int numbytes) {
			bool error = false;
			// Send STAT and parse response
			// Send STAT Command
			error = !this.sendCommand( "STAT");
			//Parse the result
			if (!error) {
				String[] values = this.lastResponse.Split( null, 3 );
				try {
					num = System.Int32.Parse(values[1]);
					numbytes = System.Int32.Parse(values[2]);
				} catch (System.Exception e) {
					num = 0;
					numbytes = 0;
					if ( log.IsErrorEnabled ) log.Error ( "Error while parsing STAT response", e );
				}
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <param name="nlines"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected bool top (int mindex, ulong nlines, ref System.IO.MemoryStream response) {
			bool error = false;
			// Send TOP for mindex message and get nlines lines of the message
			// Send TOP Command
			error = !this.sendCommand( "TOP " + mindex + " " + nlines, ref response, false );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected bool uidl ( System.String[] list, int mindex ) {
			bool error = false;
			System.IO.MemoryStream response = new System.IO.MemoryStream();
			System.String tmp;

			// Send UIDL and parse response
			// Send UIDL Command
			error = !this.sendCommand( "UIDL " + ((mindex>0)?mindex.ToString():System.String.Empty), ref response, (mindex>0) );
			//Parse the result
			if ( !error ) {
				System.IO.StreamReader resp = new System.IO.StreamReader(response);
				if ( mindex == 0 ) {
					resp.ReadLine();
				}
				for ( tmp=resp.ReadLine() ; tmp != null && tmp != "." ; tmp=resp.ReadLine() ) {
					try {
						if ( mindex>0 ) {
							tmp = tmp.Remove(0,4);
						}
						String[] values = tmp.Split( null, 2 );
						list[System.Int32.Parse(values[0])-1] = values[1];
					} catch ( System.Exception e ) {
						if ( log.IsErrorEnabled ) log.Error ( "Error while parsing UIDL response", e );
					}
				}
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool quit () {
			bool error = false;

			// Send Quit and disconnect
			error = !this.sendCommand( "QUIT");
			this.disconnect();

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		public string errormessage {
			get {
				return client.errormessage;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string lastMessage {
			get {
				return this.lastResponse;
			}
		}
	}
}
