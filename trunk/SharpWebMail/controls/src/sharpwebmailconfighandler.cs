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
	public class SharpWebMailConfigHandler : System.Configuration.IConfigurationSectionHandler {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public virtual System.Object Create( System.Object parent, System.Object context, System.Xml.XmlNode section ) {
			System.Collections.Specialized.ListDictionary config = new System.Collections.Specialized.ListDictionary();
			ParseNode(parent, context, section, config, "sharpwebmail");
			return config;
		}
		private void ParseNode ( System.Object parent, System.Object context, System.Xml.XmlNode node, System.Collections.Specialized.ListDictionary config, System.String prefix ) {
			foreach ( System.Xml.XmlNode item in node.ChildNodes ) {
				if ( item.NodeType.Equals(System.Xml.XmlNodeType.Element) ) {
					System.String sectionname = System.String.Concat(prefix, "/", item.Name);;
					switch ( item.Name ) {
						case "general":
						case "login":
						case "message":
						case "inbox":
							System.Configuration.SingleTagSectionHandler singlesection = new System.Configuration.SingleTagSectionHandler();
							config.Add(sectionname, singlesection.Create(parent, context, item));
							break;
						case "read":
						case "send":
							if ( item.HasChildNodes )
								ParseNode( parent, context, item, config, sectionname );
							break;
						case "servers":
							System.Configuration.NameValueSectionHandler namevaluesection = new System.Configuration.NameValueSectionHandler();
							config.Add(sectionname, namevaluesection.Create(parent, context, item));
							break;
						case "addressbook":
							if ( !config.Contains(sectionname) )
								config.Add(sectionname, new System.Collections.SortedList());
							System.Collections.SortedList addressbooks = (System.Collections.SortedList)config[sectionname];
							System.Collections.Hashtable tmpaddressbook = (System.Collections.Hashtable)(new System.Configuration.SingleTagSectionHandler()).Create(parent, context, item);
							System.Collections.Specialized.ListDictionary addressbook = new System.Collections.Specialized.ListDictionary(new System.Collections.CaseInsensitiveComparer());
							foreach ( System.String configitem in tmpaddressbook.Keys) {
								addressbook.Add(configitem, tmpaddressbook[configitem]);
							}
							tmpaddressbook = null;
							if ( addressbook.Contains("type") && !addressbook["type"].Equals("none") && addressbook.Contains("name") && !addressbooks.Contains(addressbook["name"]) ) {
								if ( addressbook.Contains("pagesize") )
									addressbook["pagesize"] = System.Int32.Parse(addressbook["pagesize"].ToString());
								else
									addressbook["pagesize"] = 10;
								addressbooks.Add(addressbook["name"], addressbook);
							}
							break;
					}
				}
			}
		}
	}
}
