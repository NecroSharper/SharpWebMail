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
	public class AddressBook : System.Web.UI.Page {
		protected static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected System.Web.UI.WebControls.DataGrid AddressBookDataGrid;
		protected System.Web.UI.HtmlControls.HtmlSelect addressbookselect;

		public static System.Collections.SortedList GetDataSource (System.Collections.Specialized.ListDictionary addressbook, bool specific, anmar.SharpWebMail.IEmailClient client ) {
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
			if ( addressbook["type"].Equals("ldap") )
				return GetDataSourceLDAP(connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn);
			else if ( addressbook["type"].Equals("odbc") )
				return GetDataSourceODBC(connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn);
			else if ( addressbook["type"].Equals("oledb") )
				return GetDataSourceOLEDB(connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn);
			else
				return null;
		}
		private static System.Collections.SortedList GetDataSourceData (System.Data.Common.DbDataAdapter adapter, System.String namecolumn, System.String mailcolumn) {
			System.Collections.SortedList datasource = new System.Collections.SortedList();
			System.Data.DataSet data = new System.Data.DataSet();
			try {
				adapter.Fill(data);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error while doing query", e);
				return null;
			}
			if ( data.Tables.Count>0 ) {
				foreach ( System.Data.DataTable table in data.Tables ) {
					if ( !table.Columns.Contains(namecolumn) || !table.Columns.Contains(mailcolumn) )
						continue;
					foreach ( System.Data.DataRow item in table.Rows )
						if ( !datasource.ContainsKey(item[mailcolumn]) )
							datasource.Add(item[mailcolumn], item[namecolumn] );
				}
			}
			return datasource;
		}
		private static System.Collections.SortedList GetDataSourceLDAP (System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter, System.String namecolumn, System.String mailcolumn) {
			System.Collections.SortedList datasource = new System.Collections.SortedList();
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
				if ( name!=null && value!=null && !datasource.ContainsKey(value) )
					datasource.Add(value, name);
			}
			return datasource;
		}
		private static System.Collections.SortedList GetDataSourceODBC (System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter, System.String namecolumn, System.String mailcolumn) {
			System.Data.Odbc.OdbcDataAdapter adapter = new System.Data.Odbc.OdbcDataAdapter(searchfilter, connectstring);
			return GetDataSourceData(adapter, namecolumn, mailcolumn);
		}
		private static System.Collections.SortedList GetDataSourceOLEDB (System.String connectstring, System.String connectusername, System.String connectpassword, System.String searchfilter, System.String namecolumn, System.String mailcolumn) {
			System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(searchfilter, connectstring);
			return GetDataSourceData(adapter, namecolumn, mailcolumn);
		}
		protected void AddressBook_Changed ( System.Object sender, System.EventArgs args ) {
			this.AddressBookDataGrid.CurrentPageIndex = 0;
		}
		protected void AddressBookDataGrid_PageIndexChanged ( System.Object sender, System.Web.UI.WebControls.DataGridPageChangedEventArgs args ) {
			this.AddressBookDataGrid.CurrentPageIndex = args.NewPageIndex;
		}
		protected void Page_Init () {
			if ( Application["sharpwebmail/send/addressbook"]!=null ) {
				System.Collections.SortedList addressbooks = (System.Collections.SortedList)Application["sharpwebmail/send/addressbook"];
				if ( addressbooks.Count>1 )
					addressbookselect.Visible = true;
				addressbookselect.DataSource = addressbooks;
				addressbookselect.DataTextField = "Key";
				addressbookselect.DataValueField = "Key";
				addressbookselect.DataBind();
			}
		}
		protected void Page_Load( System.Object sender, System.EventArgs args ) {
			System.Collections.SortedList addressbooks = (System.Collections.SortedList)Application["sharpwebmail/send/addressbook"];
			System.Collections.Specialized.ListDictionary addressbook = (System.Collections.Specialized.ListDictionary)addressbooks[addressbookselect.Value];
			if ( !addressbook["type"].Equals("none") ) {
				this.AddressBookDataGrid.PageSize = (int)addressbook["pagesize"];
				this.AddressBookDataGrid.DataSource = GetDataSource(addressbook, false, Session["client"] as anmar.SharpWebMail.IEmailClient );
			}
		}
		protected void Page_PreRender( System.Object sender, System.EventArgs args ) {
			System.Resources.ResourceSet resources = (System.Resources.ResourceSet) Session["resources"];;
			this.AddressBookDataGrid.Columns[0].HeaderText = resources.GetString("newMessageWindowToEmailLabel");
			this.AddressBookDataGrid.Columns[0].HeaderStyle.Wrap = false;
			this.AddressBookDataGrid.DataBind();
		}
	}
}
