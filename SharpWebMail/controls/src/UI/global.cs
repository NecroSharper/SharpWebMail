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
		protected static System.Globalization.CultureInfo invariant = null;
		protected static System.Collections.Specialized.HybridDictionary availablecultures;
		protected static System.Resources.ResourceManager resources = null;
		private System.Collections.Hashtable configOptions = System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable();

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
			Session["effectiveculture"] = getEffectiveCulture(System.Threading.Thread.CurrentThread.CurrentCulture);
			Session["resources"] = resources.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentUICulture, true, true);
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
					if ( dir.Exists )
						dir.Delete(true);
					dir=null;
				}
			} catch( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error cleanling up dir", e);
			}
		}
		public int getEffectiveCulture ( System.Globalization.CultureInfo ci ) {
			int culture = 127;
			if ( !availablecultures.Contains(ci.LCID) && !ci.Equals(System.Globalization.CultureInfo.InvariantCulture) )
				culture = this.getEffectiveCulture(ci.Parent);
			else
				culture = ci.LCID;
			return culture;
		}
		private void initConfig () {
			this.configOptions.Add ( "sharpwebmail/general/default_lang", "en" );
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

			resources = new System.Resources.ResourceManager("SharpWebMail", System.Reflection.Assembly.GetExecutingAssembly());
			Application["resources"] = resources;
			System.Collections.Specialized.ListDictionary config = (System.Collections.Specialized.ListDictionary)System.Configuration.ConfigurationSettings.GetConfig("sharpwebmail");
			initConfigSection("sharpwebmail/general", config);
			initConfigSection("sharpwebmail/login", config);
			initConfigSection("sharpwebmail/read/inbox", config);
			initConfigSection("sharpwebmail/read/message", config);
			initConfigSection("sharpwebmail/send/message", config);
			if ( config.Contains("sharpwebmail/send/addressbook") )
			    Application.Add ("sharpwebmail/send/addressbook", config["sharpwebmail/send/addressbook"]);

			// Set defaults for unset config options
			foreach ( System.String item in this.configOptions.Keys ) {
				if ( Application[item]==null )
					Application[item]=this.configOptions[item];
			}
			parseConfigServers ("sharpwebmail/read/servers", config);
			parseConfigServers ("sharpwebmail/send/servers", config);

			TestAvailableCultures();
			Application["AvailableCultures"] = new System.Collections.SortedList(availablecultures);
			initInvariantCulture();

			Application["sharpwebmail/read/message/temppath"] = parseTempFolder(Server.MapPath("/"), Application["sharpwebmail/read/message/temppath"]);
			Application["sharpwebmail/send/message/temppath"] = parseTempFolder(Server.MapPath("/"), Application["sharpwebmail/send/message/temppath"]);
		}
		private void initConfigSection ( System.String section, System.Collections.Specialized.ListDictionary globalconfig ) {
			System.Collections.Hashtable config = null;
			if ( globalconfig.Contains(section) )
				config = (System.Collections.Hashtable)globalconfig[section];
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
				if ( defaultvalue.GetType().Equals(typeof(int)) )
					return parseConfigElement(value.ToString(), (int)defaultvalue);
				else if ( defaultvalue.GetType().Equals(typeof(System.String)) )
					return (value==null)?defaultvalue:value;
			}
			return value;
		}
		private void initInvariantCulture() {
			if ( invariant==null )
				ParseInvariant(Application["sharpwebmail/general/default_lang"].ToString());
			if ( invariant==null )
				ParseInvariant("en");
			if ( invariant==null )
				invariant = System.Globalization.CultureInfo.InvariantCulture;
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
		private void parseConfigServers ( System.String config_item, System.Collections.Specialized.ListDictionary globalconfig ) {
			System.Collections.Specialized.NameValueCollection config = null;
			if ( globalconfig.Contains(config_item) )
				config = (System.Collections.Specialized.NameValueCollection)globalconfig[config_item];
			anmar.SharpWebMail.ServerSelector selector = new anmar.SharpWebMail.ServerSelector();
			if ( config!=null ) {
				foreach ( System.String item in config.Keys ) {
					selector.Add(item, config[item]);
				}
			}
			Application.Add(config_item, selector);
		}
		private System.Globalization.CultureInfo ParseCulture ( System.String culturename ) {
			System.Globalization.CultureInfo culture = null;
			try {
				culture = new System.Globalization.CultureInfo(culturename);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error parsing culture", e);
			}
			return culture;
		}
		private System.Globalization.CultureInfo ParseCultureSpecific ( System.String culturename ) {
			if ( culturename.IndexOf(';')>0 )
				culturename = culturename.Remove(culturename.IndexOf(';'), culturename.Length - culturename.IndexOf(';'));
			System.Globalization.CultureInfo culture = null;
			try {
				culture = System.Globalization.CultureInfo.CreateSpecificCulture(culturename);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error parsing specific culture", e);
			}
			return culture;
		}
		private System.Globalization.CultureInfo ParseCultures ( System.String[] cultures ) {
			System.Globalization.CultureInfo culture = null;
			if ( cultures!=null ) {
				foreach ( System.String item in cultures ) {
					culture = this.ParseCultureSpecific(item);
					if ( culture!=null && getEffectiveCulture(culture)!=127 )
						break;
					else
						culture = null;
				}
			}
			if ( culture==null )
				culture = invariant;
			
			return culture;
		}
		private void ParseInvariant ( System.String culture ) {
			invariant = ParseCulture(culture);
			if ( invariant!=null ) {
				if ( availablecultures.Contains(invariant.LCID) )
					invariant = ParseCultureSpecific(culture);
				else
					invariant = null;
			}
		}
		private System.String parseTempFolder( System.Object prefix, System.Object sufix ) {
			// Temp folder
			if ( prefix!=null && sufix!=null && !prefix.Equals(System.String.Empty) && !sufix.Equals(System.String.Empty) )
				return System.IO.Path.Combine (prefix.ToString(), sufix.ToString());
			else
				return null;
		}
		private void TestAvailableCultures() {
			availablecultures = new System.Collections.Specialized.HybridDictionary();
			foreach ( System.Globalization.CultureInfo item in System.Globalization.CultureInfo.GetCultures( System.Globalization.CultureTypes.AllCultures) )  {
				if ( !item.Equals(System.Globalization.CultureInfo.InvariantCulture) && !availablecultures.Contains(item.LCID) && resources.GetResourceSet(item, true, false)!=null )
					availablecultures.Add(item.LCID, item.EnglishName);
			}
		}
	}
}
