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

		public static System.Collections.SortedList GetDataSource (System.Collections.Specialized.ListDictionary addressbook, bool specific, System.String username ) {
			System.String connectstring = addressbook["connectionstring"].ToString();
			System.String connectusername = addressbook["connectionusername"].ToString();
			System.String connectpassword = addressbook["connectionpassword"].ToString();
			System.String searchfilter;
			if ( specific )
				searchfilter = addressbook["searchstringrealname"].ToString();
			else
				searchfilter = addressbook["searchstring"].ToString();
			searchfilter=searchfilter.Replace("$USERNAME$", username);
			System.String namecolumn = addressbook["namecolumn"].ToString();
			System.String mailcolumn = addressbook["emailcolumn"].ToString();
			if ( addressbook["type"].Equals("ldap") )
				return GetDataSourceLDAP(connectstring, connectusername, connectpassword, searchfilter, namecolumn, mailcolumn);
			else
				return null;

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
				if ( name!=null && value!=null )
					datasource.Add(value, name);
			}
			return datasource;
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
				this.AddressBookDataGrid.DataSource = GetDataSource(addressbook, false, User.Identity.Name);
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
