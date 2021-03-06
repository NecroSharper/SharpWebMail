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
	public class ServerSelector {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private System.Collections.ArrayList _servers;
		public ServerSelector () {
			this._servers = new System.Collections.ArrayList();
		}
		public System.Collections.ICollection Servers {
			get {
				return this._servers;
			}
		}
		public void Add ( System.Object key, System.Object value ) {
			if ( key==null || value ==null )
				throw new System.ArgumentNullException();

			anmar.SharpWebMail.Config.EmailServerInfo server = anmar.SharpWebMail.Config.EmailServerInfo.Parse(value.ToString());
			if ( server!=null ) {
				server.SetCondition(key.ToString());
				if ( server.IsValid() )
					this._servers.Add (server);
			}
		}
		public void Add ( anmar.SharpWebMail.Config.EmailServerInfo server ) {
			if ( server==null || !server.IsValid() )
				throw new System.ArgumentNullException();
			this._servers.Add (server);
		}
		public anmar.SharpWebMail.Config.EmailServerInfo Select ( System.String key, bool match ) {
			foreach( anmar.SharpWebMail.Config.EmailServerInfo item in this._servers ) {
				if ( item.Condition!=null && match ) {
					if ( item.Condition.IsMatch(key) ) {
						if ( log.IsDebugEnabled )
							log.Debug (System.String.Concat("[", item.Name, "] selected for condicion [", item.Condition, "] and input [", key, "]" ));
						return item;
					}
				} else if ( !match && item.Name!=null && item.Name.Equals(key) ) {
					if ( log.IsDebugEnabled )
						log.Debug (System.String.Concat("[", item.Name, "] selected for input [", key, "]" ));
					return item;
				}
			}
			return null;
		}
	}
}
