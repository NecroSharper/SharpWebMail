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
	public class readmessage : System.Web.UI.Page {
		// General variables
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected System.String msgid;
		protected bool delete=false;
		protected anmar.SharpWebMail.UI.globalUI SharpUI;

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

		private void decodeMessage ( anmar.SharpMimeTools.SharpMimeMessage mm, System.Web.UI.WebControls.PlaceHolder entity ) {
			System.String inline = System.String.Empty;
			switch ( mm.Header.TopLevelMediaType ) {
				case anmar.SharpMimeTools.MimeTopLevelMediaType.multipart:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.message:
					// TODO: allow other subtypes of "message"
					// Only message/rfc822 allowed, other subtypes ignored
					if ( mm.Header.TopLevelMediaType.Equals(anmar.SharpMimeTools.MimeTopLevelMediaType.message)
						 && !mm.Header.SubType.Equals("rfc822") )
						break;
					if ( mm.Header.SubType.Equals ("alternative") ) {
						if ( mm.PartsCount>0 ) {
							this.decodeMessage ( mm.GetPart(mm.PartsCount-1),
												 entity);
						}
					// TODO: Take into account each subtype of "multipart"
					} else if ( mm.PartsCount>0 ) {
						System.Web.UI.WebControls.PlaceHolder nestedentity = new System.Web.UI.WebControls.PlaceHolder ();
						System.Collections.IEnumerator enu = mm.GetEnumerator();
						while ( enu.MoveNext() ) {
							this.decodeMessage ((anmar.SharpMimeTools.SharpMimeMessage) enu.Current, nestedentity);
						}
						entity.Controls.Add (nestedentity);
					}
					break;
				case anmar.SharpMimeTools.MimeTopLevelMediaType.text:
					if ( ( mm.Disposition==null || !mm.Disposition.Equals("attachment") )
						&& ( mm.Header.SubType.Equals("plain") || mm.Header.SubType.Equals("html") ) ) {
						System.Web.UI.WebControls.Label label = new System.Web.UI.WebControls.Label ();
						label.Text = mm.BodyDecoded;
						if ( mm.IsTextBrowserDisplay ) {
							label.Text = System.Web.HttpUtility.HtmlEncode (label.Text);
							label.Text = label.Text.Insert (0, "<pre>");
							label.Text = label.Text.Insert (label.Text.Length, "</pre>");
						} else {
							label.CssClass = "XPFormText";
							if ( (int)Application["sharpwebmail/read/message/sanitizer_mode"]==1 ) {
								label.Text = anmar.SharpWebMail.BasicSanitizer.SanitizeHTML(label.Text, anmar.SharpWebMail.SanitizerMode.CommentBlocks|anmar.SharpWebMail.SanitizerMode.RemoveEvents);
							}
						}
						entity.Controls.Add (label);
						break;
					} else {
						goto case anmar.SharpMimeTools.MimeTopLevelMediaType.application;
					}
				case anmar.SharpMimeTools.MimeTopLevelMediaType.application:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.audio:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.image:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.video:
					System.Web.UI.WebControls.HyperLink attachment = new System.Web.UI.WebControls.HyperLink ();
					System.Web.UI.WebControls.Image image = null;
					attachment.CssClass = "XPDownload";
					if ( mm.Name!=null )
						attachment.Text = System.String.Format ("{0} ({1} bytes)", System.IO.Path.GetFileName(mm.Name), mm.Size);
					if ( Session["sharpwebmail/read/message/temppath"]!=null ) {
						System.String path = Session["sharpwebmail/read/message/temppath"].ToString();
						path = System.IO.Path.Combine (path, msgid);
						// Dump file contents
						System.IO.FileInfo file = mm.DumpBody ( path, true );
						if ( file!=null && file.Exists ) {
							System.String urlstring = System.String.Format("download.aspx?msgid={0}&name={1}&i={2}",
																Server.UrlEncode(msgid), Server.UrlEncode(file.Name),
																inline);
							if ( mm.Disposition!=null && mm.Disposition.Equals("inline") ) {
								inline = "1";
								if ( mm.Header.TopLevelMediaType.Equals(anmar.SharpMimeTools.MimeTopLevelMediaType.image)
										&& ( mm.Header.SubType.Equals("gif") || mm.Header.SubType.Equals("jpg") || mm.Header.SubType.Equals("png")) ) {
									image = new System.Web.UI.WebControls.Image ();
									image.ImageUrl = urlstring;
								}
							}
							attachment.NavigateUrl = urlstring;
							attachment.Text = System.String.Format ("{0} ({1} bytes)", file.Name, file.Length);
						}
					}
					this.readMessageWindowAttachmentsHolder.Controls.Add (attachment);
					// Display inline image
					if ( image!=null ) {
						entity.Controls.Add (image);
					}
					break;
				default:
					break;
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
					break;
				case "reply":
					Server.Transfer("newmessage.aspx", true);
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
				System.Object[] details = this.SharpUI.Inbox[ msgid ];
				if ( details != null && details.Length>0 ) {
					//Delete message
					if ( delete && (bool)details[15]==false ) {
						this.SharpUI.Inbox.deleteMessage ( msgid );
						this.SharpUI.setVariableLabels();
					}
					// Disable delete button if message is already deleted
					if ( (bool)details[15]==true || ( delete && (bool)details[15]==false ) )
						((System.Web.UI.WebControls.ImageButton)this.SharpUI.FindControl("msgtoolbarDelete")).Enabled=false;
					this.readMessageWindowDateTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[14].ToString());
					this.readMessageWindowFromTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[6].ToString());
					this.readMessageWindowToTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[8].ToString());
					this.readMessageWindowSubjectTextLabel.Text = System.Web.HttpUtility.HtmlEncode (details[10].ToString());
					this.newMessageWindowTitle.Text = System.Web.HttpUtility.HtmlEncode (details[10].ToString());
					if ( this.newMessageWindowTitle.Text.Equals (System.String.Empty) )
						this.newMessageWindowTitle.Text = this.SharpUI.LocalizedRS.GetString("noSubject");
					anmar.SharpWebMail.IEmailClient client = (anmar.SharpWebMail.IEmailClient)Session["client"];
					System.IO.MemoryStream ms = new System.IO.MemoryStream ();
					if ( client.getMessage ( ms, System.Int32.Parse(details[1].ToString() ) , msgid ) ) {
						anmar.SharpMimeTools.SharpMimeMessage mm = new anmar.SharpMimeTools.SharpMimeMessage ( ms );
						this.readMessageWindowCcTextLabel.Text = System.Web.HttpUtility.HtmlEncode (anmar.SharpMimeTools.SharpMimeTools.parseFrom (mm.Header.Cc).ToString());
						this.decodeMessage ( mm, this.readMessageWindowBodyTextHolder );
						mm = null;
						this.SharpUI.Inbox.readMessage ( msgid );
						ms.Close();
					}
					ms = null;
					client = null;
				}
				details = null;
			}
		}
	}
}
