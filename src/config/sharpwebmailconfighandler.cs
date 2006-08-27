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
	public class SharpWebMailConfigHandler : System.Configuration.IConfigurationSectionHandler {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public virtual System.Object Create( System.Object parent, System.Object context, System.Xml.XmlNode section ) {
			System.Collections.Hashtable config = System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable();
			InitConfigDefaults(config);
			ParseNode(parent, context, section, config, "sharpwebmail");
			return config;
		}
		private void ParseNode ( System.Object parent, System.Object context, System.Xml.XmlNode node, System.Collections.Hashtable config, System.String prefix ) {
			foreach ( System.Xml.XmlNode item in node.ChildNodes ) {
				if ( item.NodeType.Equals(System.Xml.XmlNodeType.Element) ) {
					System.String sectionname = System.String.Concat(prefix, "/", item.Name);;
					switch ( item.Name ) {
						case "general":
						case "login":
						case "message":
						case "inbox":
							System.Configuration.SingleTagSectionHandler singlesection = new System.Configuration.SingleTagSectionHandler();
							InitConfigSection(config, sectionname, singlesection.Create(parent, context, item) as System.Collections.Hashtable);
							break;
						case "read":
						case "send":
							if ( item.HasChildNodes )
								ParseNode( parent, context, item, config, sectionname );
							break;
						case "servers":
							if ( item.HasChildNodes )
								config.Add(sectionname, ParseConfigServers(item.ChildNodes));
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
									addressbook["pagesize"] = ParseConfigElement(addressbook["pagesize"].ToString(), 10);
								else
									addressbook["pagesize"] = 10;
								addressbooks.Add(addressbook["name"], addressbook);
								if ( addressbook.Contains("allowupdate") )
									addressbook["allowupdate"] = ParseConfigElement(addressbook["allowupdate"].ToString(), false);
								else
									addressbook["allowupdate"] = false;
							}
							break;
					}
				}
			}
		}
		private void InitConfigDefaults (System.Collections.Hashtable config) {
			config.Add ( "sharpwebmail/general/addressbooks", false );
			config.Add ( "sharpwebmail/general/default_lang", "en" );
			config.Add ( "sharpwebmail/general/title", System.String.Empty );
			config.Add ( "sharpwebmail/login/append", System.String.Empty );
			config.Add ( "sharpwebmail/login/enablequerystringlogin", false );
			config.Add ( "sharpwebmail/login/mode", 1 );
			config.Add ( "sharpwebmail/login/serverselection", System.String.Empty );
			config.Add ( "sharpwebmail/login/title", System.String.Empty );
			config.Add ( "sharpwebmail/read/inbox/commit_onexit", true );
			config.Add ( "sharpwebmail/read/inbox/commit_ondelete", false );
			config.Add ( "sharpwebmail/read/inbox/pagesize", 10 );
			config.Add ( "sharpwebmail/read/inbox/sort", "msgnum DESC" );
			config.Add ( "sharpwebmail/read/inbox/stat", 2 );
			config.Add ( "sharpwebmail/read/message/commit_ondelete", false );
			config.Add ( "sharpwebmail/read/message/sanitizer_mode", 0 );
			config.Add ( "sharpwebmail/read/message/temppath", System.String.Empty );
			config.Add ( "sharpwebmail/send/message/attach_ui", "normal" );
			config.Add ( "sharpwebmail/send/message/charset", System.Text.Encoding.GetEncoding("iso-8859-1") );
			config.Add ( "sharpwebmail/send/message/forwardattachments", true );
			config.Add ( "sharpwebmail/send/message/replyquotechar", "> " );
			config.Add ( "sharpwebmail/send/message/replyquotestyle", "padding-left: 5px; margin-left: 5px; border-left: #0000ff 2px solid; margin-left: 0px" );
			config.Add ( "sharpwebmail/send/message/sanitizer_mode", 0 );
			config.Add ( "sharpwebmail/send/message/smtp_engine", System.String.Empty );
			config.Add ( "sharpwebmail/send/message/temppath", System.String.Empty );
			config.Add ( "sharpwebmail/read/message/useserverencoding", false );
		}
		private void InitConfigSection ( System.Collections.Hashtable config, System.String section, System.Collections.Hashtable configsection ) {
			foreach ( System.Collections.DictionaryEntry item in configsection ) {
				System.String config_item = System.String.Concat(section, "/", item.Key);
				config[config_item] = ParseConfigElement(item.Value.ToString(), config[config_item]);
			}
		}
		private System.Object ParseConfigElement ( System.String value, System.Object defaultvalue ) {
			if ( value==null )
				return defaultvalue;
			try {
				if ( defaultvalue.GetType().Equals(typeof(int)) )
					return System.Int32.Parse(value);
				else if ( defaultvalue.GetType().Equals(typeof(bool)) )
					return System.Boolean.Parse(value);
				else if ( typeof(System.Text.Encoding).IsAssignableFrom(defaultvalue.GetType()) )
					return System.Text.Encoding.GetEncoding(value);
				else
					return value;
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error parsing value", e);
				return defaultvalue;
			}
		}
		private anmar.SharpWebMail.Config.ServerSelector ParseConfigServers ( System.Xml.XmlNodeList list ) {
			anmar.SharpWebMail.Config.ServerSelector selector = new anmar.SharpWebMail.Config.ServerSelector();
			foreach ( System.Xml.XmlNode item in list ) {
				if ( item.NodeType.Equals(System.Xml.XmlNodeType.Element) && (item.LocalName.Equals("server") || item.LocalName.Equals("add")) ) {
					System.Xml.XmlElement element = (System.Xml.XmlElement)item;
					if ( element.HasAttribute("key") && element.HasAttribute("value") ) // Old format
						selector.Add(element.GetAttribute("key"), element.GetAttribute("value"));
					else if ( element.HasAttribute("protocol") && element.HasAttribute("host") && element.HasAttribute("port") ) { // New format
						anmar.SharpWebMail.Config.EmailServerInfo server = new anmar.SharpWebMail.Config.EmailServerInfo(element.GetAttribute("protocol"), element.GetAttribute("host"), element.GetAttribute("port"));
						if ( element.HasAttribute("regexp") )
							server.SetCondition (element.GetAttribute("regexp"));
						if ( element.HasAttribute("name") )
							server.Name = element.GetAttribute("name");

						if ( server.IsValid() )
							selector.Add(server);
					}
				}
			}
			return selector;
		}
	}
}
