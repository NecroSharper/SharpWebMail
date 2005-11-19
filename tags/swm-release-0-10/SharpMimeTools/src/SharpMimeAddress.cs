// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2005 Angel Marin
// 
//   This file is part of SharpMimeTools
//
//   SharpMimeTools is free software; you can redistribute it and/or
//   modify it under the terms of the GNU Lesser General Public
//   License as published by the Free Software Foundation; either
//   version 2.1 of the License, or (at your option) any later version.
//
//   SharpMimeTools is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//   Lesser General Public License for more details.
//
//   You should have received a copy of the GNU Lesser General Public
//   License along with SharpMimeTools; if not, write to the Free Software
//   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
// -----------------------------------------------------------------------

using System;

namespace anmar.SharpMimeTools
{
	internal class SharpMimeAddressCollection : System.Collections.IEnumerable {
		protected System.Collections.ArrayList list = new System.Collections.ArrayList();

		public SharpMimeAddressCollection ( System.String text ) {
			System.Text.RegularExpressions.Regex email = new System.Text.RegularExpressions.Regex(
			    @"(" + anmar.SharpMimeTools.ABNF.address + @")");
			System.String[] tokens = email.Split(text);
			foreach ( System.String token in tokens ) {
				if ( email.IsMatch(token ) )
					this.Add ( new anmar.SharpMimeTools.SharpMimeAddress( token ) );
			}
		}
		public anmar.SharpMimeTools.SharpMimeAddress this [ int index ] {
			get {
					return this.Get( index );
			}
		}
		public System.Collections.IEnumerator GetEnumerator() {
			return list.GetEnumerator();
		}
		public void Add ( anmar.SharpMimeTools.SharpMimeAddress address ) {
			list.Add ( address);
		}
		public anmar.SharpMimeTools.SharpMimeAddress Get ( int index ) {
			return (anmar.SharpMimeTools.SharpMimeAddress) list[index];
		}
		public static anmar.SharpMimeTools.SharpMimeAddressCollection Parse( System.String text ) {
			if ( text == null )
				throw new ArgumentNullException();
			return new anmar.SharpMimeTools.SharpMimeAddressCollection ( text );
		}
		public int Count {
			get {
				return list.Count;
			}
		}
		public override string ToString() {
			System.Text.StringBuilder text = new System.Text.StringBuilder();
			foreach ( anmar.SharpMimeTools.SharpMimeAddress token in list ) {
				text.Append ( token.ToString() );
				if ( token.Length>0 )
					text.Append ("; ");
			}
			return text.ToString(); 
		}
	}
	/// <summary>
	/// rfc 2822 email address
	/// </summary>
	public class SharpMimeAddress {
		private System.String name;
		private System.String address;
		/// <summary>
		/// Initializes a new address from a RFC 2822 name-addr specification string
		/// </summary>
		/// <param name="dir">RFC 2822 name-addr address</param>
		/// 
		public SharpMimeAddress ( System.String dir ) {
			name = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( dir, 1 );
			address = anmar.SharpMimeTools.SharpMimeTools.parseFrom ( dir, 2 );
		}
		/// <summary>
		/// Gets the decoded address or name contained in the name-addr
		/// </summary>
		public System.String this [System.Object key] {
			get {
				if ( key == null ) throw new System.ArgumentNullException();
				switch (key.ToString()) {
					case "0":
					case "name":
						return this.name;
					case "1":
					case "address":
						return this.address;
				}
				return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override System.String ToString() {
			if ( this.name.Equals (System.String.Empty ) && this.address.Equals (System.String.Empty ) )
				return "";
			if ( this.name.Equals (System.String.Empty ) )
				return String.Format( "<{0}>", this.address);
			else
				return String.Format( "\"{0}\" <{1}>" , this.name , this.address);
		}
		/// <summary>
		/// Gets the length of the decoded address
		/// </summary>
		public int Length {
			get {
				return this.name.Length + this.address.Length;
			}
		}
	}
}