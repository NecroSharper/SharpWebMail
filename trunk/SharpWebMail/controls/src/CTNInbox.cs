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

namespace anmar.SharpWebMail
{
	/// <summary>
	/// 
	/// </summary>
	public class CTNInbox {
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		/// <summary>
		/// 
		/// </summary>
		protected System.Data.DataTable inbox;
		protected System.Data.DataView inbox_view;
		/// <summary>
		/// 
		/// </summary>
		protected System.Object[] names = {   "index", typeof(System.Int32),			//  0
											  "msgnum", typeof(System.Int32),			//  1
											  "Size", typeof(System.Int32),				//  2
											  "uidl", typeof(System.String),			//  3
											  "From", typeof(System.String),			//  4
											  "FromName", typeof(System.String),		//  5
											  "FromCollection", typeof(System.Collections.IEnumerable),		//  6
											  "To", typeof(System.String),				//  7
											  "ToCollection", typeof(System.Collections.IEnumerable),	//  8
											  "Reply", typeof(System.Collections.IEnumerable),			//  9
											  "Subject", typeof(System.String),			// 10
											  "StringDate", typeof(System.String),		// 11
											  "Message-ID", typeof(System.String),		// 12
											  "Headers", typeof(anmar.SharpMimeTools.SharpMimeHeader), 		// 13
											  "Date", typeof(System.DateTime),			// 14
											  "delete", typeof(System.Boolean),			// 15
											  "read", typeof(System.Boolean),			// 16
											  "guid", typeof(System.String)};			// 17
		/// <summary>
		/// 
		/// </summary>
		protected System.Int32 mcount = 0;
		/// <summary>
		/// 
		/// </summary>
		protected System.Int32 msize = 0;
		/// <summary>
		/// 
		/// </summary>
		protected System.String sort = "msgnum DESC";

