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
		protected System.Web.UI.HtmlControls.HtmlInputText fromname;
		protected System.Web.UI.HtmlControls.HtmlInputText subject;
		protected System.Web.UI.HtmlControls.HtmlInputText toemail;

		// Labels
		protected System.Web.UI.WebControls.Label email;
		protected System.Web.UI.WebControls.Label newMessageWindowConfirmation;

		//Editor
		protected FredCK.FCKeditor FCKEditor; 

		//Panels
		protected System.Web.UI.WebControls.Panel attachmentsPanel;
		protected System.Web.UI.WebControls.Panel newMessagePanel;
		protected System.Web.UI.WebControls.Panel confirmationPanel;

		//Other form elements
		protected System.Web.UI.HtmlControls.HtmlInputFile newMessageWindowAttachFile;
		protected System.Web.UI.WebControls.CheckBoxList newMessageWindowAttachmentsList;
		protected System.Web.UI.WebControls.DataList newMessageWindowAttachmentsAddedList;

		#endregion UI variables

		#region Private Methods

		protected void bindAttachments () {
			System.Collections.SortedList attachments = new System.Collections.SortedList();
			if ( Session["temppath"]!=null ) {
				System.String path = Session["temppath"].ToString();
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
			this.newMessageWindowAttachmentsList.DataSource = attachments;
			this.newMessageWindowAttachmentsList.DataTextField = "Value";
			this.newMessageWindowAttachmentsList.DataValueField = "Key";
			this.newMessageWindowAttachmentsList.DataBind();
		}
		private System.String getfilename ( params System.String[] parts ) {
			System.String path = null;
			if ( Session["temppath"]!=null ) {
				path = Session["temppath"].ToString();
				System.IO.DirectoryInfo basedir = new System.IO.DirectoryInfo(path);
				if ( basedir.Exists ) {
					foreach ( System.String part in parts ) {
						path = System.IO.Path.Combine (path, part);
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
		/// <summary>
		/// 
		/// </summary>
		protected void mainInterface ( anmar.SharpWebMail.CTNInbox inbox ) {
			this.attachmentsPanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("attachmentsPanel");
			this.confirmationPanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("confirmationPanel");
			this.newMessagePanel=(System.Web.UI.WebControls.Panel )this.SharpUI.FindControl("newMessagePanel");
			this.newMessageWindowAttachFile=( System.Web.UI.HtmlControls.HtmlInputFile )this.SharpUI.FindControl("newMessageWindowAttachFile");
			this.newMessageWindowAttachmentsList=(System.Web.UI.WebControls.CheckBoxList )this.SharpUI.FindControl("newMessageWindowAttachmentsList");
			this.newMessageWindowAttachmentsAddedList=(System.Web.UI.WebControls.DataList )this.SharpUI.FindControl("newMessageWindowAttachmentsAddedList");

			this.FCKEditor = (FredCK.FCKeditor)this.SharpUI.FindControl("FCKEditor");
			this.fromname = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("fromname");
			this.subject = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("subject");
			this.toemail = (System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("toemail");
			
			this.newMessageWindowConfirmation = (System.Web.UI.WebControls.Label)this.SharpUI.FindControl("newMessageWindowConfirmation");

			this.SharpUI.refreshPageButton.Click += new System.Web.UI.ImageClickEventHandler(refreshPageButton_Click);

			// Disable Panels
			this.attachmentsPanel.Visible = false;
			this.confirmationPanel.Visible = false;
			this.newMessagePanel.Visible = false;

			// Disable some things
			this.SharpUI.nextPageButton.Enabled = false;
			this.SharpUI.prevPageButton.Enabled = false;

			System.String msgid = Page.Request.QueryString["msgid"];
			if ( msgid != null ) {
				System.Object[] details = inbox[ msgid ];
				if ( details != null ) {
					this.Header = (anmar.SharpMimeTools.SharpMimeHeader) details[13];
					if ( !this.IsPostBack ) {
						this.subject.Value = System.String.Format ("{0}:", this.SharpUI.LocalizedRS.GetString("replyPrefix"));
						if ( details[10].ToString().ToLower().IndexOf (this.subject.Value.ToLower())!=-1 ) {
							this.subject.Value = details[10].ToString().Trim();
						} else {
							this.subject.Value = System.String.Format ("{0} {1}", this.subject.Value, details[10].ToString()).Trim();
						}
						// From name if present on original message
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
									this.toemail.Value += ", ";
								this.toemail.Value += address["address"];
								break;
							}
						}
						this.FCKEditor.CanUpload = FredCK.EnablePropertyValues.False;
						this.FCKEditor.CanBrowse = FredCK.EnablePropertyValues.False;
					}
					details = null;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private bool sendMail ( out System.String message ) {
			bool error = false;
			message = null;

			System.Web.Mail.SmtpMail.SmtpServer = (System.String)Application["mail_server_smtp"]; //+ ":" + Application["mail_server_smtp_port"];

			System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
			mailMessage.To = this.toemail.Value;
			//mailMessage.Cc = this.TextBoxCc.Text;
			//mailMessage.Bcc = this.TextBoxBcc.Text;
			mailMessage.From = "\"" + this.fromname.Value + "\" <" + User.Identity.Name + ">";
			mailMessage.Subject = this.subject.Value;
			mailMessage.Body = bodyStart + FCKEditor.Value + bodyEnd;
			mailMessage.BodyFormat = System.Web.Mail.MailFormat.Html;

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
				System.String Value;
				foreach ( System.Web.UI.WebControls.ListItem item in this.newMessageWindowAttachmentsList.Items ) {
					if ( item.Selected ) {
						if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("Attaching {0} {1}", item.Text, item.Value) );
						if ( item.Value.StartsWith ( Session.SessionID ) ) {
							mailMessage.Attachments.Add(new System.Web.Mail.MailAttachment(this.getfilename (item.Text.Substring(0, item.Text.LastIndexOf(" (")))));
							if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("Attached {0}", this.getfilename (item.Text.Substring(0, item.Text.LastIndexOf(" (")))) );
						} else {
							Value = item.Value.Substring ( 0 , item.Value.IndexOf ( System.IO.Path.DirectorySeparatorChar ));
							System.Object[] details = inbox[ Value ];
							if ( details != null ) {
								mailMessage.Attachments.Add(new System.Web.Mail.MailAttachment(this.getfilename (Value, item.Text.Substring(0, item.Text.LastIndexOf(" (")))));
								if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("Attached {0}", this.getfilename (Value, item.Text.Substring(0, item.Text.LastIndexOf(" (")))));
								details = null;
							}
						}
					}
				}
				inbox = null;
			}
			try {
				if ( log.IsDebugEnabled) log.Error ( "Sending message" );
				System.Web.Mail.SmtpMail.Send(mailMessage);
				if ( log.IsDebugEnabled) log.Error ( "Message sent" );
			} catch (System.Exception e) {
				error = true;
				message = e.Message;
				if ( log.IsErrorEnabled ) log.Error ( "Error sending message", e );
			}
			mailMessage = null;
			return !error;
		}

		private void showAttachmentsPanel () {
			if ( Session["temppath"]!=null ) {
				this.sharpwebmailform.Enctype = "multipart/form-data";
				this.attachmentsPanel.Visible = true;
			} else {
				showMessagePanel();
			}
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		private void showConfirmationPanel () {
			this.confirmationPanel.Visible = true;
			return;
		}

		/// <summary>
		/// 
		/// </summary>
		private void showMessagePanel () {
			// Set labels localized texts
			this.email=(System.Web.UI.WebControls.Label )this.SharpUI.FindControl("email");
			this.email.Text = User.Identity.Name;
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
		protected void refreshPageButton_Click (Object sender, System.Web.UI.ImageClickEventArgs e) {
			Response.Redirect("newmessage.aspx");
		}

		protected void AttachmentAdd_Click ( System.Object sender, System.EventArgs E ) {
			this.UI_case = 2;
			if ( this.newMessageWindowAttachFile.PostedFile!=null && Session["temppath"]!=null ) {
				System.String path = Session["temppath"].ToString();
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

		protected void Attachments_Click ( System.Object sender, System.EventArgs E ) {
			this.UI_case = 2;
			if ( this.newMessageWindowAttachmentsList.Items.Count == 0 ) {
				this.bindAttachments();
			}
		}

		protected void Attach_Click ( System.Object sender, System.EventArgs E ) {
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

		/// <summary>
		/// 
		/// </summary>
		protected void Send_Click ( System.Object sender, System.EventArgs E ) {
			this.UI_case = 1;
			System.String message;
			if ( this.IsValid ) {
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
		protected void Page_Load(System.Object Src, System.EventArgs E ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			//Our Inbox
			anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];

			this.mainInterface ( inbox );

			inbox = null;

		}

		/// <summary>
		/// 
		/// </summary>
		protected void Page_PreRender(object sender, EventArgs e) {
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
