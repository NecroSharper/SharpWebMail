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
	public class download : System.Web.UI.Page {

		protected void Page_Load(System.Object Src, System.EventArgs E ) {
			// Prevent caching, so can't be viewed offline
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

			//Our Inbox
			anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];

			System.String msgid = Page.Request.QueryString["msgid"];
			System.String name = Page.Request.QueryString["name"];
			System.String inline = Page.Request.QueryString["i"];
			if ( msgid != null && name!=null && Session["temppath"]!=null ) {
				System.Object[] details = inbox[ msgid ];
				if ( details != null && details.Length>0 ) {
					System.String path = Session["temppath"].ToString();
					path = System.IO.Path.Combine (path, msgid);
					name = System.IO.Path.GetFileName(name);
					System.IO.FileInfo file = new System.IO.FileInfo (System.String.Format ( "{0}{1}{2}", path, System.IO.Path.DirectorySeparatorChar, name) );
					System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo ( path );
					if ( dir.Exists && file.Exists && dir.FullName.Equals (new System.IO.DirectoryInfo (file.Directory.FullName).FullName) ) {
						//TODO: return correct Content-Type
						if ( inline!=null && !inline.Equals("1") )
							Response.AppendHeader("Content-Disposition", System.String.Format ("attachment; filename=\"{0}\";", name));
						else
							Response.AppendHeader("Content-Disposition", System.String.Format ("inline; filename=\"{0}\";", name));
						Response.WriteFile ( file.FullName );
					}
					file = null;
					dir = null;
				}
				details = null;
			}
			inbox = null;
		}
	}
}
