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
		protected System.Web.UI.WebControls.LinkButton trashLinkButton;

		//General ImageButtons
		public System.Web.UI.WebControls.ImageButton logoutImageButton;
		public System.Web.UI.WebControls.ImageButton nextPageImageButton;
		public System.Web.UI.WebControls.ImageButton prevPageImageButton;
		public System.Web.UI.WebControls.ImageButton refreshPageImageButton;

		private System.Web.UI.ITemplate centralPanel = null;

		public System.Web.UI.ITemplate CentralPanel {
		    get { return centralPanel; }
		    set { centralPanel = value; }
		}
		public anmar.SharpWebMail.CTNInbox Inbox {
			get {
				return this.inbox;
			}
		}
		public System.Resources.ResourceSet LocalizedRS {
			get {
				return this.resources;
			}
		}
		private void cleanTempFolder ( System.Object value ) {
			try {
				if ( value!=null ) {
					System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo (value.ToString());
					dir.Delete(true);
					dir=null;
				}
			} catch( System.Exception ) {
			}
		}
		protected void closeSession () {
			if ( (IsPostBack)&&(Request.IsAuthenticated == true) ) {
				anmar.SharpWebMail.IEmailClient client = (anmar.SharpWebMail.IEmailClient)Session["client"];
				// Delete messages marked for deletion
				if ( client!=null ) {
					client.purgeInbox ( this.inbox, false );
					client = null;
					Session.Remove ("client");
				}
				// Flush inbox content
				this.inbox = new anmar.SharpWebMail.CTNInbox();
				Session["inbox"] = this.inbox;
				this.inbox = null;
				// Clean up temp files
				cleanTempFolder(Session["sharpwebmail/read/message/temppath"]);
				cleanTempFolder(Session["sharpwebmail/send/message/temppath"]);
				// Logout
				System.Web.Security.FormsAuthentication.SignOut();
				// Go to login page
				Response.Redirect("login.aspx");
			}
		}
		protected void mainInterface (  ) {
			// Set general labels localized texts
			this.setLabels ( this.Controls );
			this.defaultWindowTitle.Text = System.String.Format ("{0} - {1}", Application["product"], Application["sharpwebmail/general/title"]);
			this.setVariableLabels();
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
				if ( childcontrol is System.Web.UI.WebControls.Button )
					((System.Web.UI.WebControls.Button)childcontrol).Text = label;
				else if ( childcontrol is System.Web.UI.WebControls.HyperLink )
					((System.Web.UI.WebControls.HyperLink)childcontrol).ToolTip = label;
				else if ( childcontrol is System.Web.UI.WebControls.ImageButton )
					((System.Web.UI.WebControls.ImageButton)childcontrol).ToolTip = label;
				else if ( childcontrol is System.Web.UI.WebControls.Label )
					((System.Web.UI.WebControls.Label)childcontrol).Text = label;
				else if ( childcontrol is System.Web.UI.WebControls.LinkButton )
					((System.Web.UI.WebControls.LinkButton)childcontrol).Text = label;
			}
		}
		internal void setVariableLabels ( ) {
			if ( this.inboxLinkButton.Text.EndsWith(")") ) 
				this.inboxLinkButton.Text= this.inboxLinkButton.Text.Remove(this.inboxLinkButton.Text.LastIndexOf(" ("), this.inboxLinkButton.Text.Length-this.inboxLinkButton.Text.LastIndexOf(" ("));
			if ( this.trashLinkButton.Text.EndsWith(")") ) 
				this.trashLinkButton.Text= this.trashLinkButton.Text.Remove(this.trashLinkButton.Text.LastIndexOf(" ("), this.trashLinkButton.Text.Length-this.trashLinkButton.Text.LastIndexOf(" ("));
			this.inboxLinkButton.Text =  System.String.Format ("{0} ({1})", this.inboxLinkButton.Text, this.inbox.messageCount );
			this.messageCountLabel.Text = System.String.Format ("{0} {1} {2} bytes", this.inbox.messageCount, this.resources.GetString("messages"), this.inbox.messageSize);
			this.trashLinkButton.Text = System.String.Format ("{0} ({1})", this.trashLinkButton.Text, this.inbox.Count - this.inbox.messageCount );
		}
		/*
		 * Events
		*/
		protected void inboxLinkButton_Click ( System.Object sender, System.EventArgs args ) {
			Response.Redirect("default.aspx");
		}
		/// <summary>
		/// 
		/// </summary>
		protected void logoutLinkButton_Click ( System.Object sender, System.EventArgs args ) {
			this.closeSession();
		}
		/// <summary>
		/// 
		/// </summary>
		protected void logOutSessionButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			this.closeSession();
		}
		/// <summary>
		/// 
		/// </summary>
		protected void newMessageLinkButton_Click ( System.Object sender, System.EventArgs args ) {
			Response.Redirect("newmessage.aspx");
		}
		/// <summary>
		/// 
		/// </summary>
		protected void searchLinkButton_Click ( System.Object sender, System.EventArgs args ) {
			Response.Redirect("search.aspx");
		}
		/// <summary>
		/// 
		/// </summary>
		protected void trashLinkButton_Click ( System.Object sender, System.EventArgs args ) {
			Response.Redirect("default.aspx?mode=trash");
		}
		/*
		 * Page Events
		*/
		protected void Page_Disposed ( System.EventArgs args ) {
			Session["inbox"] = this.inbox;
			this.inbox = null;
			this.resources = null;
		}
		protected void Page_Init () {
		    if ( this.centralPanel!=null ) {
	            this.centralPanel.InstantiateIn (this.centralPanelHolder);
		    }
		    if ( this.inbox==null) {
			    this.inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
		    }
			this.resources = (System.Resources.ResourceSet) Session["resources"];
		}
		protected void Page_Load ( System.Object sender, System.EventArgs args ) {
			this.mainInterface();
		}
	}
}