		/// <summary>
		/// 
		/// </summary>
		public CTNInbox() {
			this.init();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="names"></param>
		public CTNInbox( System.String[] names ) {
			this.names = names;
		}
		public System.Object[] this [ System.String uid ] {
			get {
				try {
					return this[new System.Guid(uid)];
				} catch ( System.Exception e ) {
					if ( log.IsErrorEnabled )
						log.Error("Error parsing UID", e);
					return null;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public System.Object[] this [ System.Guid guid ] {
			get {
				this.inbox_view.RowFilter = System.String.Format ("guid='{0}'", guid);
				if ( this.inbox_view.Count==1 ) {
					if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("{0} guid='{0}' found", guid));
					return this.inbox_view[0].Row.ItemArray;
				} else {
					if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("guid='{0}' not found", guid));
					return null;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="uidllist"></param>
		/// <returns></returns>
		public bool buildMessageTable ( System.Int32[] list, System.String[] uidlist ) {
			bool error = false;

			// We need to initialize the inbox list
			if (this.Count == 0) {
				for ( int i=0 ; i<uidlist.Length; i++) {
					this.newMessage (i+1, list[i], uidlist[i]);
				}
			} else {
				System.Collections.Specialized.StringCollection uidl = new System.Collections.Specialized.StringCollection();
				uidl.AddRange(uidlist);
				// As we already have an index, we try to put it in sync
				// with the mail server
				for ( int i=0 ; i<uidlist.Length; i++ ) {
					this.inbox_view.RowFilter = System.String.Concat("uidl = '", uidlist[i].Replace("'", "''"), "'");
					// Message not found, so we add it
					if (this.inbox_view.Count == 0 ){
						this.newMessage (i+1, list[i], uidlist[i]);
					} else {
						// Message found, but at the wrong position
						if ( !this.inbox_view[0][1].Equals(i+1) ) {
							this.inbox_view[0][1] = i+1;
						}
					}
				}
				// now we try to find deleted messages
				this.inbox_view.RowFilter = System.String.Empty;

				for ( int i=0 ; i<this.inbox.Rows.Count; i++ ) {
					System.Data.DataRow item = this.inbox.Rows[i];
					if ( !uidl.Contains(item[3].ToString() ) ) {
						this.mcount--;
						this.msize -= (int)item[2];
						item.Delete();
						i--;
					}
				}
			}
			return !error;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msgs"></param>
		/// <param name="npage"></param>
		/// <param name="npagesize"></param>
		/// <returns></returns>
		public bool buildMessageList ( System.Collections.Hashtable msgs, int npage, int npagesize ) {
			bool error = false;
			int start = (int)npage * npagesize;
			start = (start<0)?0:start;
			int end = start + npagesize;
			System.String tmpvalue;
			System.Int32 tmpkey;

			System.String field = sort.Split(' ')[0];
			// If we are sorting by any colunm which data is not contained
			// in UIDL or LIST responses, then we need to cache the whole
			// inbox in order to do the sort opetarion
			if ( this.inbox.Columns[field].Ordinal>3 ) {
				start = 0;
				end = this.Count;
			}
			this.inbox_view.RowFilter = "delete=false";
			this.inbox_view.Sort = sort;

			for ( int i=start; (!error) && i<this.inbox_view.Count && i<end ; i++ ) {
				tmpkey = (int)this.inbox_view[i][1];
				tmpvalue = this.inbox_view[i][3].ToString();
				// Message added to list but hash and msgnum added do not match with
				// last server response
				if ( (msgs.ContainsKey (tmpkey) && !msgs[tmpkey].Equals(tmpvalue)) || (msgs.ContainsValue(tmpvalue) && !msgs[tmpkey].Equals(tmpvalue) ) ){
					msgs.Remove(tmpkey);
				}
				// We want to get headers only if we do not have them
				if ( this.inbox_view[i][13].Equals(System.DBNull.Value) && !msgs.ContainsKey (tmpkey) ) {
					msgs.Add( this.inbox_view[i][1], this.inbox_view[i][3].ToString() );
				}
			}
			this.inbox_view.RowFilter = System.String.Empty;
			this.inbox_view.Sort = System.String.Empty;
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		public void deleteMessage ( System.String uid ) {
			if ( this.markMessage( uid, 15, true ) )
				this.mcount--;
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		public void flush () {
			this.init();
		}
		private System.Guid getGuid ( System.String uid ) {
			try {
				return new System.Guid(uid);
			} catch ( System.Exception e ) {
				if ( log.IsErrorEnabled )
					log.Error("Error parsing UID", e);
				return System.Guid.Empty;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		public System.String getMessageIndex ( System.String uid ) {
			System.Guid guid = this.getGuid(uid);
			if ( !guid.Equals(System.Guid.Empty) )
				this.inbox_view.RowFilter = System.String.Concat("guid='", guid, "'");
			return ( this.inbox_view.Count==1 )?this.inbox_view[0][1].ToString():System.String.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void init () {
			inbox = new System.Data.DataTable("Inbox");
			for ( int i=0 ; i<names.Length/2 ; i++ ) {
				inbox.Columns.Add(new System.Data.DataColumn((System.String)names[i*2], (System.Type)names[i*2+1]));
			}
			inbox.Columns[0].AutoIncrement=true;
			inbox.Columns[0].Unique=true;
			this.inbox_view = new System.Data.DataView(this.inbox);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="col"></param>
		/// <param name="val"></param>
		protected bool markMessage ( System.String uid, int col, bool val ) {
			System.Guid guid = this.getGuid(uid);
			if ( guid.Equals(System.Guid.Empty) )
				return false;
			this.inbox_view.RowFilter = System.String.Concat("guid='", guid, "'");
			if ( this.inbox_view.Count==1 && this.inbox_view[0][col].Equals(!val) ) {
				this.inbox_view[0][col] = val;
				return true;
			} else {
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uidl"></param>
		/// <param name="header"></param>
		/// <returns></returns>
		public bool newMessage (System.String uidl, anmar.SharpMimeTools.SharpMimeHeader header ) {
			bool error = false;
			this.inbox_view.RowFilter = System.String.Concat("uidl='", uidl, "'");
			if (this.inbox_view.Count == 1 ) {
				System.Data.DataRowView msg = this.inbox_view[0];
				msg[4] = header.From;
				msg[5] = "";
				msg[6] = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( header.From );
				msg[7] = header.To;
				msg[8] = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( header.To );
				msg[9] = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( header.Reply );
				msg[10] = anmar.SharpMimeTools.SharpMimeTools.parserfc2047Header ( header.Subject );
				System.String date = header.Date;
				if ( date.Equals(System.String.Empty) && header.Contains("Received") ) {
					date = header["Received"];
					if ( date.IndexOf("\r\n")>0 )
						date = date.Substring(0, date.IndexOf("\r\n"));
					if ( date.LastIndexOf(';')>0 )
						date = date.Substring(date.LastIndexOf(';')+1).Trim();
					else
						date = System.String.Empty;
				}
				msg[11] = date;
				msg[12] = header.MessageID;
				msg[13] = header;
				msg[14] = anmar.SharpMimeTools.SharpMimeTools.parseDate ( date );
				msg[15] = false;
				msg[16] = false;
				if ( msg[6]!=null ) {
					foreach ( anmar.SharpMimeTools.SharpMimeAddress item in ((System.Collections.IEnumerable)msg[6]) ) {
						msg[5] = item["name"];
						if ( msg[5]==null || msg[5].Equals(System.String.Empty) )
							msg[5] = item["address"];
					}
				}
			} else {
				error = true;
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="size"></param>
		/// <param name="uidl"></param>
		public void newMessage (System.Int32 index, System.Int32 size, System.String uidl) {
			System.Data.DataRow tmpRow;
			tmpRow = inbox.NewRow();

			tmpRow[1] = index;
			tmpRow[2] = size;
			tmpRow[3] = uidl;
			tmpRow[12] = System.String.Empty;
			tmpRow[13] = System.DBNull.Value;
			tmpRow[15] = false;
			tmpRow[16] = false;
			tmpRow[17] = System.Guid.NewGuid().ToString();
			inbox.Rows.Add (tmpRow);
			this.mcount++;
			this.msize += size;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		public void readMessage ( System.String uid ) {
			this.markMessage( uid, 16, true );
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		public void undeleteMessage ( System.String uid ) {
			if ( this.markMessage( uid, 15, false ))
				this.mcount++;
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		public System.Data.DataTable getInbox {
			get {
				return this.inbox;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public System.Int32 Count {
			get {
				return this.inbox.Rows.Count;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public System.Int32 messageCount {
			get {
				return this.mcount;
			}
			set {
				this.mcount = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public System.Int32 messageSize {
			get {
				return this.msize;
			}
			set {
				this.msize = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public System.String sortExpression {
			get {
				return this.sort;
			}
			set {
				if ( value!=null && value.Length>0 )
					this.sort = value;
			}
		}
	}
}
