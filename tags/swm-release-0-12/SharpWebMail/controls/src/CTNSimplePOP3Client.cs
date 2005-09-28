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
	/// <summary>
	/// 
	/// </summary>
	internal class CTNSimplePOP3Client : anmar.SharpWebMail.SimpleEmailClient {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		public CTNSimplePOP3Client( System.String host, System.Int32 port, System.String user, System.String pass ) : base(host, port, user, pass) {
			this.commandEnd = "\r\n";
			this.responseEnd = "\r\n.\r\n";
			this.responseEndSL = "\r\n";
			this.responseEndOnEnd = true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <param name="timeout"></param>
		public CTNSimplePOP3Client( System.String host, System.Int32 port, System.String user, System.String pass, long timeout ) : base(host, port, user, pass, timeout) {
			this.commandEnd = "\r\n";
			this.responseEnd = "\r\n.\r\n";
			this.responseEndSL = "\r\n";
			this.responseEndOnEnd = true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected override System.String buildcommand ( anmar.SharpWebMail.EmailClientCommand cmd, params System.Object[] args ) {
			System.String command = System.String.Empty;
			switch ( cmd ) {
				case anmar.SharpWebMail.EmailClientCommand.Delete:
					if ( args.Length==1 )
						command = System.String.Format("DELE {0}", args[0]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.Header:
					if ( args.Length==2 )
						command = System.String.Format("TOP {0} {1}", args[0], args[1]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.ListSize:
					command = "LIST";
					break;
				case anmar.SharpWebMail.EmailClientCommand.ListUID:
					if ( args.Length==1 ) {
						if ( args[0]==null || args[0].Equals(System.String.Empty) )
							command = "UIDL";
						else
							command = System.String.Concat("UIDL ", args[0]);
					}
					break;
				case anmar.SharpWebMail.EmailClientCommand.Logout:
					command = "QUIT";
					break;
				case anmar.SharpWebMail.EmailClientCommand.Message:
					if ( args.Length==1 )
						command = System.String.Format("RETR {0}", args[0]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.Status:
					command = "STAT";
					break;
				
			}
			return command;
		}
		protected override bool commandResponseTypeIsSL ( anmar.SharpWebMail.EmailClientCommand cmd, params System.Object[] args ) {
			bool responseSL = true;
			switch ( cmd ) {
				case anmar.SharpWebMail.EmailClientCommand.Delete:
					break;
				case anmar.SharpWebMail.EmailClientCommand.Header:
					responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.ListSize:
					responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.ListUID:
					if ( args.Length==1 )
						if ( args[0].ToString().Equals(System.String.Empty) )
							responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.Logout:
					break;
				case anmar.SharpWebMail.EmailClientCommand.Message:
					responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.Status:
					break;
				
			}
			return responseSL;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool evaluateresponse ( System.String response ) {
			return response.ToLower().Trim().StartsWith("+ok");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override System.IO.MemoryStream getStreamDataPortion ( System.IO.MemoryStream data ) {
			return data;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		protected override bool login ( System.String user, System.String pass ) {
			bool error = false;

			// Send USER and PASS and see what happends
			// Send USER Command
			error = !this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Login, System.String.Concat( "USER ", user ) );
			// If USER is accepted send PASS
			error = (error)?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Login, System.String.Concat( "PASS ", pass ) );

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool parseListSize ( System.Int32[] list, System.IO.MemoryStream response ) {
			bool error = false;
			System.String tmp;
			//Parse the result
			if (!error) {
				System.IO.StreamReader resp = new System.IO.StreamReader(response);
				resp.ReadLine();

				for ( tmp=resp.ReadLine() ; tmp != null && tmp != "." ; tmp=resp.ReadLine() ) {
					try {
						String[] values = tmp.Split( null, 2 );
						list[System.Int32.Parse(values[0])-1] = System.Int32.Parse(values[1]);
					} catch ( System.Exception e ) {
						if ( log.IsErrorEnabled ) log.Error ( "Error while parsing LIST response", e );
					}
				}
			}
			return !error;
		}
		protected override bool parseListUID ( System.String[] list, System.IO.MemoryStream response, int mindex ) {
			bool error = false;
			System.String tmp;
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
		/// <param name="num"></param>
		/// <param name="numbytes"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool parseStatus ( ref int num, ref int numbytes, System.IO.MemoryStream response ) {
			bool error = false;
			//Parse the result
			if (!error) {
				System.String resp = new System.IO.StreamReader(response).ReadToEnd();
				String[] values = resp.Split( null, 3 );
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
	}
}
