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

namespace anmar.SharpWebMail.UI
{
	public class newmessage : System.Web.UI.Page {

		#region General Fields
		// General Fields
		protected anmar.SharpWebMail.UI.globalUI SharpUI;
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static System.String bodyStart = "<html><head><title></title></head><body bgcolor=\"#FFFFFF\" text=\"#000000\" leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\">";
		private static System.String bodyEnd = "</body></html>";
		private  anmar.SharpMimeTools.SharpMimeHeader _headers=null;
		private int UI_case=0;
		private anmar.SharpWebMail.UI.MessageMode _message_mode = anmar.SharpWebMail.UI.MessageMode.None;
		
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

		//PlaceHolders
		protected System.Web.UI.WebControls.PlaceHolder attachmentsPH;
		protected System.Web.UI.WebControls.PlaceHolder confirmationPH;
		protected System.Web.UI.WebControls.PlaceHolder newMessageFromPH;
		protected System.Web.UI.WebControls.PlaceHolder newMessagePH;

		protected System.Web.UI.WebControls.PlaceHolder newattachmentPH;

		//Other form elements
		protected System.Web.UI.HtmlControls.HtmlInputFile newMessageWindowAttachFile;
		protected System.Web.UI.WebControls.CheckBoxList newMessageWindowAttachmentsList;
		protected System.Web.UI.WebControls.DataList newMessageWindowAttachmentsAddedList;

		#endregion UI variables

		#region Private Methods

