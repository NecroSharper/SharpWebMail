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
	public class globalUI : System.Web.UI.UserControl {
		// General variables
		protected System.Resources.ResourceSet resources;
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		anmar.SharpWebMail.CTNInbox inbox = null;
		
		// Holders
		protected System.Web.UI.WebControls.PlaceHolder centralPanelHolder;

		//General interface Labels
		protected System.Web.UI.WebControls.Label defaultWindowTitle;
		protected System.Web.UI.WebControls.Label messageCountLabel;
		protected System.Web.UI.WebControls.Label optionsLabel;

		//General LinkButtons
		protected System.Web.UI.WebControls.LinkButton inboxLinkButton;
		protected System.Web.UI.WebControls.LinkButton logoutLinkButton;
		protected System.Web.UI.WebControls.LinkButton newMessageLinkButton;
		protected System.Web.UI.WebControls.LinkButton searchLinkButton;

		//General ImageButtons
		public System.Web.UI.WebControls.ImageButton logOutSessionButton;
		public System.Web.UI.WebControls.ImageButton nextPageButton;
		public System.Web.UI.WebControls.ImageButton prevPageButton;
		public System.Web.UI.WebControls.ImageButton refreshPageButton;

		private System.Web.UI.ITemplate centralPanel = null;

		public System.Web.UI.ITemplate CentralPanel {
		    get { return centralPanel; }
		    set { centralPanel = value; }
		}
		public System.Resources.ResourceSet LocalizedRS {
			get {
				return this.resources;
			}
		}

		protected void closeSession () {
			if ( (IsPostBack)&&(Request.IsAuthenticated == true) ) {
				Session.Remove ("client");
				// Flush inbox content
				inbox = new anmar.SharpWebMail.CTNInbox();
				Session["inbox"] = inbox;
				// Clean up temp files
				try {
					if ( Session["temppath"]!=null ) {
						System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo (Session["temppath"].ToString());
						dir.Delete(true);
					}
				} catch( System.Exception ) {
				}
				// Logout
				System.Web.Security.FormsAuthentication.SignOut();
				// Go to login page
				Response.Redirect("login.aspx");
			}
		}
		protected void mainInterface (  ) {
			// Set general labels localized texts
			this.defaultWindowTitle.Text = System.String.Format ("{0} - {1}", Application["product"], Application["system_name"]);
			this.inboxLinkButton.Text =  System.String.Format ("{0} ({1})", this.resources.GetString("inboxLabel"), this.inbox.messageCount );
			this.logoutLinkButton.Text = this.resources.GetString("logoutLabel");
			this.searchLinkButton.Text = this.resources.GetString("searchLabel");
			this.messageCountLabel.Text = System.String.Format ("{0} {1} {2} bytes", this.inbox.messageCount, this.resources.GetString("messages"), this.inbox.messageSize);
			this.newMessageLinkButton.Text = this.resources.GetString("newMessageLabel");
			this.optionsLabel.Text = this.resources.GetString("optionsLabel");
			this.setLabels ( this.centralPanelHolder.Controls );
		}
		protected void setLabels ( System.Web.UI.ControlCollection controls ) {
			System.String label;
			foreach ( System.Web.UI.Control childcontrol in controls ) {
				if ( childcontrol.HasControls() )
					this.setLabels ( childcontrol.Controls );
				if ( childcontrol.ID==null )
					continue;
				label = null;
				label = this.resources.GetString(childcontrol.ID);
				if ( label==null )
					continue;
				if ( childcontrol is System.Web.UI.WebControls.Label )
					((System.Web.UI.WebControls.Label)childcontrol).Text = label;
				else if ( childcontrol is System.Web.UI.WebControls.Button )
					((System.Web.UI.WebControls.Button)childcontrol).Text = label;
			}
		}
		/*
		 * Events
		*/
		protected void inboxLinkButton_Click ( Object sender, System.EventArgs e ) {
			Response.Redirect("default.aspx");
		}
		/// <summary>
		/// 
		/// </summary>
		protected void logoutLinkButton_Click ( Object sender, System.EventArgs e ) {
			this.closeSession();
		}
		/// <summary>
		/// 
		/// </summary>
		protected void logOutSessionButton_Click ( Object sender, System.Web.UI.ImageClickEventArgs e ) {
			this.closeSession();
		}
		/// <summary>
		/// 
		/// </summary>
		protected void newMessageLinkButton_Click ( Object sender, System.EventArgs e ) {
			Response.Redirect("newmessage.aspx");
		}
		/// <summary>
		/// 
		/// </summary>
		protected void searchLinkButton_Click ( Object sender, System.EventArgs e ) {
			Response.Redirect("search.aspx");
		}
		/*
		 * Page Events
		*/
		protected void Page_Init() {
		    if ( this.centralPanel!=null ) {
	            this.centralPanel.InstantiateIn (this.centralPanelHolder);
		    }
		    if ( this.inbox==null) {
			    this.inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
		    }
			this.resources = (System.Resources.ResourceSet) Session["resources"];
			this.mainInterface();
		}

	}
}
