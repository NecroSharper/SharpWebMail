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

namespace anmar.SharpWebMail.UI
{
	public class Login : System.Web.UI.Page {

		// General variables
		private System.Resources.ResourceSet rm;

		// Labels
		protected System.Web.UI.WebControls.Label errorMsgLogin;
		protected System.Web.UI.WebControls.Label loginWindowPassword;
		protected System.Web.UI.WebControls.Label loginWindowTitle;
		protected System.Web.UI.WebControls.Label loginWindowUsername;

		// Input boxes
		protected System.Web.UI.HtmlControls.HtmlInputText username;
		protected System.Web.UI.HtmlControls.HtmlInputText password;

		//Other form elements
		protected System.Web.UI.HtmlControls.HtmlForm LoginForm;
		protected System.Web.UI.WebControls.Button Button1;

		protected void Login_Click(System.Object sender, System.EventArgs E) {
    
			// authenticate user: this samples accepts only one user with
			// a name of jdoe@somewhere.com and a password of 'password'
			if (this.IsPostBack&&this.IsValid) {
				anmar.SharpWebMail.CTNSimplePOP3Client client = new anmar.SharpWebMail.CTNSimplePOP3Client (Application["mail_server_pop3"].ToString(), (int) Application["mail_server_pop3_port"], username.Value, password.Value );
				anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
				if ( client.getInboxIndex ( inbox, 0, (int) Application["pagesize"], true ) ) {
					System.Web.Security.FormsAuthentication.RedirectFromLoginPage(username.Value, false);
					Session["client"] = client;
					Session["inbox"] = inbox;
				} else {
					errorMsgLogin.Visible=true;
				}
				client = null;
				inbox = null;
			}
		}

		protected void Page_Load(System.Object Src, System.EventArgs E ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			//Localized resources for this session
			rm = (System.Resources.ResourceSet) Session["resources"];

			// Set labels localized texts
			loginWindowTitle.Text = rm.GetString("loginWindowTitle") + ": " + System.Configuration.ConfigurationSettings.AppSettings["system_name"];
            loginWindowUsername.Text = rm.GetString("loginWindowUsername");
			loginWindowPassword.Text = rm.GetString("loginWindowPassword");
			errorMsgLogin.Text = rm.GetString("errorMsgLogin");

			rm = null;
		}

	}
}
