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
	public class EmailServerInfo {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private anmar.SharpWebMail.ServerProtocol _protocol;
		private System.String _host;
		private int _port;

		public EmailServerInfo ( anmar.SharpWebMail.ServerProtocol protocol, System.String host, int port ) {
			this._protocol = protocol;
			this._host = host;
			this._port = port;
		}
		public System.String Host {
			get {
				return this._host;
			}
		}
		public int Port {
			get {
				return this._port;
			}
		}
		public anmar.SharpWebMail.ServerProtocol Protocol {
			get {
				return this._protocol;
			}
		}
		private static int GetDefaultPort ( anmar.SharpWebMail.ServerProtocol protocol ) {
			switch ( protocol ) {
				case anmar.SharpWebMail.ServerProtocol.Imap:
					return 143;
				case anmar.SharpWebMail.ServerProtocol.Pop3:
					return 110;
				case anmar.SharpWebMail.ServerProtocol.Smtp:
					return 25;
			}
			return 0;
		}
		public static anmar.SharpWebMail.EmailServerInfo Parse ( System.String value ) {
			anmar.SharpWebMail.EmailServerInfo server = null;
			System.String[] values = value.ToString().Split(':');
			if ( values.Length==3 ) {
				anmar.SharpWebMail.ServerProtocol protocol = anmar.SharpWebMail.EmailServerInfo.ParseProtocol(values[0]);
				System.String host = anmar.SharpWebMail.EmailServerInfo.ParseHost(values[1]);
				int port = anmar.SharpWebMail.EmailServerInfo.ParsePort(values[2], protocol);
				if ( !protocol.Equals(anmar.SharpWebMail.ServerProtocol.Unknown) && port>0 && host!=null ) {
					server = new anmar.SharpWebMail.EmailServerInfo(protocol, host, port);
				}
			}
			return server;
		}
		private static System.String ParseHost ( System.String value ) {
			try {
				System.Net.Dns.Resolve(value);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing host: {0}", value), e);
				return null;
			}
			return value;

		}
		private static int ParsePort ( System.String value, anmar.SharpWebMail.ServerProtocol protocol ) {
			int port;
			try {
				port = System.Int32.Parse(value);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing port: {0}", value), e);
				port = anmar.SharpWebMail.EmailServerInfo.GetDefaultPort(protocol);
			}
			return port;
		}
		private static anmar.SharpWebMail.ServerProtocol ParseProtocol ( System.String value ) {
			anmar.SharpWebMail.ServerProtocol protocol;
			try {
				protocol = (anmar.SharpWebMail.ServerProtocol)System.Enum.Parse(typeof(anmar.SharpWebMail.ServerProtocol), value, true);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing protocol: {0}", value), e);
				protocol = anmar.SharpWebMail.ServerProtocol.Unknown;
			}
			return protocol;
		}
	}
	/// <summary>
	/// 
	/// </summary>	
	public enum ServerProtocol {
		/// <summary>
		/// IMAP. Read RFC 3501
		/// </summary>
		Imap,
		/// <summary>
		/// POP3. Read RFC 3461
		/// </summary>
		Pop3,
		/// <summary>
		/// SMTP. Read RFC 2821
		/// </summary>
		Smtp,
		/// <summary>
		/// SMTP combined with SMTP AUTH
		/// </summary>
		SmtpAuth,
		/// <summary>
		/// Unknown protocol
		/// </summary>
		Unknown
	}
}
