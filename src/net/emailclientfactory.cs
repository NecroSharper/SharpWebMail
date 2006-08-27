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

namespace anmar.SharpWebMail.Net
{
	public sealed class EmailClientFactory {
		public static anmar.SharpWebMail.Net.IEmailClient CreateEmailClient ( anmar.SharpWebMail.Config.EmailServerInfo server, System.String username, System.String password ) {
			if ( server==null )
				return null;
			switch ( server.Protocol ) {
				case anmar.SharpWebMail.Config.EmailServerProtocol.Pop3:
					return new anmar.SharpWebMail.Net.SharpPop3Client ( server.Host, server.Port, username, password );
				case anmar.SharpWebMail.Config.EmailServerProtocol.Imap:
					return new anmar.SharpWebMail.Net.SharpImap4Client( server.Host, server.Port, username, password );
			}
			return null;
		}
	}
}
