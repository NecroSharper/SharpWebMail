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
	public class ServerSelector {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private System.Collections.Specialized.HybridDictionary _servers;
		public ServerSelector () {
			this._servers = new System.Collections.Specialized.HybridDictionary();
		}
		public void Add ( System.Object key, System.Object value ) {
			if ( key==null || value ==null )
				throw new System.ArgumentNullException();
			
			System.Text.RegularExpressions.Regex condition = this.ParseCondition(key.ToString());
			anmar.SharpWebMail.EmailServer server = anmar.SharpWebMail.EmailServer.Parse(value.ToString());
			if ( condition!=null && server!=null )
				this._servers.Add (condition, server);
		}
		private System.Text.RegularExpressions.Regex ParseCondition ( System.String pattern ) {
			System.Text.RegularExpressions.Regex condition = null;
			try {
				if ( pattern.Equals("*") )
					pattern = ".*";
				condition = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.ECMAScript);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error(System.String.Format("Error parsing pattern: {0}", pattern), e);
			}
			return condition;
		}
		public anmar.SharpWebMail.EmailServer Select ( System.String key ) {
			foreach(System.Text.RegularExpressions.Regex item in this._servers.Keys ) {
				log.Error(item.IsMatch(key));
				if ( item.IsMatch(key) )
					return (anmar.SharpWebMail.EmailServer)this._servers[item];
			}
			return null;
		}
	}
}
