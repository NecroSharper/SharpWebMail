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
	public class AddressBookData : System.Web.UI.Page {
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected anmar.SharpWebMail.UI.globalUI SharpUI;
		private System.String _book_name;
		// Input boxes
		private System.Web.UI.HtmlControls.HtmlInputText _delimiter;
		private System.Web.UI.HtmlControls.HtmlTextArea _data;
		
		private System.Data.DataTable GetData () {
			if ( this._book_name!=null && this._book_name.Length>0 ) {
				System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this._book_name, Application["sharpwebmail/send/addressbook"]);
				if ( addressbook!=null )
					return anmar.SharpWebMail.UI.AddressBook.GetDataSource(addressbook, false, Session["client"] as anmar.SharpWebMail.IEmailClient );
			}
			return null;
		}
		private void SetMessage (System.String msg, int count ) {
			System.String message = this.SharpUI.LocalizedRS.GetString(msg);
			if ( message!=null && message.Length>0 ) {
				if ( message.IndexOf("##")>0 ) {
					message = message.Replace("##", count.ToString());
				}
				System.Web.UI.Control holder = this.SharpUI.FindControl("ConfirmationPH");
				holder.Visible = true;
				System.Web.UI.WebControls.Label label = (System.Web.UI.WebControls.Label)holder.FindControl("ConfirmationMessage");
				label.Text = message;
			}
		}

		protected void AddressExport_Click ( System.Object sender, System.EventArgs args ) {
			if ( !this.IsValid )
				return;
			System.Data.DataTable data = GetData();
			if ( data!=null ) {
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				foreach ( System.Data.DataRowView item in data.DefaultView ) {
					sb.Append(item[0]);
					sb.Append(this._delimiter.Value);
					sb.Append(item[1]);
					sb.Append("\r\n");
				}
				this._data.Value = sb.ToString();
			}
		}
		protected void AddressImport_Click ( System.Object sender, System.EventArgs args ) {
			if ( !this.IsValid )
				return;
			bool update_duplicates = false;
			System.Web.UI.HtmlControls.HtmlInputCheckBox duplicates = (System.Web.UI.HtmlControls.HtmlInputCheckBox)this.SharpUI.FindControl("duplicates");
			if ( duplicates!=null && duplicates.Checked )
				update_duplicates = true;
			bool error = false;
			int count = 0;
			int linenumber = 0;
			System.Data.DataTable data = GetData();
			System.Data.DataView view = data.DefaultView;
			if ( update_duplicates )
				view.AllowEdit = true;
			if ( data!=null && this._data.Value.Length>0 ) {
				System.IO.StringReader reader = new System.IO.StringReader(this._data.Value);
				System.String line = null;
				while ( !error && (line=reader.ReadLine())!=null ) {
					linenumber++;
					line=line.Trim();
					if ( line.Length==0 )
						continue;
					int index = line.IndexOf(this._delimiter.Value);
					if ( index==-1 || index==(line.Length-1) ) {
						error = true;
						break;
					}
					System.String name = line.Substring(0, index);
					System.String addr = line.Substring(index+1);
					if ( name.Length>0 && addr.Length>0 ) {
						name = System.Web.HttpUtility.HtmlDecode(name);
						addr = System.Web.HttpUtility.HtmlDecode(addr);
						if ( !anmar.SharpMimeTools.ABNF.address_regex.IsMatch(addr) ) {
							error = true;
							break;
						}
						view.RowFilter = System.String.Concat(data.Columns[1].ColumnName, "='", addr, "'");
						if ( view.Count==1 ) {
							if ( update_duplicates ) {
								view[0][0] = name;
								count++;
							} else {
								if ( log.IsErrorEnabled )
									log.Error(System.String.Concat("Error importing record [", addr, "] in addressbook [", this._book_name, "] for user [", this.User.Identity.Name, "] (duplicated item)"));
								error = true;
							}
						} else {
							try {
								data.Rows.Add(new object[]{name, addr, this._book_name, this.User.Identity.Name});
								count++;
							} catch ( System.Exception e ) {
								if ( log.IsErrorEnabled )
									log.Error(System.String.Concat("Error importing record [", addr, "] in addressbook [", this._book_name, "] for user [", this.User.Identity.Name, "]"), e);
								error = true;
							}
						}
					}
				}
				if ( !error ) {
					System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this._book_name, Application["sharpwebmail/send/addressbook"]);
					error = !anmar.SharpWebMail.UI.AddressBook.UpdateDataSource(data, addressbook, Session["client"] as anmar.SharpWebMail.IEmailClient );
				}
				reader.Close();
				reader = null;
			} else {
				error = true;
			}
			if ( error )
				this.SetMessage("addressbookImportError", linenumber);
			else
				this.SetMessage("addressbookImportSuccess", count);
			data = null;
		}
		protected void Page_Init () {
			this.EnsureChildControls();
			this._book_name = Request.QueryString["book"];
			this._delimiter=(System.Web.UI.HtmlControls.HtmlInputText)this.SharpUI.FindControl("delimiter");
			this._data=(System.Web.UI.HtmlControls.HtmlTextArea)this.SharpUI.FindControl("data");
			System.Web.UI.WebControls.Label addrname = (System.Web.UI.WebControls.Label)this.SharpUI.FindControl("addressbookLabelItem");
			if ( addrname!=null ) {
				addrname.Text = this._book_name;
			}
			this.SharpUI.nextPageImageButton.Enabled = false;
			this.SharpUI.prevPageImageButton.Enabled = false;
			this.SharpUI.refreshPageImageButton.Enabled = false;
		}
	}
}
