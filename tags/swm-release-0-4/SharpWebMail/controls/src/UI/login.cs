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

		// Labels
		protected System.Web.UI.WebControls.Label errorMsgLogin;
		protected System.Web.UI.WebControls.Label loginWindowPassword;
		protected System.Web.UI.WebControls.Label loginWindowTitle;
		protected System.Web.UI.WebControls.Label loginWindowUsername;

		// Input boxes
		protected System.Web.UI.HtmlControls.HtmlInputText username;
		protected System.Web.UI.HtmlControls.HtmlInputText password;

		//Other form elements
		protected System.Web.UI.WebControls.Button loginButton;
		protected System.Web.UI.HtmlControls.HtmlForm LoginForm;
		protected System.Web.UI.WebControls.Literal loginWindowHeadTitle;
		protected System.Web.UI.WebControls.RegularExpressionValidator usernameValidator;

		protected System.String PrepareLogin ( System.String user ) {
			// Remove comments allowed by addr-spec
			user = anmar.SharpMimeTools.SharpMimeTools.uncommentString (user);
			System.String[] tmp = user.Split ('@');
			if ( tmp.Length==2 )
				// Remove space surrounding local-part and domain allowed by addr-spec
				user = System.String.Format ("{0}@{1}", tmp[0].Trim(), tmp[1].Trim());
			// TODO: limit user length
			return user;
		}
		/*
		 * Events
		*/
		protected void Login_Click ( System.Object sender, System.EventArgs args ) {
			// authenticate user
			if (this.IsPostBack&&this.IsValid) {
				int login_mode = (int)Application["login_mode"];
				if ( login_mode==3 && Application["login_mode_append"]!=null ) {
					if ( this.username.Value.IndexOf ("@") == -1 ) {
						this.username.Value = System.String.Format ( "{0}@{1}", this.username.Value, Application["login_mode_append"]);
					}
					this.usernameValidator.ValidationExpression = "^" + anmar.SharpMimeTools.ABNF.addr_spec + "$";
					this.usernameValidator.Validate();
				}
				if ( this.IsValid ) {
					this.username.Value=this.PrepareLogin(this.username.Value);
					anmar.SharpWebMail.CTNSimplePOP3Client client = new anmar.SharpWebMail.CTNSimplePOP3Client (Application["mail_server_pop3"].ToString(), (int) Application["mail_server_pop3_port"], this.username.Value, password.Value );
					anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
					if ( client.getInboxIndex ( inbox, 0, (int) Application["pagesize"], true ) ) {
						System.Web.Security.FormsAuthentication.RedirectFromLoginPage(this.username.Value, false);
						Session["client"] = client;
						Session["inbox"] = inbox;
					} else {
						errorMsgLogin.Visible=true;
					}
					client = null;
					inbox = null;
				}
			}
		}
		/*
		 * Page Events
		*/
		protected void Page_Load ( System.Object sender, System.EventArgs args ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			if ( !this.IsPostBack ) {
				//Localized resources for this session
				System.Resources.ResourceSet rs = (System.Resources.ResourceSet) Session["resources"];
				// Set labels localized texts
				loginWindowTitle.Text = rs.GetString("loginWindowTitle") + ": " + System.Configuration.ConfigurationSettings.AppSettings["system_name"];
				loginWindowHeadTitle.Text = System.Configuration.ConfigurationSettings.AppSettings["system_name"];
	            loginWindowUsername.Text = rs.GetString("loginWindowUsername");
				loginWindowPassword.Text = rs.GetString("loginWindowPassword");
				loginButton.Text = rs.GetString("loginButton");
				errorMsgLogin.Text = rs.GetString("errorMsgLogin");

				switch ( (int)Application["login_mode"] ) {
					case 2:
						loginWindowUsername.Text = rs.GetString("loginWindowUsername2");
						this.usernameValidator.ValidationExpression = ".+";
						break;
					case 3:
						this.usernameValidator.ValidationExpression = "^" + anmar.SharpMimeTools.ABNF.local_part + "(@" + anmar.SharpMimeTools.ABNF.domain + "$){0,1}";
						break;
					case 1:
					default:
						this.usernameValidator.ValidationExpression = "^" + anmar.SharpMimeTools.ABNF.addr_spec + "$";
						break;
				}
				rs = null;
			}
		}

	}
}
