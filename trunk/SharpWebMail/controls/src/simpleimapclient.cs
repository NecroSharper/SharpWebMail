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
	internal class SimpleIMAPClient : anmar.SharpWebMail.SimpleEmailClient {
		/// <summary>
		/// 
		/// </summary>
		protected System.String folder;
		/// <summary>
		/// 
		/// </summary>
		protected System.String server_delimiter;
		/// <summary>
		/// 
		/// </summary>
		protected System.String tag;
		private bool selected = false;
		private System.Random taggen;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		public SimpleIMAPClient( System.String host, System.Int32 port, System.String user, System.String pass ) : base(host, port, user, pass) {
			this.folder = "INBOX";
			this.server_delimiter = "/";
			this.taggen = new System.Random();
			this.commandEnd = "\r\n";
			this.responseEndSL = "\r\n";
			this.responseEndOnEnd = false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="user"></param>
		/// <param name="pass"></param>
		/// <param name="timeout"></param>
		public SimpleIMAPClient( System.String host, System.Int32 port, System.String user, System.String pass, System.Double timeout ) : base(host, port, user, pass, timeout) {
			this.folder = "INBOX";
			this.server_delimiter = "/";
			this.taggen = new System.Random();
			this.commandEnd = "\r\n";
			this.responseEndSL = "\r\n";
			this.responseEndOnEnd = false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected override System.String buildcommand ( anmar.SharpWebMail.EmailClientCommand cmd, params System.Object[] args ) {
			System.String command = System.String.Empty;
			this.randomTag();
			switch ( cmd ) {
				case anmar.SharpWebMail.EmailClientCommand.Delete:
					if ( args.Length==1 )
						command = System.String.Format("{0} STORE {1}:{1} +FLAGS.SILENT (\\Deleted)", this.tag, args[0]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.Header:
					if ( args.Length==2 )
						command = System.String.Format("{0} FETCH {1}:{1} (RFC822.HEADER)", this.tag, args[0], args[1]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.ListSize:
					command = System.String.Format("{0} FETCH 1:* (RFC822.SIZE)", this.tag);
					break;
				case anmar.SharpWebMail.EmailClientCommand.ListUID:
					if ( args.Length==1 ) {
						if ( args[0].ToString().Equals(System.String.Empty) )
							command = System.String.Format("{0} FETCH 1:* (UID)", this.tag);
						else
							command = System.String.Format("{0} FETCH {1}:{1} (UID)", this.tag, args[0]);
					}
					break;
				case anmar.SharpWebMail.EmailClientCommand.Login:
					if ( args.Length==2 )
						command = System.String.Format("{0} LOGIN {1} {2}", this.tag, args[0], args[1]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.Logout:
					command = System.String.Concat(this.tag, " LOGOUT");
					break;
				case anmar.SharpWebMail.EmailClientCommand.Message:
					if ( args.Length==1 )
						command = System.String.Format("{0} FETCH {1}:{1} (BODY.PEEK[])", this.tag, args[0]);
					break;
				case anmar.SharpWebMail.EmailClientCommand.Status:
					command = System.String.Format("{0} EXAMINE {1}", this.tag, this.folder);
					break;
				
			}
			return command;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="args"></param>
		/// <returns></returns>
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
					responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.Login:
					break;
				case anmar.SharpWebMail.EmailClientCommand.Logout:
					responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.Message:
					responseSL = false;
					break;
				case anmar.SharpWebMail.EmailClientCommand.Status:
					responseSL = false;
					break;
			}
			return responseSL;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected override bool delete ( int mindex ) {
			if ( !this.select() ) {
				return false;
			}
			return base.delete(mindex);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		protected override bool deletemessages ( System.Data.DataRow[] result ) {
			bool error = !base.deletemessages( result );
			if ( !error ) {
				this.randomTag();
				System.IO.MemoryStream response = new System.IO.MemoryStream();
				System.String cmd = System.String.Format("{0} EXPUNGE ", this.tag);
				error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Other, cmd, response, false );
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool evaluateresponse ( System.String response ) {
			bool error = false;
			if ( response.IndexOf(' ')>0 ) {
				error = !response.Substring(response.IndexOf(' ')+1).ToLower().Trim().StartsWith("ok");
			} else {
				error = true;
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override System.IO.MemoryStream getStreamDataPortion ( System.IO.MemoryStream data ) {
			System.IO.StreamReader reader = new System.IO.StreamReader(data, System.Text.ASCIIEncoding.ASCII);
			System.String line = reader.ReadLine();
			if ( line.StartsWith("*") ) {
				int size = this.parseInteger((line.Substring(line.LastIndexOf('{'))).Trim(new Char[]{'{','}'}));
				if ( size>0 ) {
					int offset = System.Text.ASCIIEncoding.ASCII.GetByteCount(line + "\r\n");
					reader.DiscardBufferedData();
					reader=null;
					data.Seek(offset, System.IO.SeekOrigin.Begin);
					data.SetLength(offset + size);
				}
			}
			return data;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <param name="nlines"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool header ( int mindex, ulong nlines, System.IO.MemoryStream response ) {
			if ( !this.select() ) {
				return false;
			}
			return base.header(mindex, nlines, response);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		protected override bool list ( System.Int32[] list ) {
			if ( !this.select() ) {
				return false;
			}
			return base.list(list);
		}
		private int parseInteger ( System.String value ) {
			try {
				return System.Int32.Parse(value);
			} catch ( System.Exception ) {
				return -1;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="SLReponse"></param>
		protected override void parseLastResponse ( System.IO.MemoryStream response, bool SLReponse ) {
			if ( !SLReponse ) {
				response.Seek (0, System.IO.SeekOrigin.Begin);
				System.IO.StreamReader resp = new System.IO.StreamReader(response);
				System.String line = System.String.Empty;
				for ( System.String tmp=resp.ReadLine(); tmp!=null ; line=tmp, tmp=resp.ReadLine() ) {}
				this.lastResponse=line;
				response.Seek (0, System.IO.SeekOrigin.Begin);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool parseListSize ( System.Int32[] list, System.IO.MemoryStream response ) {
			System.Object[] tmplist = new System.Object[list.Length];
			bool error = !this.parseUntaggedResponse(response, tmplist, "RFC822.SIZE", anmar.SharpWebMail.EmailClientCommand.ListSize);
			if ( !error )
				tmplist.CopyTo(list, 0);
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="response"></param>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected override bool parseListUID ( System.String[] list, System.IO.MemoryStream response, int mindex ) {
			return this.parseUntaggedResponse(response, list, "UID", anmar.SharpWebMail.EmailClientCommand.ListUID);
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
			System.Object[] tmplist = new System.Object[1];
			error = !this.parseUntaggedResponse(response, tmplist, " EXISTS", anmar.SharpWebMail.EmailClientCommand.Status);
			if ( !error ) {
				numbytes = 0;
				num = (int)tmplist[0];
			}
			return !error;
		}
		private bool parseUntaggedResponse ( System.IO.MemoryStream response, System.Object[] list, System.String token, anmar.SharpWebMail.EmailClientCommand cmd ) {
			bool error = false;
			int msgnum=-1;
			System.Object value=-1;
			System.IO.StreamReader resp = new System.IO.StreamReader(response);
			for ( System.String line=resp.ReadLine(); line!=null; line=resp.ReadLine(), msgnum=-1, value=-1 ) {
				if ( line.StartsWith("*") && line.IndexOf(token)>0 ) {
					String[] values = line.Split( new char[]{' ', '(', ')'} );
					for ( int i=0; i<values.Length; i++ ) {
						if ( values[i].Equals("*") ) {
							msgnum = this.parseInteger(values[++i]);
						} else if ( values[i].Equals(token) ) {
							if  ( cmd.Equals(anmar.SharpWebMail.EmailClientCommand.ListSize) )
								value = this.parseInteger(values[++i]);
							else
								value = values[++i];
						}
					}
					if ( ( cmd.Equals(anmar.SharpWebMail.EmailClientCommand.ListUID) || cmd.Equals(anmar.SharpWebMail.EmailClientCommand.ListSize) )
						&& msgnum>0 && list.Length>=msgnum ) {
						list[msgnum-1]=value;
					} else if ( cmd.Equals(anmar.SharpWebMail.EmailClientCommand.Status) && msgnum>=0 ) {
						list[0] = msgnum;
					} else if ( log.IsErrorEnabled ) {
						log.Error ( "Error while parsing response line:" + line);
						error = true;
					}
				}
			}
			return !error;
		}
		private void randomTag () {
			this.tag = System.String.Format ("swm{0}", (int)(this.taggen.NextDouble()*10000));
			this.responseEnd = this.tag + " ";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mindex"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override bool retrieve ( int mindex, System.IO.MemoryStream response ) {
			if ( !this.select() ) {
				return false;
			}
			return base.retrieve(mindex, response);
		}
		private bool select () {
			bool error = false;
			if ( !this.selected ) {
				this.randomTag();
				System.IO.MemoryStream response = new System.IO.MemoryStream();
				System.String cmd = System.String.Format("{0} SELECT {1}", this.tag, this.folder);
				error = ( cmd.Equals(System.String.Empty) )?true:!this.sendCommand( anmar.SharpWebMail.EmailClientCommand.Other, cmd, response, false );
				if ( !error )
					this.selected = true;
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="mindex"></param>
		/// <returns></returns>
		protected override bool uidl ( System.String[] list, int mindex ) {
			if ( !this.select() ) {
				return false;
			}
			return base.uidl(list, mindex);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override bool quit () {
			this.selected = false;
			return base.quit();
		}
	}
}
