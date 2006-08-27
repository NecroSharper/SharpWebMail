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
	public class AddressBookEdit : System.Web.UI.Page {
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected anmar.SharpWebMail.UI.globalUI SharpUI;
		// Input boxes
		private System.Web.UI.HtmlControls.HtmlInputText _email;
		private System.Web.UI.HtmlControls.HtmlInputText _name;
		private System.Web.UI.WebControls.Label _addressbook_entry_title;
		private System.String _entry;
		private System.String _book_name;
		private bool _update = false;

		private System.Data.DataTable GetData () {
			if ( this._book_name!=null && this._book_name.Length>0 ) {
				System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this._book_name, Application["sharpwebmail/send/addressbook"]);
				if ( addressbook!=null )
					return anmar.SharpWebMail.UI.AddressBook.GetDataSource(addressbook, false, Session["client"] as anmar.SharpWebMail.IEmailClient );
			}
			return null;
		}
		private void SetMessage (System.String msg) {
			System.String message = this.SharpUI.LocalizedRS.GetString(msg);
			if ( message!=null && message.Length>0 ) {
				System.Web.UI.Control holder = this.SharpUI.FindControl("ConfirmationPH");
				holder.Visible = true;
				System.Web.UI.WebControls.Label label = (System.Web.UI.WebControls.Label)holder.FindControl("ConfirmationMessage");
				label.Text = message;
			}
		}
		protected void AddressInsert_Click ( System.Object sender, System.EventArgs args ) {
			if ( !this.IsValid )
				return;
			bool error = false;
			System.Data.DataTable data = GetData();
			if ( data!=null ) {
				try {
					data.Rows.Add(new object[]{this._name.Value, this._email.Value, this._book_name, this.User.Identity.Name});
				} catch ( System.Exception e ) {
					if ( log.IsErrorEnabled )
						log.Error(System.String.Concat("Error inserting record [", this._email.Value, "] in addressbook [", this._book_name, "] for user [", this.User.Identity.Name, "]"), e);
					error = true;
				}
				System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this._book_name, Application["sharpwebmail/send/addressbook"]);
				if ( !error && anmar.SharpWebMail.UI.AddressBook.UpdateDataSource(data, addressbook, Session["client"] as anmar.SharpWebMail.IEmailClient ) ) {
					data = null;
					this._email.Value = System.String.Empty;
					this._name.Value = System.String.Empty;
				} else
					error = true;
				data = null;
			}
			if ( error )
				this.SetMessage("addressbookEntryInsertError");
			
		}
		protected void AddressToolbarCommand ( System.Object sender, System.Web.UI.WebControls.CommandEventArgs args ) {
			switch ( args.CommandName ) {
				case "delete":
					bool error = false;
					System.Data.DataTable data = GetData();
					if ( data!=null && data.Rows.Count>0 ) {
						System.Data.DataView view = data.DefaultView;
						view.AllowDelete = true;
						view.RowFilter = System.String.Concat(data.Columns[1].ColumnName, "='", this._entry, "'");
						 if ( view.Count==1 ) {
							view[0].Delete();
							System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this._book_name, Application["sharpwebmail/send/addressbook"]);
							if ( anmar.SharpWebMail.UI.AddressBook.UpdateDataSource(data, addressbook, Session["client"] as anmar.SharpWebMail.IEmailClient) ) {
								this._update = false;
								data = null;
								Response.Redirect(System.String.Concat("addressbookfull.aspx?book=", this._book_name));
							} else
								error = true;
						} else
							error = true;
					} else
						error = true;
					if ( error )
						this.SetMessage("addressbookEntryDeleteError");
					data = null;
					break;
			}
		}
		protected void AddressUpdate_Click ( System.Object sender, System.EventArgs args ) {
			if ( !this.IsValid )
				return;
			bool error = false;
			bool update = false;
			System.Data.DataTable data = GetData();
			if ( data!=null && data.Rows.Count>0 ) {
				System.Data.DataView view = data.DefaultView;
				view.AllowEdit = true;
				view.RowFilter = System.String.Concat(data.Columns[1].ColumnName, "='", this._entry, "'");
				 if ( view.Count==1 ) {
					try {
						if ( !view[0][0].Equals(this._name.Value) ) {
							view[0][0] = this._name.Value;
							update = true;
						}
						if ( !view[0][1].Equals(this._email.Value) ) {
							view[0][1] = this._email.Value;
							update = true;
						}
					} catch ( System.Exception e ) {
						if ( log.IsErrorEnabled )
							log.Error(System.String.Concat("Error updating record [", this._entry, "]in addressbook [", this._book_name, "] for user [", this.User.Identity.Name, "]"), e);
						error = true;
						update = false;
					}
				} else
					error = true;
				if ( update ) {
					System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this._book_name, Application["sharpwebmail/send/addressbook"]);
					error = !anmar.SharpWebMail.UI.AddressBook.UpdateDataSource(data, addressbook, Session["client"] as anmar.SharpWebMail.IEmailClient );
				}
			} else
				error = true;
			if ( error )
				this.SetMessage("addressbookEntryUpdateError");
			else if ( update )
				Response.Redirect(System.String.Concat("addressbook_edit.aspx?addr=", this._email.Value, "&book=", this._book_name));
			data = null;
		}

		protected void Page_Init () {
			this.EnsureChildControls();
			this._email=(System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("email");
			this._name=(System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("name");
			this._addressbook_entry_title=(System.Web.UI.WebControls.Label)this.SharpUI.FindControl("addressbookEntryTitle");
			this._entry = Request.QueryString["addr"];
			this._book_name = Request.QueryString["book"];
			if ( this._entry!=null && this._book_name!=null && this._entry.Length>0 && this._book_name.Length>0 )
				this._update = true;
			else
				this._update = false;
			System.Web.UI.WebControls.RegularExpressionValidator emailvalidator=(System.Web.UI.WebControls.RegularExpressionValidator)this.SharpUI.FindControl("REEmailValidator");
			emailvalidator.ValidationExpression = "^" + anmar.SharpMimeTools.ABNF.addr_spec + "$";
			System.Web.UI.WebControls.Label addrname = (System.Web.UI.WebControls.Label)this.SharpUI.FindControl("addressbookLabelItem");
			if ( addrname!=null ) {
				addrname.Text = this._book_name;
			}
			this.SharpUI.nextPageImageButton.Enabled = false;
			this.SharpUI.prevPageImageButton.Enabled = false;
			this.SharpUI.refreshPageImageButton.Enabled = false;
		}
		protected void Page_Load () {
			this._email.Value = this._email.Value.Trim();
			this._name.Value = this._name.Value.Trim();
		}
		protected void Page_PreRender () {
			if ( !this.IsPostBack && this._update ) {
				System.Data.DataTable data = GetData();
				if ( data!=null && data.Rows.Count>0 ) {
				 	data.DefaultView.RowFilter = System.String.Concat(data.Columns[1].ColumnName, "='", this._entry, "'");
				 	if ( data.DefaultView.Count==1 ) {
				 		this._name.Value = data.DefaultView[0][0].ToString();
				 		this._email.Value = data.DefaultView[0][1].ToString();
				 		if ( this._addressbook_entry_title!=null ) {
				 			this._addressbook_entry_title.Text = this._name.Value;
				 		}
				 	}
				 }
			}
			if ( this._update ) {
				System.Web.UI.Control button = this.SharpUI.FindControl("addressbookEntryInsert");
				button.Visible = false;
			} else {
				System.Web.UI.Control button = this.SharpUI.FindControl("addressbookEntryUpdate");
				button.Visible = false;
			}
		}
	}
}
