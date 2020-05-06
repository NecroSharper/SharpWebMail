// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2006 Angel Marin
// 
//   This file is part of SharpWebMail.
//
//   SharpWebMail is free software; you can redistribute it and/or
//   modify it under the terms of the GNU Lesser General Public
//   License as published by the Free Software Foundation; either
//   version 2.1 of the License, or (at your option) any later version.
//
//   SharpWebMail is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//   Lesser General Public License for more details.
//
//   You should have received a copy of the GNU Lesser General Public
//   License along with SharpWebMail; if not, write to the Free Software
//   Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
// -----------------------------------------------------------------------

using System;

namespace anmar.SharpWebMail.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class EmailServerInfo {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private System.Text.RegularExpressions.Regex _condition = null;
		private anmar.SharpWebMail.Config.EmailServerProtocol _protocol;
		private System.String _host;
		private System.String _name = null;
		private int _port;

		public EmailServerInfo ( anmar.SharpWebMail.Config.EmailServerProtocol protocol, System.String host, int port ) {
			this._protocol = protocol;
			this._host = host;
			this._port = port;
		}
		public EmailServerInfo ( System.String protocol, System.String host, System.String port ) {
			this._protocol = anmar.SharpWebMail.Config.EmailServerInfo.ParseProtocol(protocol);
			this._host = anmar.SharpWebMail.Config.EmailServerInfo.ParseHost(host);
			this._port = anmar.SharpWebMail.Config.EmailServerInfo.ParsePort(port, this._protocol);
		}
		public System.Text.RegularExpressions.Regex Condition {
			get {
				return this._condition;
			}
		}
		public System.String Host {
			get {
				return this._host;
			}
		}
		public System.String Name {
			get {
				return this._name;
			}
			set {
				this._name = value;
			}
		}
		public int Port {
			get {
				return this._port;
			}
		}
		public anmar.SharpWebMail.Config.EmailServerProtocol Protocol {
			get {
				return this._protocol;
			}
		}
		private static int GetDefaultPort ( anmar.SharpWebMail.Config.EmailServerProtocol protocol ) {
			switch ( protocol ) {
				case anmar.SharpWebMail.Config.EmailServerProtocol.Imap:
					return 143;
				case anmar.SharpWebMail.Config.EmailServerProtocol.Pop3:
					return 110;
				case anmar.SharpWebMail.Config.EmailServerProtocol.Smtp:
					return 25;
			}
			return 0;
		}
		public bool IsValid () {
			return ( !this._protocol.Equals(anmar.SharpWebMail.Config.EmailServerProtocol.Unknown) && this._port>0 && this._host!=null && (this._condition!=null || this._name!=null) );
		}
		public static anmar.SharpWebMail.Config.EmailServerInfo Parse ( System.String value ) {
			anmar.SharpWebMail.Config.EmailServerInfo server = null;
			System.String[] values = value.ToString().Split(':');
			if ( values.Length==3 ) {
				anmar.SharpWebMail.Config.EmailServerProtocol protocol = anmar.SharpWebMail.Config.EmailServerInfo.ParseProtocol(values[0]);
				System.String host = anmar.SharpWebMail.Config.EmailServerInfo.ParseHost(values[1]);
				int port = anmar.SharpWebMail.Config.EmailServerInfo.ParsePort(values[2], protocol);
				if ( !protocol.Equals(anmar.SharpWebMail.Config.EmailServerProtocol.Unknown) && port>0 && host!=null ) {
					server = new anmar.SharpWebMail.Config.EmailServerInfo(protocol, host, port);
				}
			}
			return server;
		}
		private static System.String ParseHost ( System.String value ) {
			try {
				System.Net.Dns.GetHostEntry(value);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing host: {0}", value), e);
				return null;
			}
			return value;

		}
		private static int ParsePort ( System.String value, anmar.SharpWebMail.Config.EmailServerProtocol protocol ) {
			int port;
			try {
				port = System.Int32.Parse(value);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing port: {0}", value), e);
				port = anmar.SharpWebMail.Config.EmailServerInfo.GetDefaultPort(protocol);
			}
			return port;
		}
		private static anmar.SharpWebMail.Config.EmailServerProtocol ParseProtocol ( System.String value ) {
			anmar.SharpWebMail.Config.EmailServerProtocol protocol;
			try {
				protocol = (anmar.SharpWebMail.Config.EmailServerProtocol)System.Enum.Parse(typeof(anmar.SharpWebMail.Config.EmailServerProtocol), value, true);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing protocol: {0}", value), e);
				protocol = anmar.SharpWebMail.Config.EmailServerProtocol.Unknown;
			}
			return protocol;
		}
		public void SetCondition ( System.String pattern ) {
			try {
				if ( pattern.Equals("*") )
					pattern = ".*";
				this._condition = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.ECMAScript);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing pattern: {0}", pattern), e);
			}
		}
		public override System.String ToString () {
			if ( this._name!=null )
				return this._name;
			else
				return this._host;
		}
	}
}
