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
		private System.String msgid;
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
						// TODO: Sanitize html
						label.Text = mm.BodyDecoded;
						if ( mm.IsTextBrowserDisplay ) {
							label.Text = label.Text.Insert (0, "<pre>");
							label.Text = label.Text.Insert (label.Text.Length, "</pre>");
						} else {
							label.CssClass = "XPFormText";
							int i = label.Text.IndexOf ("<body");
							if ( i>-1 ) {
								i = label.Text.IndexOf ( '>', i );
								if ( i>-1 )
									label.Text = label.Text.Remove ( 0, i+1 );
							}
							i = label.Text.IndexOf ("</body>");
							if ( i>0 ) {
								label.Text = label.Text.Remove ( i, label.Text.Length-i );
							}
						}
						entity.Controls.Add (label);
						break;
					} else {
						goto case anmar.SharpMimeTools.MimeTopLevelMediaType.application;
					}
					// Lets make Mono compiler happy
					break;
				case anmar.SharpMimeTools.MimeTopLevelMediaType.application:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.audio:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.image:
				case anmar.SharpMimeTools.MimeTopLevelMediaType.video:
					System.String name = mm.Name;
					if ( name!=null ) {
						if ( log.IsDebugEnabled )
							log .Debug ("Found attachment: " + name);
						name = System.IO.Path.GetFileName(name);
						System.Web.UI.WebControls.HyperLink attachment = new System.Web.UI.WebControls.HyperLink ();
						System.Web.UI.WebControls.Image image = null;
						System.String urlstring = System.String.Format("download.aspx?msgid={0}&name={1}&i={2}",
																	Server.UrlEncode(msgid), Server.UrlEncode(name),
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
						attachment.Text = System.String.Format ("{0} ({1} bytes)", name, mm.Size);
						attachment.CssClass = "XPDownload";
						// Dump file contents
						bool error = true;
						try {
							if ( Session["temppath"]!=null ) {
								System.String path = Session["temppath"].ToString();
								path = System.IO.Path.Combine (path, msgid);
								System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo ( path );
								dir.Create();
								System.IO.FileInfo file = new System.IO.FileInfo (System.IO.Path.Combine (path, name) );
								if ( dir.Exists
									 && dir.FullName.Equals (new System.IO.DirectoryInfo (file.Directory.FullName).FullName) ) {
									if ( !file.Exists ) {
										if ( mm.Header.ContentDispositionParameters.ContainsKey("creation-date") )
											file.CreationTime = anmar.SharpMimeTools.SharpMimeTools.parseDate ( mm.Header.ContentDispositionParameters["creation-date"] );
										if ( mm.Header.ContentDispositionParameters.ContainsKey("modification-date") )
											file.LastWriteTime = anmar.SharpMimeTools.SharpMimeTools.parseDate ( mm.Header.ContentDispositionParameters["modification-date"] );
										if ( mm.Header.ContentDispositionParameters.ContainsKey("read-date") )
											file.LastAccessTime = anmar.SharpMimeTools.SharpMimeTools.parseDate ( mm.Header.ContentDispositionParameters["read-date"] );
										System.IO.Stream stream = file.Create();
										error = !mm.DumpBody (stream);
										stream.Close();
										if ( error ) {
											file.Delete();
										}
									} else {
										error = false;
									}
								}
								dir = null;
							}
						} catch ( System.Exception ) {
							error = true;
						} finally {
							// If an error happens, we only remove the URL so
							// user knows there is an attchment
							if (error) {
								attachment.NavigateUrl = System.String.Empty;
								image = null;
							}
						}
						this.readMessageWindowAttachmentsHolder.Controls.Add (attachment);
						// Display inline image
						if ( image!=null ) {
							entity.Controls.Add (image);
						}
					}
					break;
				default:
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void mainInterface ( anmar.SharpWebMail.CTNInbox inbox ) {

			if ( this.readMessageWindowSubjectTextLabel == null ) {
				this.readMessageWindowCcTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowCcTextLabel");
				this.readMessageWindowDateTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowDateTextLabel");
				this.readMessageWindowFromTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowFromTextLabel");
				this.readMessageWindowSubjectTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowSubjectTextLabel");
				this.readMessageWindowToTextLabel=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("readMessageWindowToTextLabel");
				this.newMessageWindowTitle=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("newMessageWindowTitle");
				this.readMessageWindowBodyTextHolder=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("readMessageWindowBodyTextHolder");
				this.readMessageWindowAttachmentsHolder=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("readMessageWindowAttachmentsHolder");
			}

			// Disable some things
			this.SharpUI.nextPageButton.Enabled = false;
			this.SharpUI.prevPageButton.Enabled = false;
		}
		/*
		 * Page Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void Page_Load(System.Object Src, System.EventArgs E ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			//Our Inbox
			anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];

			this.mainInterface ( inbox );
			msgid = Page.Request.QueryString["msgid"];
			if ( msgid != null ) {
				System.Object[] details = inbox[ msgid ];
				if ( details != null && details.Length>0 ) {
					this.readMessageWindowDateTextLabel.Text = details[14].ToString();
					this.readMessageWindowFromTextLabel.Text = Server.HtmlEncode (anmar.SharpMimeTools.SharpMimeTools.parseFrom (details[4].ToString()).ToString());
					this.readMessageWindowToTextLabel.Text = Server.HtmlEncode (details[8].ToString());
					this.readMessageWindowSubjectTextLabel.Text = details[10].ToString();
					this.newMessageWindowTitle.Text = details[10].ToString();
					if ( this.newMessageWindowTitle.Text.Equals (System.String.Empty) )
						this.newMessageWindowTitle.Text = this.SharpUI.LocalizedRS.GetString("noSubject");
					anmar.SharpWebMail.CTNSimplePOP3Client client = (anmar.SharpWebMail.CTNSimplePOP3Client)Session["client"];
					System.IO.MemoryStream ms = new System.IO.MemoryStream ();
					if ( client.getMessage ( ref ms, System.Int32.Parse(details[1].ToString()) ) ) {
						anmar.SharpMimeTools.SharpMimeMessage mm = new anmar.SharpMimeTools.SharpMimeMessage ( ms );
						this.readMessageWindowCcTextLabel.Text = Server.HtmlEncode (anmar.SharpMimeTools.SharpMimeTools.parseFrom (mm.Header.Cc).ToString());
						this.decodeMessage ( mm, this.readMessageWindowBodyTextHolder );
						mm = null;
						inbox.readMessage ( msgid );
						Session["inbox"] = inbox;
						ms.Close();
					}
					ms = null;
					client = null;
				}
				details = null;
			}
			inbox = null;
		}
	}
}
