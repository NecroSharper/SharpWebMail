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
			//Our Inbox
			anmar.SharpWebMail.CTNInbox inbox = (anmar.SharpWebMail.CTNInbox)Session["inbox"];

			System.String msgid = System.Web.HttpUtility.HtmlEncode (Page.Request.QueryString["msgid"]);
			System.String name = Page.Request.QueryString["name"];
			System.String inline = Page.Request.QueryString["i"];
			if ( msgid != null && name!=null && Session["temppath"]!=null ) {
				System.Object[] details = inbox[ msgid ];
				if ( details != null && details.Length>0 ) {
					System.String path = Session["temppath"].ToString();
					try {
						path = System.IO.Path.Combine (path, msgid);
					} catch ( System.ArgumentException ) {
						// Remove invalid chars
						foreach ( char ichar in System.IO.Path.InvalidPathChars ) {
							msgid = msgid.Replace ( ichar.ToString(), System.String.Empty );
						}
						path = System.IO.Path.Combine (path, msgid);
					}
					try {
						name = System.IO.Path.GetFileName(name);
					} catch ( System.ArgumentException ) {
						// Remove invalid chars
						foreach ( char ichar in System.IO.Path.InvalidPathChars ) {
							name = name.Replace ( ichar.ToString(), System.String.Empty );
						}
						name = System.IO.Path.GetFileName(name);
					}
					System.IO.FileInfo file = new System.IO.FileInfo ( System.IO.Path.Combine (path, name) );
					System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo ( path );
					if ( dir.Exists && file.Exists && dir.FullName.Equals (new System.IO.DirectoryInfo (file.Directory.FullName).FullName) ) {
						Page.Response.Clear();
						//FIXME: return correct Content-Type
						Response.AppendHeader("Content-Type", "application/octet-stream");
						System.String header;
						if ( inline!=null && inline.Equals("1") )
							header = "inline";
						else
							header = "attachment";
						header = System.String.Format ("{0}; filename=\"{1}\"; size=\"{2}\";", header, name, file.Length);
						Response.AppendHeader("Content-Disposition", header);
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