		protected void AttachmentSelect ( System.String key ) {
			foreach ( System.Web.UI.WebControls.ListItem item in this.newMessageWindowAttachmentsList.Items ) {
				if ( item.Value.Equals(key) ) {
					item.Selected = true;
					break;
				}
			}
		}
		protected void bindAttachments () {
			System.Collections.ArrayList selected = null;
			System.Collections.SortedList attachments = new System.Collections.SortedList();
			bindAttachments ( attachments, Session["sharpwebmail/read/message/temppath"] );
			if ( !Session["sharpwebmail/read/message/temppath"].Equals(Session["sharpwebmail/send/message/temppath"]) )
				bindAttachments ( attachments, Session["sharpwebmail/send/message/temppath"] );
			if ( this.newMessageWindowAttachmentsList.SelectedIndex!=-1 ) {
				selected = new System.Collections.ArrayList();
				foreach ( System.Web.UI.WebControls.ListItem item in this.newMessageWindowAttachmentsList.Items ) {
					if ( item.Selected )
						selected.Add(item.Value);
				}
			}

			this.newMessageWindowAttachmentsList.DataSource = attachments;
			this.newMessageWindowAttachmentsList.DataTextField = "Value";
			this.newMessageWindowAttachmentsList.DataValueField = "Key";
			this.newMessageWindowAttachmentsList.DataBind();
			if ( selected!=null ) {
				foreach ( System.String itemselected in selected ) {
					this.AttachmentSelect(itemselected);
				}
			}
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
		private System.String GetFromAddress () {
			System.String from = null;
			switch ( (int)Application["sharpwebmail/login/mode"] ) {
				case 2:
					from = this.fromemail.Value.Trim();
					break;
				case 1:
				case 3:
				default:
					from = User.Identity.Name;
					break;
			}
			return from;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void mainInterface ( anmar.SharpWebMail.CTNInbox inbox ) {
			this.newattachmentPH=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("newattachmentPH");
			this.attachmentsPH=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("attachmentsPH");
			this.confirmationPH=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("confirmationPH");
			this.newMessagePH=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("newMessagePH");
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

			// Disable PlaceHolders
			this.attachmentsPH.Visible = false;
			this.confirmationPH.Visible = false;

			// Disable some things
			this.SharpUI.nextPageImageButton.Enabled = false;
			this.SharpUI.prevPageImageButton.Enabled = false;
			// Get mode
			if ( Page.Request.QueryString["mode"]!=null ) {
				try {
					this._message_mode = (anmar.SharpWebMail.UI.MessageMode)System.Enum.Parse(typeof(anmar.SharpWebMail.UI.MessageMode), Page.Request.QueryString["mode"], true);
				} catch ( System.Exception ){}
			}
			// Get message ID
			System.String msgid = System.Web.HttpUtility.HtmlEncode (Page.Request.QueryString["msgid"]);
			System.Guid guid = System.Guid.Empty;
			if ( msgid!=null )
				guid = new System.Guid(msgid);
			if ( !this.IsPostBack && !guid.Equals( System.Guid.Empty) ) {
				System.Object[] details = inbox[ guid ];
				if ( details!=null ) {
					if ( this._message_mode.Equals(anmar.SharpWebMail.UI.MessageMode.None) )
						this._message_mode = anmar.SharpWebMail.UI.MessageMode.reply;
					this._headers = (anmar.SharpMimeTools.SharpMimeHeader) details[13];
					if ( !this.IsPostBack ) {
						bool html_content = this.FCKEditor.CheckBrowserCompatibility();
						
						this.subject.Value = System.String.Concat (this.SharpUI.LocalizedRS.GetString(System.String.Concat(this._message_mode, "Prefix")), ":");
						if ( details[10].ToString().ToLower().IndexOf (this.subject.Value.ToLower())!=-1 ) {
							this.subject.Value = details[10].ToString().Trim();
						} else {
							this.subject.Value = System.String.Concat (this.subject.Value, " ", details[10]).Trim();
						}
						// Get the original message
						inbox.CurrentFolder = details[18].ToString();
						System.IO.MemoryStream ms = inbox.GetMessage((int)details[1], msgid);
						anmar.SharpMimeTools.SharpMessage message = null;
						if ( ms!=null && ms.CanRead ) {
							System.String path = null;
							if ( this._message_mode.Equals(anmar.SharpWebMail.UI.MessageMode.forward) ) {
								path = Session["sharpwebmail/read/message/temppath"].ToString();
								path = System.IO.Path.Combine (path, msgid);
								path = System.IO.Path.GetFullPath(path);
							}
							bool attachments = false;
							if ( this._message_mode.Equals(anmar.SharpWebMail.UI.MessageMode.forward) )
								attachments = (bool)Application["sharpwebmail/send/message/forwardattachments"];
							message = new anmar.SharpMimeTools.SharpMessage(ms, attachments, html_content, path);
							ms.Close();
						}
						ms = null;
						if ( this._message_mode.Equals(anmar.SharpWebMail.UI.MessageMode.reply) ) {
							// From name if present on original message's To header
							// and we don't have it already
							if ( Session["DisplayName"]==null ) {
								foreach ( anmar.SharpMimeTools.SharpMimeAddress address in (System.Collections.IEnumerable) details[8] ) {
									if ( address["address"]!=null && address["address"].Equals( User.Identity.Name )
										&& address["name"].Length>0 && !address["address"].Equals(address["name"]) ) {
										this.fromname.Value = address["name"];
										break;
									}
								}
							}
							// To addresses
							foreach ( anmar.SharpMimeTools.SharpMimeAddress address in (System.Collections.IEnumerable) details[9] ) {
								if ( address["address"]!=null && !address["address"].Equals( User.Identity.Name ) ) {
									if ( this.toemail.Value.Length >0 )
										this.toemail.Value += ", ";
									this.toemail.Value += address["address"];
								}
							}
						} else if ( this._message_mode.Equals(anmar.SharpWebMail.UI.MessageMode.forward) ) {
							// If the original message has attachments, preserve them
							if ( message!=null && message.Attachments!=null &&  message.Attachments.Count>0 ) {
								this.bindAttachments();
								foreach ( anmar.SharpMimeTools.SharpAttachment attachment in message.Attachments ) {
									if ( attachment.SavedFile==null )
										continue;
									this.AttachmentSelect(System.IO.Path.Combine(attachment.SavedFile.Directory.Name, attachment.SavedFile.Name));
								}
								this.Attach_Click(this, null);
							}
						}
						// Preserve the original body and some properties
						if ( message!=null ) {
							System.Text.StringBuilder sb_body = new System.Text.StringBuilder();
							System.String line_end = null;
							if ( html_content )
								line_end = "<br />\r\n";
							else
								line_end = "\r\n";
							sb_body.Append(this.SharpUI.LocalizedRS.GetString(System.String.Concat(this._message_mode, "PrefixBody")));
							sb_body.Append(line_end);
							sb_body.Append(this.SharpUI.LocalizedRS.GetString("newMessageWindowFromNameLabel"));
							sb_body.Append(" ");
							sb_body.Append(message.From);
							sb_body.Append(" [mailto:");
							sb_body.Append(message.FromAddress);
							sb_body.Append("]");
							sb_body.Append(line_end);
							sb_body.Append(this.SharpUI.LocalizedRS.GetString("newMessageWindowDateLabel"));
							sb_body.Append(" ");
							sb_body.Append(message.Date.ToString());
							sb_body.Append(line_end);
							sb_body.Append(this.SharpUI.LocalizedRS.GetString("newMessageWindowToEmailLabel"));
							sb_body.Append(" ");
							if ( html_content )
								sb_body.Append(System.Web.HttpUtility.HtmlEncode(message.To.ToString()));
							else
								sb_body.Append(message.To);
							sb_body.Append(line_end);
							sb_body.Append(this.SharpUI.LocalizedRS.GetString("newMessageWindowSubjectLabel"));
							sb_body.Append(" ");
							sb_body.Append(message.Subject);
							sb_body.Append(line_end);
							sb_body.Append(line_end);
							if ( !message.HasHtmlBody &&  html_content )
								sb_body.Append("<pre>");
							sb_body.Append(message.Body);
							if ( !message.HasHtmlBody &&  html_content )
								sb_body.Append("</pre>");
							if ( this._message_mode.Equals(anmar.SharpWebMail.UI.MessageMode.reply) ) {
								if ( html_content ) {
									sb_body.Insert(0, System.String.Concat("<blockquote style=\"", Application["sharpwebmail/send/message/replyquotestyle"] ,"\">"));
									sb_body.Append("</blockquote>");
								} else {
									sb_body.Insert(0, Application["sharpwebmail/send/message/replyquotechar"].ToString());
									sb_body.Replace("\n", System.String.Concat("\n", Application["sharpwebmail/send/message/replyquotechar"]));
								}
							}
							sb_body.Insert(0, line_end);
							sb_body.Insert(0, " ");
							this.FCKEditor.Value = sb_body.ToString();
						}
					}
					details = null;
				}
			} else if ( !this.IsPostBack ) {
				System.String to = Page.Request.QueryString["to"];
				if ( to!=null && to.Length>0 ) {
					this.toemail.Value = to;
				}
			}
			if ( this.fromname.Value.Length>0 || this.IsPostBack )
				Session["DisplayName"] = this.fromname.Value;
			if ( this.fromname.Value.Length==0 && Session["DisplayName"]!=null )
				this.fromname.Value = Session["DisplayName"].ToString();
			if ( this.fromemail.Value.Length>0 || this.IsPostBack )
				Session["DisplayEmail"] = this.fromemail.Value;
		}
		private void ProcessMessageAttachments (System.Object message) {
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
								if ( message is System.Web.Mail.MailMessage )
									((System.Web.Mail.MailMessage)message).Attachments.Add(new System.Web.Mail.MailAttachment(attachment));
								else if ( message is DotNetOpenMail.EmailMessage )
									((DotNetOpenMail.EmailMessage)message).AddMixedAttachment(new DotNetOpenMail.FileAttachment(new System.IO.FileInfo(attachment)));
								else if ( message is OpenSmtp.Mail.MailMessage )
									((OpenSmtp.Mail.MailMessage)message).AddAttachment(attachment);
								if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("Attached {0}", attachment) );
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private System.String SendMail ( anmar.SharpWebMail.EmailServerInfo server ) {
			System.String message = null;
			System.Web.Mail.SmtpMail.SmtpServer = server.Host;

			System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
			mailMessage.To = this.toemail.Value;
			mailMessage.From = System.String.Format("{0} <{1}>", this.fromname.Value, this.GetFromAddress());
			mailMessage.Subject = this.subject.Value.Trim();
			System.String format = Request.Form["format"];
			if ( format!=null && format.Equals("html") ) {
				mailMessage.BodyFormat = System.Web.Mail.MailFormat.Html;
				mailMessage.Body = bodyStart + FCKEditor.Value + bodyEnd;
			} else {
				mailMessage.BodyFormat = System.Web.Mail.MailFormat.Text;
				mailMessage.Body = FCKEditor.Value;
			}

			if ( this._headers != null ) {
				// RFC 2822 3.6.4. Identification fields
				if ( this._headers["Message-ID"]!=null ) {
					mailMessage.Headers["In-Reply-To"] = this._headers["Message-ID"];
					mailMessage.Headers["References"] = this._headers["Message-ID"];
				}
				if ( this._headers["References"]!=null ) {
					mailMessage.Headers["References"] = System.String.Concat (this._headers["References"], " ", mailMessage.Headers["References"]).Trim();
				} else if ( this._headers["In-Reply-To"]!=null && this._headers["In-Reply-To"].IndexOf('>')==this._headers["In-Reply-To"].LastIndexOf('>') ) {
					mailMessage.Headers["References"] = System.String.Concat (this._headers["In-Reply-To"], " ", mailMessage.Headers["References"]).Trim();
				}
			}
			mailMessage.Headers["X-Mailer"] = System.String.Format ("{0} {1}", Application["product"], Application["version"]);
			this.ProcessMessageAttachments(mailMessage);
			try {
				if ( log.IsDebugEnabled) log.Debug (System.String.Concat("Sending message. engine: internal , protocol: ", server.Protocol));
				System.Web.Mail.SmtpMail.Send(mailMessage);
				if ( log.IsDebugEnabled) log.Debug ( "Message sent" );
			} catch (System.Exception e) {
				message = e.Message;
#if DEBUG && !MONO
				message += ". <br>InnerException: " + e.InnerException.Message;
#endif
				if ( log.IsErrorEnabled ) log.Error ( System.String.Concat("Error sending message. engine: internal , protocol: ", server.Protocol), e );
				if ( log.IsErrorEnabled ) log.Error ( "Error sending message (InnerException)", e.InnerException );
			}
			mailMessage = null;
			return message;
		}
		private System.String SendMailDotNetOpenMail ( anmar.SharpWebMail.EmailServerInfo server ) {
			System.String message = null;
			System.Text.Encoding encoding = (System.Text.Encoding)Application["sharpwebmail/send/message/charset"];
			DotNetOpenMail.EmailMessage mailMessage = new DotNetOpenMail.EmailMessage();
			mailMessage.HeaderCharSet = encoding;
			mailMessage.FromAddress = new DotNetOpenMail.EmailAddress(this.GetFromAddress(), this.fromname.Value);
			System.String[] tokens = anmar.SharpMimeTools.ABNF.address_regex.Split(this.toemail.Value);
			foreach ( System.String token in tokens ) {
				if ( anmar.SharpMimeTools.ABNF.address_regex.IsMatch(token ) )
					mailMessage.AddToAddress (new DotNetOpenMail.EmailAddress(token.Trim()));
			}
			mailMessage.Subject = this.subject.Value.Trim();
			System.String format = Request.Form["format"];
			if ( format!=null && format.Equals("html") ) {
				mailMessage.HtmlPart = new DotNetOpenMail.HtmlAttachment(bodyStart + FCKEditor.Value + bodyEnd);
				mailMessage.HtmlPart.CharSet = encoding;
			} else {
				mailMessage.TextPart = new DotNetOpenMail.TextAttachment(FCKEditor.Value);
				mailMessage.TextPart.CharSet = encoding;
			}

			if ( this._headers != null ) {
				// RFC 2822 3.6.4. Identification fields
				if ( this._headers["Message-ID"]!=null ) {
					mailMessage.AddCustomHeader("In-Reply-To", this._headers["Message-ID"]);
					mailMessage.AddCustomHeader("References", this._headers["Message-ID"]);
				}
				if ( this._headers["References"]!=null ) {
					mailMessage.AddCustomHeader("References", System.String.Concat (this._headers["References"], " ", this._headers["Message-ID"]).Trim());
				} else if ( this._headers["In-Reply-To"]!=null && this._headers["In-Reply-To"].IndexOf('>')==this._headers["In-Reply-To"].LastIndexOf('>') ) {
					mailMessage.AddCustomHeader("References", System.String.Concat (this._headers["In-Reply-To"], " ", this._headers["Message-ID"]).Trim());
				}
			}
			mailMessage.XMailer = System.String.Concat (Application["product"], " ", Application["version"]);
			this.ProcessMessageAttachments(mailMessage);
			try {
				if ( log.IsDebugEnabled) log.Debug (System.String.Concat("Sending message. engine: DotNetOpenMail , protocol: ", server.Protocol));
				DotNetOpenMail.SmtpServer SmtpMail = new DotNetOpenMail.SmtpServer(server.Host, server.Port);
				if ( server.Protocol.Equals(anmar.SharpWebMail.ServerProtocol.SmtpAuth) ) {
					anmar.SharpWebMail.IEmailClient client = (anmar.SharpWebMail.IEmailClient)Session["client"];
					SmtpMail.SmtpAuthToken = new DotNetOpenMail.SmtpAuth.SmtpAuthToken(client.UserName, client.Password);
				}
				mailMessage.Send(SmtpMail);
				SmtpMail=null;
				if ( log.IsDebugEnabled) log.Debug ( "Message sent" );
			} catch (System.Exception e) {
				message = e.Message;
				if ( log.IsErrorEnabled ) log.Error ( System.String.Concat("Error sending message. engine: DotNetOpenMail , protocol: ", server.Protocol), e );
			}
			mailMessage = null;
			return message;
		}
		private System.String SendMailOpenSmtp ( anmar.SharpWebMail.EmailServerInfo server ) {
			System.String message = null;
			System.Text.Encoding encoding = (System.Text.Encoding)Application["sharpwebmail/send/message/charset"];
			OpenSmtp.Mail.MailMessage mailMessage = new OpenSmtp.Mail.MailMessage();
			mailMessage.Charset = encoding.HeaderName;
			mailMessage.From = new OpenSmtp.Mail.EmailAddress(this.GetFromAddress(), this.fromname.Value);
			System.String[] tokens = anmar.SharpMimeTools.ABNF.address_regex.Split(this.toemail.Value);
			foreach ( System.String token in tokens ) {
				if ( anmar.SharpMimeTools.ABNF.address_regex.IsMatch(token ) )
					mailMessage.To.Add (new OpenSmtp.Mail.EmailAddress(token.Trim()));
			}
			mailMessage.Subject = this.subject.Value.Trim();
			System.String format = Request.Form["format"];
			if ( format!=null && format.Equals("html") ) {
				mailMessage.HtmlBody = bodyStart + FCKEditor.Value + bodyEnd;
			} else {
				mailMessage.Body = FCKEditor.Value;
			}

			if ( this._headers != null ) {
				// RFC 2822 3.6.4. Identification fields
				OpenSmtp.Mail.MailHeader references = new OpenSmtp.Mail.MailHeader("References", System.String.Empty);
				if ( this._headers["Message-ID"]!=null ) {
					mailMessage.AddCustomHeader("In-Reply-To", this._headers["Message-ID"]);
					references.Body = this._headers["Message-ID"];
				}
				if ( this._headers["References"]!=null ) {
					references.Body = System.String.Concat(this._headers["References"], " ", references.Body).Trim();
				} else if ( this._headers["In-Reply-To"]!=null && this._headers["In-Reply-To"].IndexOf('>')==this._headers["In-Reply-To"].LastIndexOf('>') ) {
					references.Body = System.String.Concat(this._headers["In-Reply-To"], " ", references.Body).Trim();
				}
				if ( !references.Body.Equals(System.String.Empty) )
					mailMessage.AddCustomHeader(references);
			}
			mailMessage.AddCustomHeader("X-Mailer", System.String.Concat (Application["product"], " ", Application["version"]));
			this.ProcessMessageAttachments(mailMessage);
			try {
				if ( log.IsDebugEnabled) log.Debug (System.String.Concat("Sending message. engine: opensmtp , protocol: ", server.Protocol));
				OpenSmtp.Mail.Smtp SmtpMail = null;
				if ( server.Protocol.Equals(anmar.SharpWebMail.ServerProtocol.SmtpAuth) ) {
					anmar.SharpWebMail.IEmailClient client = (anmar.SharpWebMail.IEmailClient)Session["client"];
					SmtpMail = new OpenSmtp.Mail.Smtp(server.Host, client.UserName, client.Password, server.Port);
				} else
					SmtpMail = new OpenSmtp.Mail.Smtp(server.Host, server.Port);
				SmtpMail.SendMail(mailMessage);
				SmtpMail=null;
				if ( log.IsDebugEnabled) log.Debug ( "Message sent" );
			} catch (System.Exception e) {
				message = e.Message;
				if ( log.IsErrorEnabled ) log.Error ( System.String.Concat("Error sending message. engine: opensmtp , protocol: ", server.Protocol), e );
			}
			mailMessage = null;
			return message;
		}

		private void showAttachmentsPanel () {
			if ( Session["sharpwebmail/send/message/temppath"]!=null || Session["sharpwebmail/read/message/temppath"]!=null ) {
				this.newMessagePH.Visible = false;
				this.sharpwebmailform.Enctype = "multipart/form-data";
				this.attachmentsPH.Visible = true;
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
			this.newMessagePH.Visible = false;
			this.confirmationPH.Visible = true;
			return;
		}

		/// <summary>
		/// 
		/// </summary>
		private void showMessagePanel () {
			System.Web.UI.WebControls.RegularExpressionValidator rev = (System.Web.UI.WebControls.RegularExpressionValidator) this.SharpUI.FindControl("toemailValidator");
			rev.ValidationExpression = @"^" + anmar.SharpMimeTools.ABNF.addr_spec + @"(,\s*" + anmar.SharpMimeTools.ABNF.addr_spec + @")*$";
			this.newMessageFromPH=(System.Web.UI.WebControls.PlaceHolder )this.SharpUI.FindControl("newMessageFromPH");

			if ( !this.IsPostBack ) {
				if ( Application["sharpwebmail/send/addressbook"]!=null ) {
					System.Collections.SortedList addressbooks = (System.Collections.SortedList)Application["sharpwebmail/send/addressbook"];
					if ( addressbooks.Count>0 ) {
						System.Web.UI.WebControls.HyperLink addressbook = (System.Web.UI.WebControls.HyperLink)this.SharpUI.FindControl("newMessageWindowToEmailLabel");
						addressbook.NavigateUrl = "javascript:window.open('addressbook.aspx', 'addressbook', 'width=400, height=400, resizable=yes, scrollbars=yes');void(true);";
						addressbook = (System.Web.UI.WebControls.HyperLink)this.SharpUI.FindControl("msgtoolbarAddressBook");
						addressbook.NavigateUrl = "javascript:window.open('addressbook.aspx', 'addressbook', 'width=400, height=400, resizable=yes, scrollbars=yes');void(true);";
						addressbook.Visible = true;
					}
				}
				switch ( (int)Application["sharpwebmail/login/mode"] ) {
					case 2:
						this.newMessageFromPH.Visible = true;
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
			}
			this.newMessagePH.Visible = true;
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
					System.String filename = System.IO.Path.GetFileName(this.newMessageWindowAttachFile.PostedFile.FileName);
					if ( dir.GetFiles (filename).Length==0 ) {
						this.newMessageWindowAttachFile.PostedFile.SaveAs (System.IO.Path.Combine (path, filename));
						this.bindAttachments();
						foreach ( System.Web.UI.WebControls.ListItem item in this.newMessageWindowAttachmentsList.Items ) {
							if ( item.Value.Equals(System.IO.Path.Combine(dir.Name, filename)) )
								item.Selected = true;
						}
						if ( Application["sharpwebmail/send/message/attach_ui"].Equals("simple") )
							this.Attach_Click(sender, args);
					}
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
					Server.Transfer(System.String.Concat("newmessage.aspx?", Request.QueryString), false);
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void Send_Click ( System.Object sender, System.EventArgs args ) {
			System.String message = null;
			if ( this.IsValid ) {
				this.UI_case = 1;
				if ( (int)Application["sharpwebmail/send/message/sanitizer_mode"]==1 ) {
					FCKEditor.Value = anmar.SharpWebMail.BasicSanitizer.SanitizeHTML(FCKEditor.Value, anmar.SharpWebMail.SanitizerMode.CommentBlocks|anmar.SharpWebMail.SanitizerMode.RemoveEvents);
				}
				anmar.SharpWebMail.ServerSelector selector = (anmar.SharpWebMail.ServerSelector)Application["sharpwebmail/send/servers"];
				anmar.SharpWebMail.EmailServerInfo server = selector.Select(User.Identity.Name, true);
				if ( server!=null && ( server.Protocol.Equals(anmar.SharpWebMail.ServerProtocol.Smtp) 
				                      || server.Protocol.Equals(anmar.SharpWebMail.ServerProtocol.SmtpAuth) ) ) {
					if ( Application["sharpwebmail/send/message/smtp_engine"].Equals("opensmtp") )
						message = this.SendMailOpenSmtp(server);
					else if ( Application["sharpwebmail/send/message/smtp_engine"].Equals("dotnetopenmail") )
						message = this.SendMailDotNetOpenMail(server);
					else
						message = this.SendMail( server );
	
					if ( message==null )
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
	public enum MessageMode {
		None,
		reply,
		forward
	}
}
