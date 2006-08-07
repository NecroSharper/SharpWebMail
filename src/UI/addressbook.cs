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
	public class AddressBook : System.Web.UI.Page {
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected anmar.SharpWebMail.UI.globalUI SharpUI;
		protected System.Web.UI.WebControls.DataGrid AddressBookDataGrid;
		protected System.Web.UI.HtmlControls.HtmlSelect addressbookselect;
		protected System.Web.UI.WebControls.Label addressbooklabel;
		private System.Collections.Specialized.StringCollection _delete_items = null;
		private System.String _sort_expression = "[NameColumn] ASC";

		public static System.Collections.Specialized.ListDictionary GetAddressbook (System.String name, System.Object books) {
			if ( name==null || name.Length==0 || books==null || !(books is System.Collections.SortedList) )
				return null;
			System.Collections.SortedList addressbooks = (System.Collections.SortedList)books;
			if ( addressbooks.ContainsKey(name) ) {
				return (System.Collections.Specialized.ListDictionary)addressbooks[name];
			}
			return null;
		}
		private static System.Data.Common.DbDataAdapter GetDataAdapter (System.String type, System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter) {
			if ( type.Equals("odbc") )
				return new System.Data.Odbc.OdbcDataAdapter(searchfilter, connectstring);
			else if ( type.Equals("oledb") )
				return new System.Data.OleDb.OleDbDataAdapter(searchfilter, connectstring);
			else
				return null;
		}
		public static System.Data.DataTable GetDataSource (System.Collections.Specialized.ListDictionary addressbook, bool specific, anmar.SharpWebMail.IEmailClient client ) {
			if ( !addressbook.Contains("connectionstring") 
			    || !addressbook.Contains("searchstring") )
				return null;
			System.String connectstring = addressbook["connectionstring"].ToString();
			System.String connectusername = null, connectpassword = null;
			if ( addressbook.Contains("connectionusername") && addressbook.Contains("connectionpassword") ) {
				connectusername = addressbook["connectionusername"].ToString();
				connectpassword = addressbook["connectionpassword"].ToString();
			} else if ( client!=null ) {
				connectusername = client.UserName;
				connectpassword = client.Password;
			}

			System.String searchfilter;
			if ( specific )
				searchfilter = addressbook["searchstringrealname"].ToString();
			else
				searchfilter = addressbook["searchstring"].ToString();
			if ( client!=null )
				searchfilter=searchfilter.Replace("$USERNAME$", client.UserName);
			else
				searchfilter=searchfilter.Replace("$USERNAME$", System.String.Empty);
			System.String namecolumn = addressbook["namecolumn"].ToString();
			System.String mailcolumn = addressbook["emailcolumn"].ToString();
			System.String ownercolumn = "owner";
			if ( addressbook.Contains("ownercolumn") )
				ownercolumn = addressbook["ownercolumn"].ToString();
			if ( addressbook["type"].Equals("ldap") )
				return GetDataSourceLDAP(addressbook["name"].ToString(), connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn, ownercolumn);
			else if ( addressbook["type"].Equals("odbc") )
				return GetDataSourceODBC(addressbook["name"].ToString(), connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn, ownercolumn);
			else if ( addressbook["type"].Equals("oledb") )
				return GetDataSourceOLEDB(addressbook["name"].ToString(), connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn, ownercolumn);
			else
				return null;
		}
		private static System.Data.DataTable GetDataSourceData (System.Data.Common.DbDataAdapter adapter, System.String namecolumn, System.String mailcolumn, System.String ownercolumn, System.String book) {
			if ( adapter==null )
				return null;
			System.Data.DataTable data = GetDataSourceDataTable (namecolumn, mailcolumn, ownercolumn, book);
			try {
				adapter.Fill(data);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error while doing query", e);
				return null;
			}
			return data;
		}
		private static System.Data.DataTable GetDataSourceDataTable (System.String namecolumn, System.String mailcolumn, System.String ownercolumn, System.String book) {
			System.Data.DataTable table = new System.Data.DataTable("addressbook");
			table.Columns.Add(new System.Data.DataColumn(namecolumn, typeof(System.String)));
			table.Columns.Add(new System.Data.DataColumn(mailcolumn, typeof(System.String)));
			table.Columns.Add(new System.Data.DataColumn("addressbook", typeof(System.String)));
			table.Columns.Add(new System.Data.DataColumn(ownercolumn, typeof(System.String)));
			table.Columns[1].Unique = true;
			table.Columns[2].DefaultValue = book;
			return table;
		}
		private static System.Data.DataTable GetDataSourceLDAP (System.String book, System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter, System.String namecolumn, System.String mailcolumn, System.String ownercolumn) {
			System.Data.DataTable datasource = GetDataSourceDataTable(namecolumn, mailcolumn, ownercolumn, book);
			System.DirectoryServices.DirectoryEntry direntry = new System.DirectoryServices.DirectoryEntry(connectstring);
			direntry.Username = connectusername;
			direntry.Password = connectpassword;
			System.DirectoryServices.DirectorySearcher dirsearcher = new System.DirectoryServices.DirectorySearcher(direntry);
			dirsearcher.Filter = searchfilter;
			dirsearcher.SearchScope = System.DirectoryServices.SearchScope.OneLevel;
			dirsearcher.PropertiesToLoad.Add(namecolumn);
			dirsearcher.PropertiesToLoad.Add(mailcolumn);
			System.DirectoryServices.SearchResultCollection results = null;
			try {
				results = dirsearcher.FindAll();
			} catch ( System.Exception e) {
				if (log.IsErrorEnabled)
					log.Error("Error while doing LDAP query", e);
				return null;
			}
			System.String name, value;
			foreach ( System.DirectoryServices.SearchResult result in results ) {
				name = null;
				value = null;
				if ( result.Properties.Contains(namecolumn) && result.Properties.Contains(mailcolumn) && result.Properties[namecolumn].Count>0 && result.Properties[mailcolumn].Count>0 ) {
					name = result.Properties[namecolumn][0].ToString();
					value = result.Properties[mailcolumn][0].ToString();
				}
				if ( name!=null && value!=null ) {
					try {
						datasource.Rows.Add(new object[]{name, value});
					} catch ( System.Exception ){}
				}
			}
			return datasource;
		}
		private static System.Data.DataTable GetDataSourceODBC (System.String book, System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter, System.String namecolumn, System.String mailcolumn, System.String ownercolumn) {
			return GetDataSourceData(GetDataAdapter("odbc", connectstring, connectusername, connectpassword, searchfilter), namecolumn, mailcolumn, ownercolumn, book);
		}
		private static System.Data.DataTable GetDataSourceOLEDB (System.String book, System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter, System.String namecolumn, System.String mailcolumn, System.String ownercolumn) {
			return GetDataSourceData(GetDataAdapter("oledb", connectstring, connectusername, connectpassword, searchfilter), namecolumn, mailcolumn, ownercolumn, book);
		}
		public static bool UpdateDataSource (System.Data.DataTable data, System.Collections.Specialized.ListDictionary addressbook, anmar.SharpWebMail.IEmailClient client) {
			bool error = false;
			if ( data==null || addressbook==null || !addressbook.Contains("connectionstring") 
			    || !addressbook.Contains("searchstring") || !addressbook.Contains("allowupdate") || !((bool)addressbook["allowupdate"]) )
				return false;
			System.String connectstring = addressbook["connectionstring"].ToString();
			System.String connectusername = null, connectpassword = null;
			if ( addressbook.Contains("connectionusername") && addressbook.Contains("connectionpassword") ) {
				connectusername = addressbook["connectionusername"].ToString();
				connectpassword = addressbook["connectionpassword"].ToString();
			} else if ( client!=null ) {
				connectusername = client.UserName;
				connectpassword = client.Password;
			}

			System.String searchfilter = addressbook["searchstring"].ToString();
			if ( client!=null )
				searchfilter=searchfilter.Replace("$USERNAME$", client.UserName);
			else
				searchfilter=searchfilter.Replace("$USERNAME$", System.String.Empty);
			System.Data.Common.DbDataAdapter adapter = GetDataAdapter(addressbook["type"].ToString(), connectstring, connectusername, connectpassword, searchfilter);
			if ( adapter!=null ) {
				try {
					if ( addressbook["type"].Equals("odbc") ) {
						System.Data.Odbc.OdbcCommandBuilder builder = new System.Data.Odbc.OdbcCommandBuilder(adapter as System.Data.Odbc.OdbcDataAdapter);
						adapter.Update(data);
						builder = null;
					} else if ( addressbook["type"].Equals("oledb") ) {
						System.Data.OleDb.OleDbCommandBuilder builder = new System.Data.OleDb.OleDbCommandBuilder(adapter as System.Data.OleDb.OleDbDataAdapter);
						adapter.Update(data);
						builder = null;
					}
				} catch ( System.Exception e ) {
					if ( log.IsErrorEnabled )
						log.Error(System.String.Concat("Error updating address book [", addressbook["name"], "] for user [", client.UserName, "]" ), e);
					error = true;
				}
			} else {
				error = true;
			}
			adapter = null;
			return !error;
		}

		protected void AddressBookDataGrid_DeleteCheckBox ( System.Object sender, System.EventArgs args ) {
			if ( this._delete_items==null )
				this._delete_items = new System.Collections.Specialized.StringCollection();
			if ( ((System.Web.UI.HtmlControls.HtmlInputCheckBox)sender).Checked )
				this._delete_items.Add (((System.Web.UI.HtmlControls.HtmlInputCheckBox)sender).Value);
		}
		protected void AddressBookDataGrid_Delete ( System.Object sender, System.Web.UI.WebControls.DataGridCommandEventArgs args ) {
			if ( this._delete_items!=null && this._delete_items.Count>0 ) {
				System.Collections.Specialized.ListDictionary addressbook = anmar.SharpWebMail.UI.AddressBook.GetAddressbook(this.addressbookselect.Value, Application["sharpwebmail/send/addressbook"]);
				System.Data.DataTable data = GetDataSource(addressbook, false, Session["client"] as anmar.SharpWebMail.IEmailClient);
				if ( data!=null ) {
					bool delete = false;
					System.Data.DataView view = data.DefaultView;
					foreach ( System.String item in this._delete_items ) {
						view.RowFilter = System.String.Concat(data.Columns[1].ColumnName, "='", item, "'");
						if ( view.Count==1 ) {
							view[0].Delete();
							delete = true;
						}
					}
					if ( delete ) {
						anmar.SharpWebMail.UI.AddressBook.UpdateDataSource(data, addressbook, Session["client"] as anmar.SharpWebMail.IEmailClient);
					}
				}
			}
		}
		protected void AddressBook_Changed ( System.Object sender, System.EventArgs args ) {
			this.AddressBookDataGrid.CurrentPageIndex = 0;
		}
		protected void AddressBookDataGrid_PageIndexChanged ( System.Object sender, System.Web.UI.WebControls.DataGridPageChangedEventArgs args ) {
			if ( args.NewPageIndex>=0 && args.NewPageIndex<this.AddressBookDataGrid.PageCount )
				this.AddressBookDataGrid.CurrentPageIndex = args.NewPageIndex;
		}
		protected void AddressBookNextPageButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			if ( (this.AddressBookDataGrid.CurrentPageIndex+1)<this.AddressBookDataGrid.PageCount ) {
				AddressBookDataGrid_PageIndexChanged ( sender, new System.Web.UI.WebControls.DataGridPageChangedEventArgs ( sender, this.AddressBookDataGrid.CurrentPageIndex+1 ) );
			}
		}
		protected void AddressBookPrevPageButton_Click ( System.Object sender, System.Web.UI.ImageClickEventArgs args ) {
			if ( this.AddressBookDataGrid.CurrentPageIndex>0 ) {
				AddressBookDataGrid_PageIndexChanged (sender, new System.Web.UI.WebControls.DataGridPageChangedEventArgs ( sender, this.AddressBookDataGrid.CurrentPageIndex-1 ));
			}
		}
		protected void AddressBookDataGrid_Sort ( System.Object sender, System.Web.UI.WebControls.DataGridSortCommandEventArgs args ) {
			this.AddressBookDataGrid.CurrentPageIndex = 0;
			this._sort_expression = args.SortExpression;
		}
		protected void Page_Init () {
			this.EnsureChildControls();
			// Full page things
			if ( this.AddressBookDataGrid==null && this.SharpUI!=null ) {
				this.addressbookselect=(System.Web.UI.HtmlControls.HtmlSelect)this.SharpUI.FindControl("addressbookselect");
				this.AddressBookDataGrid=(System.Web.UI.WebControls.DataGrid)this.SharpUI.FindControl("AddressBookDataGrid");
				this.addressbooklabel=(System.Web.UI.WebControls.Label)this.SharpUI.FindControl("addressbookLabel");
				this.SharpUI.nextPageImageButton.Click += new System.Web.UI.ImageClickEventHandler(AddressBookNextPageButton_Click);
				this.SharpUI.prevPageImageButton.Click += new System.Web.UI.ImageClickEventHandler(AddressBookPrevPageButton_Click);
				this.SharpUI.refreshPageImageButton.Enabled = false;
				this.AddressBookDataGrid.PagerStyle.NextPageText = this.SharpUI.LocalizedRS.GetString("inboxNextPage");
				this.AddressBookDataGrid.PagerStyle.PrevPageText = this.SharpUI.LocalizedRS.GetString("inboxPrevPage");
			}
			
			if ( Application["sharpwebmail/send/addressbook"]!=null ) {
				System.Collections.SortedList addressbooks = (System.Collections.SortedList)Application["sharpwebmail/send/addressbook"];
				if ( addressbooks.Count>1 ) {
					addressbookselect.Visible = true;
					if ( this.addressbooklabel!=null )
						this.addressbooklabel.Visible = true;
				}
				addressbookselect.DataSource = addressbooks;
				addressbookselect.DataTextField = "Key";
				addressbookselect.DataValueField = "Key";
				addressbookselect.DataBind();
				if ( !this.IsPostBack ) {
					System.String book = Request.QueryString["book"];
					if ( book!=null && book.Length>0 ) {
						addressbookselect.Value = book;
					}
				}
			}
		}
		protected void Page_PreRender( System.Object sender, System.EventArgs args ) {
			System.Collections.Specialized.ListDictionary addressbook = GetAddressbook(addressbookselect.Value, Application["sharpwebmail/send/addressbook"]);
			if ( addressbook!=null && !addressbook["type"].Equals("none") ) {
				this.AddressBookDataGrid.PageSize = (int)addressbook["pagesize"];
				System.Data.DataTable data = GetDataSource(addressbook, false, Session["client"] as anmar.SharpWebMail.IEmailClient );
				if ( data!=null ) {
					if ( this._sort_expression!=null && this._sort_expression.Length>0 ) {
						if ( this._sort_expression.IndexOf("[NameColumn]")!=-1 ) {
							this._sort_expression = this._sort_expression.Replace("[NameColumn]", addressbook["NameColumn"].ToString());
						} else if ( this._sort_expression.IndexOf("[EmailColumn]")!=-1 ) {
							this._sort_expression = this._sort_expression.Replace("[EmailColumn]", addressbook["EmailColumn"].ToString());
						} else {
							this._sort_expression = null;
						}
						if ( this._sort_expression!=null )
							data.DefaultView.Sort = this._sort_expression;
					}
					this.AddressBookDataGrid.DataSource = data.DefaultView;
				}
				// Full Page Mode
				if ( this.SharpUI!=null ) {
					bool allowupdate = (bool)addressbook["allowupdate"];
					// Editable columns?
					if ( this.AddressBookDataGrid.Columns.Count>1 ) {
						this.AddressBookDataGrid.Columns[this.AddressBookDataGrid.Columns.Count-1].Visible = allowupdate;
						this.AddressBookDataGrid.Columns[this.AddressBookDataGrid.Columns.Count-2].Visible = allowupdate;
					}
					// Editable links?
					System.Web.UI.HtmlControls.HtmlAnchor link = (System.Web.UI.HtmlControls.HtmlAnchor)this.SharpUI.FindControl("addressbookEntryInsert");
					if ( link!=null ) {
						link.HRef = System.String.Concat("addressbook_edit.aspx?book=", System.Web.HttpUtility.UrlEncode(this.addressbookselect.Value));
						link.Visible = allowupdate;
					}
					// Editable links?
					link = (System.Web.UI.HtmlControls.HtmlAnchor)this.SharpUI.FindControl("addressbookImportExport");
					if ( link!=null ) {
						link.HRef = System.String.Concat("addressbook_data.aspx?book=", System.Web.HttpUtility.UrlEncode(this.addressbookselect.Value));
						link.Visible = allowupdate;
					}
				}
			}
			System.Resources.ResourceSet resources = (System.Resources.ResourceSet) Session["resources"];;
			// Pop-up mode
			if ( this.SharpUI==null ) {
				this.AddressBookDataGrid.Columns[0].HeaderText = resources.GetString("addressbookNameLabel");
			}
			this.AddressBookDataGrid.Columns[0].HeaderStyle.Wrap = false;
			this.AddressBookDataGrid.DataBind();
		}
	}
}
