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

		public override void Init() {
		}

		public void Application_Start ( Object sender, EventArgs e ) {
			initConfig();
		}
		public void Application_End ( Object sender, EventArgs e ) {
		}

		public void Application_Error ( Object sender, EventArgs e ) {
#if !DEBUG
			Server.ClearError();
#endif
		}

		public void Session_Start ( Object sender, EventArgs e ) {
			// For each request initialize the culture values with the
			// user language as specified by the browser.
			try {
				System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
			} catch(Exception) {
				try {
					// provide fallback for not supported languages. This is really just a safety catch, for 'in-case' scenarios
					System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(System.Configuration.ConfigurationSettings.AppSettings["default_lang"]);
				} catch(Exception) {
					try {
						// provide fallback for not supported languages. This is really just a safety catch, for 'in-case' scenarios
						System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
					} catch(Exception) {
					}
				}
			}

			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;

			Session["resources"] = ((System.Resources.ResourceManager)Application["resources"]).GetResourceSet(System.Threading.Thread.CurrentThread.CurrentUICulture, true, true);
			// Inbox Object
			Session["inbox"] = new anmar.SharpWebMail.CTNInbox();

			// Temp folder
			if ( Application["temppath"]!=null ) {
				Session["temppath"] = System.IO.Path.Combine (Application["temppath"].ToString(), Session.SessionID);
			} else {
				Session["temppath"] = null;
			}

		}

		public void Session_End ( Object sender, EventArgs e ) {
			// Clean up temp files
			try {
				if ( Session["temppath"]!=null ) {
					System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo (Session["temppath"].ToString());
					dir.Delete(true);
					dir=null;
				}
			} catch( System.Exception ) {
			}
		}
		private void initConfig () {
			Application["product"] = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			Application["version"] = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

			Application["resources"] = new System.Resources.ResourceManager("SharpWebMail", System.Reflection.Assembly.GetExecutingAssembly());

			foreach ( System.String key in  System.Configuration.ConfigurationSettings.AppSettings.Keys ) {
				Application.Add (key, System.Configuration.ConfigurationSettings.AppSettings[key]);
			}

			parseConfig ("mail_server_pop3_port", 110);
			parseConfig ("mail_server_smtp_port", 25);
			parseConfig ("pagesize", 10);

			if ( Application["temppath"]!=null && Application["temppath"].ToString().Length>0) {
				Application["temppath"] = System.IO.Path.Combine (Server.MapPath("/"), Application["temppath"].ToString());
			} else {
				Application["temppath"] = null;
			}
		}
		private void parseConfig ( System.String key, System.Int32 dflt ) {
			try {
				Application[key] = System.Int32.Parse(Application[key].ToString());
			} catch ( System.Exception ) {
				Application[key] = dflt;
			}
		}
	}
}
