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
											  "read", typeof(System.Boolean)};			// 16
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
		/// <summary>
		/// 
		/// </summary>
		public System.Object[] this [ System.Object uidl ] {
			get {
				uidl = uidl.ToString().Replace("'", "''");
				System.Data.DataRow[] result = this.inbox.Select(System.String.Format ("uidl='{0}'", uidl));
				if ( result.Length==1 ) {
					if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("{1} uidl='{0}' found", uidl));
					return result[0].ItemArray;
				} else {
					if ( log.IsDebugEnabled ) log.Debug ( System.String.Format ("uidl='{0}' not found", uidl));
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
		public bool buildMessageTable ( System.Int32[] list, System.String[] uidllist ) {
			bool error = false;

			// We need to initialize the inbox list
			if (this.Count == 0) {
				this.initMessageList (list, uidllist );
			} else {
				// As we already have an index, we try to put it in sync
				// with the mail server
				System.Data.DataRow[] result;
				for ( int i=0 ; i<uidllist.Length; i++ ) {
					result = this.inbox.Select("uidl = '" + uidllist[i].Replace("'", "''") + "'");
					// Message not found, so we add it
					if (result.Length == 0 ){
						this.newMessage (i+1, list[i], uidllist[i]);
					} else {
						// Message found, but at the incorrect position
						if (((int)result[0][1]) != (i+1)) {
							result[0][1] = i+1;
						}
					}
				}
				// now we try to find deleted messages
				result = this.inbox.Select();

				for ( int i=0, max = result.GetLength(0), j=0 ; i<max; i++ ) {
					for (j = 0; j<uidllist.Length; j++ ) {
						if (uidllist[j].Equals(result[i][3])) {
							break;
						}
					}
					if (j==uidllist.Length) {
						this.mcount--;
						this.msize -= (int) result[j][2];
						result[i].Delete();
						result = this.inbox.Select();
						max = result.GetLength(0);
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
			System.Data.DataRow[] result;
			System.String tmpvalue;
			System.Int32 tmpkey;

			System.String field = sort.Split(' ')[0];
			// If we are sorting by any colunm which data is not contained
			// in UIDL or LIST responses, then we need to cache the whole
			// inbox in orther to do the sort opetarion
			if ( this.inbox.Columns[field].Ordinal>3 ) {
				start = 0;
				end = this.Count;
			}
			result = this.inbox.Select("delete=false", sort);

			for ( int i=start; (!error) && i<result.GetLength(0) && i<end ; i++ ) {
				tmpkey = (int)result[i][1];
				tmpvalue = result[i][3].ToString();
				// Message added to list but hash and msgnum added do not match with
				// last server response
				if ( ( msgs.ContainsKey (tmpkey) && msgs[tmpkey].ToString() != tmpvalue ) ||( msgs.ContainsValue(tmpvalue) && msgs[tmpkey].ToString() != tmpvalue ) ){
					msgs.Remove(tmpkey);
				}
				// We want to get headers only if we do not have them
				if ( result[i][13].Equals(System.DBNull.Value) && !msgs.ContainsKey (tmpkey) ) {
					msgs.Add( result[i][1], result[i][3].ToString() );
				}
			}
			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uidl"></param>
		public void deleteMessage ( System.String uidl ) {
			if ( this.markMessage( uidl, 15, true ) )
				this.mcount--;
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		public void flush () {
			this.init();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uidl"></param>
		/// <returns></returns>
		public System.String getMessageIndex ( System.String uidl ) {
			System.Data.DataRow[] result;
			uidl = uidl.Replace("'", "''");
			result = this.inbox.Select("uidl='" + uidl + "'");
			return ( result.Length==1 )?result[0][1].ToString():System.String.Empty;
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
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="uidllist"></param>
		/// <returns></returns>
		protected bool initMessageList(System.Int32[] list, System.String[] uidllist) {
			bool error = false;
			for ( int i=0 ; i<uidllist.Length; i++) {
				this.newMessage (i+1, list[i], uidllist[i]);
			}

			return !error;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uidl"></param>
		/// <param name="col"></param>
		/// <param name="val"></param>
		protected bool markMessage ( System.String uidl, int col, bool val ) {
			System.Data.DataRow[] result;
			if ( uidl==null )
				return false;
			uidl = uidl.Replace("'", "''");
			result = this.inbox.Select("uidl='" + uidl + "'");
			if ( result.Length==1 && ((bool)result[0][col])==!val ) {
				result[0][col] = val;
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
			System.Data.DataRow[] result;
			uidl = uidl.Replace("'", "''");
			result = this.inbox.Select("uidl = '" + uidl + "'");
			if (result.Length == 1 ){
				result[0][4] = header.From;
				result[0][5] = "";
				result[0][6] = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( header.From );
				result[0][7] = header.To;
				result[0][8] = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( header.To );
				result[0][9] = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( header.Reply );
				result[0][10] = anmar.SharpMimeTools.SharpMimeTools.parserfc2047Header ( header.Subject );
				result[0][11] = header.Date;
				result[0][12] = header.MessageID;
				result[0][13] = header;
				result[0][14] = anmar.SharpMimeTools.SharpMimeTools.parseDate ( header.Date );
				result[0][15] = false;
				result[0][16] = false;
				if ( result[0][6]!=null ) {
					System.Collections.IEnumerator fromenum = ((System.Collections.IEnumerable)result[0][6]).GetEnumerator();
					if ( fromenum.MoveNext() ) {
						result[0][5] = ((anmar.SharpMimeTools.SharpMimeAddress)fromenum.Current)["name"];
						if ( result[0][5]==null || result[0][5].ToString().Equals(System.String.Empty) )
							result[0][5] = ((anmar.SharpMimeTools.SharpMimeAddress)fromenum.Current)["address"];
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
			inbox.Rows.Add (tmpRow);
			this.mcount++;
			this.msize += size;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uidl"></param>
		public void readMessage ( System.String uidl ) {
			this.markMessage( uidl, 16, true );
			return;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uidl"></param>
		public void undeleteMessage ( System.String uidl ) {
			if ( this.markMessage( uidl, 15, false ))
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
