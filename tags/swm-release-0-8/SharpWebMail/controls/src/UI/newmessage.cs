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
	public class newmessage : System.Web.UI.Page {

		#region General Fields
		// General Fields
		protected anmar.SharpWebMail.UI.globalUI SharpUI;
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static System.String bodyStart = "<html><head><title></title></head><body bgcolor=\"#FFFFFF\" text=\"#000000\" leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\">";
		private static System.String bodyEnd = "</body></html>";
		private  anmar.SharpMimeTools.SharpMimeHeader Header=null;
		private int UI_case=0;
		
		#endregion General variables

		#region UI Fields

		//Form
		protected System.Web.UI.HtmlControls.HtmlForm sharpwebmailform;

		// Input boxes
		protected System.Web.UI.HtmlControls.HtmlInputText fromemail;
		protected System.Web.UI.HtmlControls.HtmlInputText fromname;
		protected System.Web.UI.HtmlControls.HtmlInputText subject;
		protected System.Web.UI.HtmlControls.HtmlInputText toemail;

		// Labels
		protected System.Web.UI.WebControls.Label newMessageWindowConfirmation;

		//Editor
		protected FredCK.FCKeditorV2.FCKeditor FCKEditor; 

		//Panels
		protected System.Web.UI.WebControls.Panel attachmentsPanel;
		protected System.Web.UI.WebControls.Panel confirmationPanel;
		protected System.Web.UI.WebControls.Panel newMessageFromPanel;
		protected System.Web.UI.WebControls.Panel newMessagePanel;

		protected System.Web.UI.WebControls.PlaceHolder newattachmentPH;

		//Other form elements
		protected System.Web.UI.HtmlControls.HtmlInputFile newMessageWindowAttachFile;
		protected System.Web.UI.WebControls.CheckBoxList newMessageWindowAttachmentsList;
		protected System.Web.UI.WebControls.DataList newMessageWindowAttachmentsAddedList;

		#endregion UI variables

		#region Private Methods

		protected void bindAttachments () {
			System.Collections.SortedList attachments = new System.Collections.SortedList();
			bindAttachments ( attachments, Session["sharpwebmail/read/message/temppath"] );
			if ( !Session["sharpwebmail/read/message/temppath"].Equals(Session["sharpwebmail/send/message/temppath"]) )
				bindAttachments ( attachments, Session["sharpwebmail/send/message/temppath"] );
			this.newMessageWindowAttachmentsList.DataSource = attachments;
			this.newMessageWindowAttachmentsList.DataTextField = "Value";
			this.newMessageWindowAttachmentsList.DataValueField = "Key";
			this.newMessageWindowAttachmentsList.DataBind();
		}
		protected void bindAttachments ( System.Collections.SortedList attachments, System.Object pathname ) {
			if ( pathname!=null ) {
				System.String path = pathname.ToString();
				System.IO.DirectoryInfo basedir = new System.IO.DirectoryInfo(path);
				if ( basedir.Exists ) {
					foreach ( System.IO.FileInfo file in basedir.GetFiles() ) {
						attachments.Add ( System.IO.Path.Combine(file.Directory.Name, file.Name), System.String.Format ("{0} ({1:F2} KB)", file.Name, file.Length/1024.0));
					}
					foreach ( System.String dir in System.IO.Directory.GetDirectories (path) ){
						foreach ( System.IO.FileInfo file in (new System.IO.DirectoryInfo(dir)).GetFiles() ) {
							attachments.Add (System.IO.Path.Combine(file.Directory.Name, file.Name), System.String.Format ("{0} ({1:F2} KB)", file.Name, file.Length/1024.0));
						}
					}
				}
				basedir = null;
			}
		}
		private System.String getfilename ( System.Object temppath, params System.String[] parts ) {
			System.String path = null;
			if ( temppath!=null ) {
				path = temppath.ToString();
				System.IO.DirectoryInfo basedir = new System.IO.DirectoryInfo(path);
				if ( basedir.Exists ) {
					foreach ( System.String part in parts ) {
						try {
							path = System.IO.Path.Combine (path, part);
						} catch ( System.ArgumentException e ) {
							if ( log.IsErrorEnabled )
								log.Error("Filename has invalid chars", e);
							// Remove invalid chars
							System.String tmppart = part;
							foreach ( char ichar in System.IO.Path.InvalidPathChars ) {
								tmppart = tmppart.Replace ( ichar.ToString(), System.String.Empty );
							}
							path = System.IO.Path.Combine (path, tmppart);
						}
					}
					System.IO.FileInfo file = new System.IO.FileInfo ( path );
					System.IO.DirectoryInfo filedir = new System.IO.DirectoryInfo (file.Directory.FullName);
					if ( !file.Exists || (!basedir.FullName.Equals(filedir.FullName) && !basedir.FullName.Equals(filedir.Parent.FullName) ) ) {
						path = null;
					}
					filedir = null;
					file = null;
				} else {
					path = null;
				}
				basedir = null;
			}
			if ( log.IsDebugEnabled ) log.Debug ("Path: " + path );
			return path;
		}
		private System.String getFrom () {
			System.String from = null;
			switch ( (int)Application["sharpwebmail/login/mode"] ) {
				case 2:
					from = System.String.Format("{0} <{1}>", this.fromname.Value.Trim(), this.fromemail.Value.Trim());
					break;
				case 1:
				case 3:
				default:
					from = System.String.Format("{0} <{1}>", this.fromname.Value.Trim(), User.Identity.Name);
					break;
			}
			return from;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void mainInterface ( anmar.SharpWebMail.CTNInbox inbox ) {
			this.newattachmentPH=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("newattachmentPH");
			this.attachmentsPanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("attachmentsPanel");
			this.confirmationPanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("confirmationPanel");
			this.newMessagePanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("newMessagePanel");
			this.newMessageWindowAttachFile=( System.Web.UI.HtmlControls.HtmlInputFile )this.SharpUI.FindControl("newMessageWindowAttachFile");
			this.newMessageWindowAttachmentsList=(System.Web.UI.WebControls.CheckBoxList )this.SharpUI.FindControl("newMessageWindowAttachmentsList");
			this.newMessageWindowAttachmentsAddedList=(System.Web.UI.WebControls.DataList )this.SharpUI.FindControl("newMessageWindowAttachmentsAddedList");

			this.FCKEditor = (FredCK.FCKeditorV2.FCKeditor)this.SharpUI.FindControl("FCKEditor");
			this.fromname = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("fromname");
			this.fromemail = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("fromemail");
			this.subject = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("subject");
			this.toemail = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("toemail");

#if MONO
			System.Web.UI.WebControls.RequiredFieldValidator rfv = (System.Web.UI.WebControls.RequiredFieldValidator) this.SharpUI.FindControl("ReqbodyValidator");
			rfv.Enabled=false;
			this.Validators.Remove(rfv);
#endif

			this.newMessageWindowConfirmation = (System.Web.UI.WebControls.Label)this.SharpUI.FindControl("newMessageWindowConfirmation");

			this.SharpUI.refreshPageImageButton.Click += new System.Web.UI.ImageClickEventHandler(refreshPageButton_Click);

			// Disable Panels
			this.attachmentsPanel.Visible = false;
			this.confirmationPanel.Visible = false;

			// Disable some things
			this.SharpUI.nextPageImageButton.Enabled = false;
			this.SharpUI.prevPageImageButton.Enabled = false;

			System.String msgid = System.Web.HttpUtility.HtmlEncode (Page.Request.QueryString["msgid"]);
			System.Guid guid = System.Guid.Empty;
			if ( msgid != null)
				guid = new System.Guid(msgid);
			if ( !guid.Equals( System.Guid.Empty) ) {
				System.Object[] details = inbox[ guid ];
				if ( details != null ) {
					this.Header = (anmar.SharpMimeTools.SharpMimeHeader) details[13];
					if ( !this.IsPostBack ) {
						this.subject.Value = System.String.Format ("{0}:", this.SharpUI.LocalizedRS.GetString("replyPrefix"));
						if ( details[10].ToString().ToLower().IndexOf (this.subject.Value.ToLower())!=-1 ) {
							this.subject.Value = details[10].ToString().Trim();
						} else {
							this.subject.Value = System.String.Format ("{0} {1}", this.subject.Value, details[10].ToString()).Trim();
						}
						// From name if present on original message's To header
						foreach ( anmar.SharpMimeTools.SharpMimeAddress address in (System.Collections.IEnumerable) details[8] ) {
							if ( address["address"]!=null && address["address"].Equals( User.Identity.Name )
								&& address["name"].Length>0 && !address["address"].Equals(address["name"]) ) {
								this.fromname.Value = address["name"];
								break;
							}
						}
						// To addresses
						foreach ( anmar.SharpMimeTools.SharpMimeAddress address in (System.Collections.IEnumerable) details[9] ) {
							if ( address["address"]!=null && !address["address"].Equals( User.Identity.Name ) ) {
								if ( this.toemail.Value.Length >0 )
									this.toemail.Value += "; ";
								this.toemail.Value += address["address"];
							}
						}
					}
					details = null;
				}
			}
			if ( this.fromname.Value.Length>0 || this.IsPostBack )
				Session["DisplayName"] = this.fromname.Value;
			if ( this.fromname.Value.Length==0 && Session["DisplayName"]!=null )
				this.fromname.Value = Session["DisplayName"].ToString();
			if ( this.fromemail.Value.Length>0 || this.IsPostBack )
				Session["DisplayEmail"] = this.fromemail.Value;
		}
		/// <summary>
		/// 
		/// </summary>
		private bool sendMail ( out System.String message ) {
			bool error = false;
			message = null;
			anmar.SharpWebMail.ServerSelector selector = (anmar.SharpWebMail.ServerSelector)Application["sharpwebmail/send/servers"];
			anmar.SharpWebMail.EmailServerInfo server = selector.Select(User.Identity.Name);
			if ( server==null || !server.Protocol.Equals(anmar.SharpWebMail.ServerProtocol.Smtp ) ) {
				error = true;
				return !error;
			}
			System.Web.Mail.SmtpMail.SmtpServer = server.Host;

			System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
			mailMessage.To = this.toemail.Value;
			mailMessage.From = this.getFrom();
			mailMessage.Subject = this.subject.Value.Trim();
			System.String format = Request.Form["format"];
			if ( format!=null && format.Equals("html") ) {
				mailMessage.BodyFormat = System.Web.Mail.MailFormat.Html;
				mailMessage.Body = bodyStart + FCKEditor.Value + bodyEnd;
			} else {
				mailMessage.BodyFormat = System.Web.Mail.MailFormat.Text;
				mailMessage.Body = FCKEditor.Value;
			}

			if ( this.Header != null ) {
				// RFC 2822 3.6.4. Identification fields
				if ( this.Header["Message-ID"]!=null ) {
					mailMessage.Headers["In-Reply-To"] = this.Header["Message-ID"];
					mailMessage.Headers["References"] = this.Header["Message-ID"];
				}
				if ( this.Header["References"]!=null ) {
					mailMessage.Headers["References"] = System.String.Format ("{0} {1}", this.Header["References"], mailMessage.Headers["References"]).Trim();
				} else if ( this.Header["In-Reply-To"]!=null && this.Header["In-Reply-To"].IndexOf('>')==this.Header["In-Reply-To"].LastIndexOf('>') ) {
					mailMessage.Headers["References"] = System.String.Format ("{0} {1}", this.Header["In-Reply-To"], mailMessage.Headers["References"]).Trim();
				}
			}
			mailMessage.Headers["X-Mailer"] = System.String.Format ("{0} {1}", Application["product"], Application["version"]);
			// Attachments
			if ( this.newMessageWindowAttachmentsAddedList.Items.Count>0 ) {
				anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];
				System.String Value="";
				foreach ( System.Web.UI.WebControls.ListItem item in this.newMessageWindowAttachmentsList.Items ) {
					if ( item.Selected ) {
						if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("Attaching {0} {1}", item.Text, item.Value) );
						if ( !item.Value.StartsWith ( Session.SessionID ) ) {
							Value = item.Value.Substring ( 0 , item.Value.IndexOf ( System.IO.Path.DirectorySeparatorChar ));
							System.Object[] details = inbox[ Value ];
							if ( details == null ) {
								Value=null;
							}
							details = null;
						}
						if ( Value!=null ) {
							System.String attachment = this.getfilename (Session["sharpwebmail/send/message/temppath"], Value, item.Text.Substring(0, item.Text.LastIndexOf(" (")));
							if ( attachment==null )
								attachment = this.getfilename (Session["sharpwebmail/read/message/temppath"], Value, item.Text.Substring(0, item.Text.LastIndexOf(" (")));
							if ( attachment!=null ) {
								mailMessage.Attachments.Add(new System.Web.Mail.MailAttachment(attachment));
								if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("Attached {0}", attachment) );
							}
						}
					}
				}
			}
			try {
				if ( log.IsDebugEnabled) log.Error ( "Sending message" );
				System.Web.Mail.SmtpMail.Send(mailMessage);
				if ( log.IsDebugEnabled) log.Error ( "Message sent" );
			} catch (System.Exception e) {
				error = true;
				message = e.Message;
#if DEBUG && !MONO
				message += ". <br>InnerException: " + e.InnerException.Message;
#endif
				if ( log.IsErrorEnabled ) log.Error ( "Error sending message", e );
				if ( log.IsErrorEnabled ) log.Error ( "Error sending message (InnerException)", e.InnerException );
			}
			mailMessage = null;
			return !error;
		}

		private void showAttachmentsPanel () {
			if ( Session["sharpwebmail/send/message/temppath"]!=null || Session["sharpwebmail/read/message/temppath"]!=null ) {
				this.newMessagePanel.Visible = false;
				this.sharpwebmailform.Enctype = "multipart/form-data";
				this.attachmentsPanel.Visible = true;
				if ( Session["sharpwebmail/send/message/temppath"]==null ) {
					this.newattachmentPH.Visible = false;
				}
			} else {
				showMessagePanel();
			}
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		private void showConfirmationPanel () {
			this.newMessagePanel.Visible = false;
			this.confirmationPanel.Visible = true;
			return;
		}

		/// <summary>
		/// 
		/// </summary>
		private void showMessagePanel () {
			System.Web.UI.WebControls.RegularExpressionValidator rev = (System.Web.UI.WebControls.RegularExpressionValidator) this.SharpUI.FindControl("toemailValidator");
			rev.ValidationExpression = @"^" + anmar.SharpMimeTools.ABNF.addr_spec + @"(;\s*" + anmar.SharpMimeTools.ABNF.addr_spec + @")*$";
			this.newMessageFromPanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("newMessageFromPanel");
#if !MONO
			if ( !this.IsPostBack ) {
#endif
				switch ( (int)Application["sharpwebmail/login/mode"] ) {
					case 2:
						this.newMessageFromPanel.Visible = true;
						rev = (System.Web.UI.WebControls.RegularExpressionValidator) this.SharpUI.FindControl("fromemailValidator");
						rev.ValidationExpression = "^" + anmar.SharpMimeTools.ABNF.addr_spec + "$";
						if ( this.fromemail.Value.Length==0 && Session["DisplayEmail"]!=null ) {
							this.fromemail.Value = Session["DisplayEmail"].ToString();
						}
						break;
					case 1:
					case 3:
					default:
						System.Web.UI.WebControls.Label newMessageWindowFromEmail = (System.Web.UI.WebControls.Label )this.SharpUI.FindControl("newMessageWindowFromEmail");
						newMessageWindowFromEmail.Text = User.Identity.Name;
						break;
				}
#if !MONO
			}
#endif
			this.newMessagePanel.Visible = true;
			return;
		}
		#endregion Private Methods
		
		#region Events
		/*
		 * Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void refreshPageButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			Response.Redirect("newmessage.aspx");
		}

		protected void AttachmentAdd_Click ( System.Object sender, System.EventArgs args ) {
			this.UI_case = 2;
			if ( this.newMessageWindowAttachFile.PostedFile!=null && Session["sharpwebmail/send/message/temppath"]!=null ) {
				System.String path = Session["sharpwebmail/send/message/temppath"].ToString();
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo ( path );
				try {
					dir.Create();
					if ( dir.GetFiles (System.IO.Path.GetFileName(this.newMessageWindowAttachFile.PostedFile.FileName) ).Length==0 )
						this.newMessageWindowAttachFile.PostedFile.SaveAs (System.IO.Path.Combine (path, System.IO.Path.GetFileName(this.newMessageWindowAttachFile.PostedFile.FileName)));
					this.bindAttachments();
				} catch ( System.Exception e ) {
					if ( log.IsErrorEnabled )
						log.Error ("", e);
				}
			}
		}

		protected void Attachments_Click ( System.Object sender, System.EventArgs args ) {
			this.UI_case = 2;
			if ( this.newMessageWindowAttachmentsList.Items.Count == 0 ) {
				this.bindAttachments();
			}
		}

		protected void Attach_Click ( System.Object sender, System.EventArgs args ) {
			this.UI_case = 0;
			System.Collections.ArrayList attachments = new System.Collections.ArrayList();
			foreach ( System.Web.UI.WebControls.ListItem item in this.newMessageWindowAttachmentsList.Items ) {
				if ( item.Selected ) {
					attachments.Add ( item );
				}
			}
			this.newMessageWindowAttachmentsAddedList.DataSource = attachments;
			this.newMessageWindowAttachmentsAddedList.DataBind();
		}
		protected void msgtoolbarCommand ( System.Object sender, System.Web.UI.WebControls.CommandEventArgs args ) {
			switch ( args.CommandName ) {
				case "cancel":
					Server.Transfer("newmessage.aspx", true);
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void Send_Click ( System.Object sender, System.EventArgs args ) {
			System.String message;
			if ( this.IsValid ) {
				this.UI_case = 1;
				if ( (int)Application["sharpwebmail/send/message/sanitizer_mode"]==1 ) {
					FCKEditor.Value = anmar.SharpWebMail.BasicSanitizer.SanitizeHTML(FCKEditor.Value, anmar.SharpWebMail.SanitizerMode.CommentBlocks|anmar.SharpWebMail.SanitizerMode.RemoveEvents);
				}
				if ( this.sendMail( out message ) ) {
					message = this.SharpUI.LocalizedRS.GetString("newMessageSent");
				}
			} else {
				message = this.SharpUI.LocalizedRS.GetString("newMessageValidationError");
			}
			newMessageWindowConfirmation.Text = message;
		}
		#endregion Events

		#region Page Events
		/*
		 * Page Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void Page_Load( System.Object sender, System.EventArgs args ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			this.mainInterface ( this.SharpUI.Inbox );
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Page_PreRender( System.Object sender, System.EventArgs args ) {
			switch ( UI_case ) {
				case 1:
					this.showConfirmationPanel();
					break;
				case 2:
					this.showAttachmentsPanel();
					break;
				default:
				case 0:
					this.showMessagePanel();
					break;
			}
		}
		#endregion Page Events
	}
}
