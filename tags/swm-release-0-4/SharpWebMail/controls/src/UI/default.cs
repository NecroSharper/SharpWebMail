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
	/// <summary>
	/// 
	/// </summary>
	public class Default : System.Web.UI.Page {

		// General variables
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected bool resetsearch = false;
		protected bool refresh = false;
		protected System.String sort = "msgnum DESC";
		protected anmar.SharpWebMail.UI.globalUI SharpUI;

		//Form
		protected System.Web.UI.HtmlControls.HtmlForm sharpwebmailform;

		// DataGrid
		protected System.Web.UI.WebControls.DataGrid InboxDataGrid = null;

		// Holders
		protected System.Web.UI.WebControls.PlaceHolder inboxWindowSearchHolder;

		/*
		 * General functions
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void bindInbox () {
			anmar.SharpWebMail.CTNSimplePOP3Client client = (anmar.SharpWebMail.CTNSimplePOP3Client)Session["client"];

			System.String pattern;
			if ( this.searchPattern ( out pattern ) ) {
				if ( client!=null )
					client.getInboxIndex ( this.SharpUI.Inbox, 0, this.SharpUI.Inbox.Count , false );
			} else {
				// Ask for new messages if necessary
				if ( client!=null ) 
					client.getInboxIndex ( this.SharpUI.Inbox, this.InboxDataGrid.CurrentPageIndex, (int) Application["pagesize"], this.refresh );
				// Update message count
				if ( this.refresh )
					this.SharpUI.setVariableLabels();
			}
			this.buildDataGrid ();
			System.Data.DataView tmpV = new System.Data.DataView ( this.SharpUI.Inbox.getInbox );
			this.sort = this.SharpUI.Inbox.sortExpression;
			if ( this.sort==null || this.sort.Length==0 ) {
				this.sort = "msgnum DESC";
			}
			tmpV.RowFilter = pattern;
			tmpV.Sort = this.sort;
			this.InboxDataGrid.DataSource = tmpV;
			this.InboxDataGrid.DataBind();

			this.mainInterface ( );

			Session["client"] = client;
			client = null;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void buildDataGrid () {
			this.InboxDataGrid.PageSize = (int)Application["pagesize"];
			this.InboxDataGrid.Columns[0].HeaderText = this.SharpUI.LocalizedRS.GetString("inboxHeaderNumber");
			this.InboxDataGrid.Columns[0].HeaderStyle.Wrap = false;
			this.InboxDataGrid.Columns[0].ItemStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;

			this.InboxDataGrid.Columns[1].HeaderText = this.SharpUI.LocalizedRS.GetString("inboxHeaderFrom");
			this.InboxDataGrid.Columns[1].HeaderStyle.Wrap = false;

			this.InboxDataGrid.Columns[2].HeaderText = this.SharpUI.LocalizedRS.GetString("inboxHeaderSubject");
			this.InboxDataGrid.Columns[2].HeaderStyle.Wrap = false;

			this.InboxDataGrid.Columns[3].HeaderText = this.SharpUI.LocalizedRS.GetString("inboxHeaderDate");
			this.InboxDataGrid.Columns[3].HeaderStyle.Wrap = false;
			this.InboxDataGrid.Columns[3].ItemStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;

			this.InboxDataGrid.Columns[4].HeaderText = this.SharpUI.LocalizedRS.GetString("inboxHeaderSize");
			this.InboxDataGrid.Columns[4].HeaderStyle.Wrap = false;
			this.InboxDataGrid.Columns[4].ItemStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;

			this.InboxDataGrid.PagerStyle.NextPageText = this.SharpUI.LocalizedRS.GetString("inboxNextPage");
			this.InboxDataGrid.PagerStyle.PrevPageText = this.SharpUI.LocalizedRS.GetString("inboxPrevPage");
		}
		/// <summary>
		/// 
		/// </summary>
		protected void mainInterface ( ) {

			// Disable some things
			if ( this.InboxDataGrid.CurrentPageIndex == 0 ){
				this.SharpUI.prevPageImageButton.Enabled = false;
			} else {
				this.SharpUI.prevPageImageButton.Enabled = true;
			}
			if ( this.InboxDataGrid.CurrentPageIndex == (this.InboxDataGrid.PageCount-1) ) {
				this.SharpUI.nextPageImageButton.Enabled = false;
			} else {
				this.SharpUI.nextPageImageButton.Enabled = true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected bool searchPattern ( out System.String pattern ) {
			bool forcecache = false;
			System.String Value, format, searchtype;
			System .String mode = Page.Request.QueryString["mode"];
			if ( mode!=null && mode.Equals("trash") )
				pattern = "delete=true";
			else
				pattern = "delete=false";
			searchtype = "OR";
			if ( this.resetsearch == false ) {
				foreach ( System.String param in Request.Form ) {
					Value = Request.Form[param];
					format = "";
					switch ( param ) {
						case "SharpUI:fromsearch":
							format = "From like '%{0}%'";
							if ( Value.Length>0 )
								forcecache = true;
							break;
						case "SharpUI:subjectsearch":
							format = "Subject like '%{0}%'";
							if ( Value.Length>0 )
								forcecache = true;
							break;
					}
					if ( format.Length>0 && Value.Length>0 ) {
						Value = Value.Trim();
						System.String[] items = Value.Replace("\'", "\'\'").Split (' ');
						pattern += " AND (";
						for ( int i=0; i<items.Length; i++ ) {
							if ( items[i].StartsWith("\"") && !items[i].EndsWith("\"")) {
								items[i+1] = System.String.Format ( "{0} {1}", items[i], items[i+1]);
							}
							if ( i>0 )
								pattern += " " + searchtype + " ";
							pattern += System.String.Format ( format, items[i] );
						}
						pattern += ")";
						System.Web.UI.HtmlControls.HtmlInputHidden hidden = new System.Web.UI.HtmlControls.HtmlInputHidden();
						hidden.ID = param.Remove(0, 8);
						hidden.Value = Value;
						this.inboxWindowSearchHolder.Controls.Add ( hidden );
					}
				}
			}
			return forcecache;
		}
		
		/*
		 * Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void InboxDataGrid_ItemDataBound ( System.Object sender, System.Web.UI.WebControls.DataGridItemEventArgs args ) {
			// TODO: change aspect of read messages
			if ( args.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item ) {
				args.Item.Attributes.Add ("onmouseover", "this.className='" + this.InboxDataGrid.ItemStyle.CssClass + "H'");
				args.Item.Attributes.Add("onmouseout", "this.className='" + this.InboxDataGrid.ItemStyle.CssClass + "'");
			}
			if ( args.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem ) {
				args.Item.Attributes.Add ("onmouseover", "this.className='" + this.InboxDataGrid.AlternatingItemStyle.CssClass + "H'");
				args.Item.Attributes.Add("onmouseout", "this.className='" + this.InboxDataGrid.AlternatingItemStyle.CssClass + "'");
			}
			System.Web.UI.WebControls.HyperLink label = (System.Web.UI.WebControls.HyperLink)(args.Item.Cells[2].FindControl("inboxItemSubjectLink"));
			if ( label!=null && label.Text.Length == 0 ) {
				label.Text = SharpUI.LocalizedRS.GetString("noSubject");
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void InboxDataGrid_Sort ( System.Object sender, System.Web.UI.WebControls.DataGridSortCommandEventArgs args ) {
			this.sort = args.SortExpression.ToString();
			anmar.SharpWebMail.CTNSimplePOP3Client client = (anmar.SharpWebMail.CTNSimplePOP3Client)Session["client"];
			anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
			if ( inbox.sortExpression != this.sort ) {
				inbox.sortExpression = this.sort;
				if ( client.getInboxIndex ( inbox, 0, (int) Application["pagesize"], false ) ) {
					Session["client"] = client;
					Session["inbox"] = inbox;
					this.InboxDataGrid.CurrentPageIndex = 0;
				}
			}
			inbox = null;
			client = null;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void InboxDataGrid_PageIndexChanged ( System.Object sender, System.Web.UI.WebControls.DataGridPageChangedEventArgs args ) {
			if ( this.InboxDataGrid.CurrentPageIndex < args.NewPageIndex ) {
				anmar.SharpWebMail.CTNSimplePOP3Client client = (anmar.SharpWebMail.CTNSimplePOP3Client)Session["client"];
				anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
				if ( client.getInboxIndex ( inbox, args.NewPageIndex, (int) Application["pagesize"], false ) ) {
					Session["inbox"] = inbox;
					Session["client"] = client;
				}
				inbox = null;
				client = null;
			}
			this.InboxDataGrid.CurrentPageIndex = args.NewPageIndex;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void inboxLinkButton_Click ( System.Object sender, System.EventArgs args ) {
			this.InboxDataGrid.CurrentPageIndex = 0;
			this.resetsearch = true;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void nextPageButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			if ( this.InboxDataGrid.CurrentPageIndex < this.InboxDataGrid.PageCount ) {
				InboxDataGrid_PageIndexChanged ( sender, new System.Web.UI.WebControls.DataGridPageChangedEventArgs ( sender, this.InboxDataGrid.CurrentPageIndex+1 ) );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void prevPageButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			if ( this.InboxDataGrid.CurrentPageIndex > 0 ) {
				InboxDataGrid_PageIndexChanged (sender, new System.Web.UI.WebControls.DataGridPageChangedEventArgs ( sender, this.InboxDataGrid.CurrentPageIndex-1 ));
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void refreshPageButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			this.refresh = true;
		}
		/*
		 * Page Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void Page_Load ( System.Object sender, System.EventArgs args ) {
			if ( this.InboxDataGrid == null ) {
				this.InboxDataGrid=(System.Web.UI.WebControls.DataGrid )this.SharpUI.FindControl("InboxDataGrid");
				this.inboxWindowSearchHolder=(System.Web.UI.WebControls.PlaceHolder)this.SharpUI.FindControl("inboxWindowSearchHolder");
			}
			this.SharpUI.nextPageImageButton.Click += new System.Web.UI.ImageClickEventHandler(nextPageButton_Click);
			this.SharpUI.prevPageImageButton.Click += new System.Web.UI.ImageClickEventHandler(prevPageButton_Click);
			this.SharpUI.refreshPageImageButton.Click += new System.Web.UI.ImageClickEventHandler(refreshPageButton_Click);
		}
		/// <summary>
		/// 
		/// </summary>
		protected void Page_PreRender( System.Object sender, EventArgs args ) {
			this.bindInbox();
		}
	}
}
