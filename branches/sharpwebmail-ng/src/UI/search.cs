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
	public class Search : System.Web.UI.Page {
		// General variables
		protected anmar.SharpWebMail.UI.globalUI SharpUI;

		/*
		 * General functions
		*/
		protected void mainInterface ( ) {
			// Disable some things
			this.SharpUI.nextPageImageButton.Enabled = false;
			this.SharpUI.prevPageImageButton.Enabled = false;
		}
		public System.String From {
			get {
				return ((System.Web.UI.HtmlControls.HtmlInputControl)this.SharpUI.FindControl("fromsearch")).Value;
			}
		}
		public System.String Subject {
			get {
				return ((System.Web.UI.HtmlControls.HtmlInputControl)this.SharpUI.FindControl("subjectsearch")).Value;
			}
		}
		/*
		 * Events
		*/
		/// <summary>
		/// 
		/// </summary>
		protected void Search_Click ( System.Object sender, System.EventArgs args ) {
			Server.Transfer ("default.aspx", false);
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

			this.mainInterface ();
		}
	}
}
