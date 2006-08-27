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

namespace anmar.SharpWebMail.UI.Pages
{
	public class ReadMessage : System.Web.UI.Page {
		// General variables
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected System.String msgid;
		protected bool delete=false;
		protected anmar.SharpWebMail.UI.Pages.GlobalUI SharpUI;

		//Form
		protected System.Web.UI.HtmlControls.HtmlForm sharpwebmailform;

		// Labels
		protected System.Web.UI.WebControls.Label newMessageWindowTitle;

		// Labels
		protected System.Web.UI.WebControls.Label readMessageWindowCcTextLabel;
		protected System.Web.UI.WebControls.Label readMessageWindowDateTextLabel;
		protected System.Web.UI.WebControls.Label readMessageWindowFromTextLabel;
		protected System.Web.UI.WebControls.Label readMessageWindowSubjectTextLabel;
		protected System.Web.UI.WebControls.Label readMessageWindowToTextLabel;

		// Holders
		protected System.Web.UI.WebControls.PlaceHolder readMessageWindowAttachmentsHolder;
		protected System.Web.UI.WebControls.PlaceHolder readMessageWindowBodyTextHolder;

		private void decodeMessage ( anmar.SharpMimeTools.SharpMessage message, System.Web.UI.WebControls.PlaceHolder entity ) {
			System.Web.UI.HtmlControls.HtmlGenericControl body = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
			// RFC 2392
			message.SetUrlBase(System.String.Concat("download.aspx?msgid=", Server.UrlEncode(msgid),"&name=[Name]&i=1"));
			// Html body
			if ( message.HasHtmlBody ) {
				body.Attributes["class"] = "XPFormText";
				if ( (int)Application["sharpwebmail/read/message/sanitizer_mode"]==1 ) {
					body.InnerHtml = anmar.SharpWebMail.Tools.BasicSanitizer.SanitizeHTML(message.Body, anmar.SharpWebMail.Tools.SanitizerMode.CommentBlocks|anmar.SharpWebMail.Tools.SanitizerMode.RemoveEvents);
				} else {
					body.InnerHtml = message.Body;
				}
			// Text body
			} else {
				body.TagName = "pre";
				body.InnerText = message.Body;
			}
			entity.Controls.Add (body);
			// Attachments
			if ( message.Attachments!=null ) {
				bool inline_added = false;
				foreach ( anmar.SharpMimeTools.SharpAttachment item in message.Attachments ) {
					if ( item.SavedFile==null )
						continue;
					System.Web.UI.HtmlControls.HtmlAnchor attachment = new System.Web.UI.HtmlControls.HtmlAnchor();
					attachment.Attributes["class"] = "XPDownload";
					attachment.Title = System.String.Concat(item.SavedFile.Name, " (", item.Size, " bytes)");
					attachment.InnerText = attachment.Title; 
					System.String urlstring = System.String.Concat("download.aspx?msgid=", Server.UrlEncode(msgid),
												"&name=", Server.UrlEncode(item.SavedFile.Name), "&i=");
					attachment.HRef = urlstring;
					this.readMessageWindowAttachmentsHolder.Controls.Add(attachment);
					// Inline attachment
					if ( item.Inline ) {
						if ( item.MimeTopLevelMediaType==anmar.SharpMimeTools.MimeTopLevelMediaType.image
								&& (item.MimeMediaSubType=="gif" || item.MimeMediaSubType=="jpg" || item.MimeMediaSubType=="png") ) {
							System.Web.UI.HtmlControls.HtmlImage image = new System.Web.UI.HtmlControls.HtmlImage();
							image.Src = System.String.Concat(urlstring, "1");
							image.Alt = attachment.Name;
							if ( !inline_added ) {
								entity.Controls.Add(new System.Web.UI.HtmlControls.HtmlGenericControl("hr"));
								inline_added = true;
							}
							entity.Controls.Add(image);
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void mainInterface ( ) {

			if ( this.readMessageWindowSubjectTextLabel == null ) {
				this.readMessageWindowCcTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowCcTextLabel");
				this.readMessageWindowDateTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowDateTextLabel");
				this.readMessageWindowFromTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowFromTextLabel");
				this.readMessageWindowSubjectTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowSubjectTextLabel");
				this.readMessageWindowToTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowToTextLabel");
				this.newMessageWindowTitle=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("newMessageWindowTitle");
				this.readMessageWindowBodyTextHolder=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("readMessageWindowBodyTextHolder");
				this.readMessageWindowAttachmentsHolder=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("readMessageWindowAttachmentsHolder");
				((System.Web.UI.WebControls.HyperLink)this.SharpUI.FindControl("msgtoolbarHeader")).Attributes.Add ("onclick", "window.open('headers.aspx?msgid=" + Server.UrlEncode(msgid) + "', '_blank', 'menubar=no, toolbar=no, resizable=yes, scrollbars=yes, width=500, height=300')");
			}

			// Disable some things
			this.SharpUI.nextPageImageButton.Enabled = false;
			this.SharpUI.prevPageImageButton.Enabled = false;
		}
		/*
		 * Events
		*/
		protected void msgtoolbarCommand ( System.Object sender, System.Web.UI.WebControls.CommandEventArgs args ) {
			switch ( args.CommandName ) {
				case "delete":
					delete = true;
					break;
				case "forward":
					Response.Redirect(System.String.Concat("newmessage.aspx?", this.Request.QueryString, "&mode=forward"), false);
					break;
				case "reply":
					Response.Redirect(System.String.Concat("newmessage.aspx?", this.Request.QueryString, "&mode=reply"), false);
					break;
			}
		}
		/*
		 * Page Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void Page_Load ( System.Object sender, System.EventArgs args ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			msgid = System.Web.HttpUtility.HtmlEncode (Page.Request.QueryString["msgid"]);
			this.mainInterface ( );
		}
		protected void Page_PreRender ( System.Object sender, System.EventArgs args ) {
			if ( msgid != null ) {
				bool deleted = false;
				System.IO.MemoryStream ms = null;
				if ( delete ) {
					deleted = this.SharpUI.Inbox.DeleteMessage(msgid);
					if ( deleted )
						this.SharpUI.setVariableLabels();
				}
				// Setup some options for later use
				System.String tmppath = null;
				anmar.SharpMimeTools.MimeTopLevelMediaType types = anmar.SharpMimeTools.MimeTopLevelMediaType.text|anmar.SharpMimeTools.MimeTopLevelMediaType.multipart|anmar.SharpMimeTools.MimeTopLevelMediaType.message;
				anmar.SharpMimeTools.SharpDecodeOptions options = anmar.SharpMimeTools.SharpDecodeOptions.AllowHtml;
				if ( Session["sharpwebmail/read/message/temppath"]!=null ) {
					tmppath = System.IO.Path.Combine (Session["sharpwebmail/read/message/temppath"].ToString(), msgid);
					options |= anmar.SharpMimeTools.SharpDecodeOptions.AllowAttachments | anmar.SharpMimeTools.SharpDecodeOptions.DecodeTnef | anmar.SharpMimeTools.SharpDecodeOptions.CreateFolder;
					types |= anmar.SharpMimeTools.MimeTopLevelMediaType.application|anmar.SharpMimeTools.MimeTopLevelMediaType.audio|anmar.SharpMimeTools.MimeTopLevelMediaType.image|anmar.SharpMimeTools.MimeTopLevelMediaType.video;
				}
				// Delete messaged, we have to commit changes
				if ( deleted && (bool)Application["sharpwebmail/read/inbox/commit_ondelete"] ) {
					this.SharpUI.Inbox.Client.PurgeInbox( this.SharpUI.Inbox, false );
					Response.Redirect("default.aspx");
				} else {
					// We retrieve the message body
					this.SharpUI.Inbox.CurrentFolder = this.SharpUI.Inbox.GetMessageFolder(msgid);
					ms = this.SharpUI.Inbox.GetMessage(msgid);
				}
				if ( ms!=null ) {
					System.Object[] details = this.SharpUI.Inbox[ msgid ];
					// Disable delete button if message is already deleted
					if ( details[15].Equals(true) || deleted )
						((System.Web.UI.WebControls.ImageButton)this.SharpUI.FindControl("msgtoolbarDelete")).Enabled=false;
					this.readMessageWindowDateTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[14].ToString());
					this.readMessageWindowFromTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[6].ToString());
					this.readMessageWindowToTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[8].ToString());
					this.readMessageWindowSubjectTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[10].ToString());
					this.newMessageWindowTitle.Text = System.Web.HttpUtility.HtmlEncode (details[10].ToString());
					if ( this.newMessageWindowTitle.Text.Equals (System.String.Empty) )
						this.newMessageWindowTitle.Text = this.SharpUI.LocalizedRS.GetString("noSubject");
					if ( ms!=null && ms.CanRead ) {
						anmar.SharpMimeTools.SharpMessage mm = new anmar.SharpMimeTools.SharpMessage(ms, types, options, tmppath, null);
						this.readMessageWindowCcTextLabel.Text = System.Web.HttpUtility.HtmlEncode (mm.Cc.ToString());
						this.decodeMessage ( mm, this.readMessageWindowBodyTextHolder );
						mm = null;
						this.SharpUI.Inbox.readMessage ( msgid );
					}
					details = null;
				}
				if ( ms!=null && ms.CanRead )
					ms.Close();
				ms = null;
				
			}
		}
	}
}
