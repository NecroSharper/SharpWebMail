// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2005 Angel Marin
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
	public class readmessageheaders : System.Web.UI.Page {
		protected System.Web.UI.WebControls.Literal headers;
		/*
		 * Page Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void Page_Load ( System.Object sender, System.EventArgs args ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			System.String msgid = Page.Request.QueryString["msgid"];
			if ( msgid != null ) {
				//Our Inbox
				anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
				System.Object[] details = inbox[ msgid ];
				if ( details != null && details.Length>0 ) {
					this.headers.Text = Server.HtmlEncode(((anmar.SharpMimeTools.SharpMimeHeader)details[13]).RawHeaders);
					this.headers.Text = System.String.Format ("<pre>{0}</pre>", this.headers.Text);
				}
				inbox = null;
			}
		}
	}
}
