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

[assembly: log4net.Config.DOMConfigurator()]
namespace anmar.SharpWebMail.UI
{
	public class Global : System.Web.HttpApplication {
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private System.Collections.Hashtable configOptions = new System.Collections.Hashtable();

		public override void Init() {
		}

		public void Application_Start ( System.Object sender, System.EventArgs args ) {
			initConfig();
		}
		public void Application_End ( System.Object sender, System.EventArgs args ) {
		}

		public void Application_Error ( System.Object sender, System.EventArgs args ) {
			if ( log.IsErrorEnabled ) log.Error ( "Application_Error", Server.GetLastError() );
#if !DEBUG
			Server.ClearError();
#endif
		}

		public void Session_Start ( System.Object sender, System.EventArgs args ) {
			// For each request initialize the culture values with the
			// user language as specified by the browser.
			System.Threading.Thread.CurrentThread.CurrentCulture = this.ParseCultures ( Request.UserLanguages );
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;

			Session["resources"] = ((System.Resources.ResourceManager)Application["resources"]).GetResourceSet(System.Threading.Thread.CurrentThread.CurrentUICulture, true, true);
			// Inbox Object
			Session["inbox"] = new anmar.SharpWebMail.CTNInbox();

			Session["sharpwebmail/read/message/temppath"] = parseTempFolder(Application["sharpwebmail/read/message/temppath"], Session.SessionID);
			Session["sharpwebmail/send/message/temppath"] = parseTempFolder(Application["sharpwebmail/send/message/temppath"], Session.SessionID);
		}

		public void Session_End ( System.Object sender, System.EventArgs args ) {
			// Clean up temp files
			cleanTempFolder(Session["sharpwebmail/read/message/temppath"]);
			cleanTempFolder(Session["sharpwebmail/send/message/temppath"]);
		}
		private void cleanTempFolder ( System.Object value ) {
			try {
				if ( value!=null ) {
					System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo (value.ToString());
					dir.Delete(true);
					dir=null;
				}
			} catch( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error cleanling up dir", e);
			}
		}
		private void initConfig () {
			this.configOptions.Add ( "sharpwebmail/general/default_lang", "en-US" );
			this.configOptions.Add ( "sharpwebmail/general/title", "" );
			this.configOptions.Add ( "sharpwebmail/login/append", "" );
			this.configOptions.Add ( "sharpwebmail/login/mode", 1 );
			this.configOptions.Add ( "sharpwebmail/login/title", "" );
			this.configOptions.Add ( "sharpwebmail/read/inbox/pagesize", 10 );
			this.configOptions.Add ( "sharpwebmail/read/inbox/stat", 2 );
			this.configOptions.Add ( "sharpwebmail/read/message/sanitizer_mode", 0 );
			this.configOptions.Add ( "sharpwebmail/read/message/temppath", "" );
			this.configOptions.Add ( "sharpwebmail/send/message/sanitizer_mode", 0 );
			this.configOptions.Add ( "sharpwebmail/send/message/temppath", "" );

			Application["product"] = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			Application["version"] = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

			Application["resources"] = new System.Resources.ResourceManager("SharpWebMail", System.Reflection.Assembly.GetExecutingAssembly());

			initConfigSection("sharpwebmail/general");
			initConfigSection("sharpwebmail/login");
			initConfigSection("sharpwebmail/read/inbox");
			initConfigSection("sharpwebmail/read/message");
			initConfigSection("sharpwebmail/send/message");

			// Set defaults for unset config options
			foreach ( System.String item in this.configOptions.Keys ) {
				if ( Application[item]==null )
					Application[item]=this.configOptions[item];
			}
			parseConfigServers ("sharpwebmail/read/servers");
			parseConfigServers ("sharpwebmail/send/servers");

			Application["sharpwebmail/read/message/temppath"] = parseTempFolder(Server.MapPath("/"), Application["sharpwebmail/read/message/temppath"]);
			Application["sharpwebmail/send/message/temppath"] = parseTempFolder(Server.MapPath("/"), Application["sharpwebmail/send/message/temppath"]);
		}
		private void initConfigSection ( System.String section ) {
			System.Collections.Hashtable config = (System.Collections.Hashtable)System.Configuration.ConfigurationSettings.GetConfig(section);
			if ( config!=null ) {
				foreach ( System.String item in config.Keys ) {
					System.String config_item = System.String.Format("{0}/{1}", section, item);
					Application.Add (config_item, initConfigElement(config_item, config[item]));
				}
			}
		}
		private System.Object initConfigElement ( System.String config_item, System.Object value ) {
			if ( this.configOptions.ContainsKey(config_item) ) {
				System.Object defaultvalue = this.configOptions[config_item];
				if ( defaultvalue.GetType().Equals(typeof(int)) ) {
					return parseConfigElement(value.ToString(), (int)defaultvalue);
				} else if ( defaultvalue.GetType().Equals(typeof(System.String)) ) {
					return (value==null)?defaultvalue:value;
				}
			}
			return value;
		}
		private System.Object parseConfigElement ( System.String value, System.Int32 defaultvalue ) {
			try {
				return System.Int32.Parse(value);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error parsing integer", e);
				return defaultvalue;
			}
		}
		private void parseConfigServers ( System.String config_item ) {
			System.Collections.Specialized.NameValueCollection config = (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationSettings.GetConfig(config_item);
			anmar.SharpWebMail.ServerSelector selector = new anmar.SharpWebMail.ServerSelector();
			if ( config!=null ) {
				foreach ( System.String item in config.Keys ) {
					selector.Add(item, config[item]);
				}
			}
			Application.Add(config_item, selector);
		}
		private System.Globalization.CultureInfo ParseCulture ( System.String culturename ) {
			if ( culturename.IndexOf(';')>0 )
				culturename = culturename.Remove(culturename.IndexOf(';'), culturename.Length - culturename.IndexOf(';'));
			System.Globalization.CultureInfo culture = null;
			try {
				culture = System.Globalization.CultureInfo.CreateSpecificCulture(culturename);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error parsing culture", e);
			}
			return culture;
		}
		private System.Globalization.CultureInfo ParseCultures ( System.String[] cultures ) {
			System.Globalization.CultureInfo culture = null;
			foreach ( System.String item in cultures ) {
				culture = this.ParseCulture(item);
				if ( culture!=null )
					break;
			}
			if ( culture==null )
				culture = ParseCulture(Application["sharpwebmail/general/default_lang"].ToString());
			if ( culture==null )
				culture = ParseCulture("en-US");
			if ( culture==null )
				culture = ParseCulture("");
			return culture;
		}
		private System.String parseTempFolder( System.Object prefix, System.Object sufix ) {
			// Temp folder
			if ( prefix!=null && sufix!=null && !prefix.Equals("") && !sufix.Equals("") ) {
				return System.IO.Path.Combine (prefix.ToString(), sufix.ToString());
			} else {
				return null;
			}
		}
	}
}
