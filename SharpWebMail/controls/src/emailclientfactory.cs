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
	public sealed class EmailClientFactory {
		public static anmar.SharpWebMail.IEmailClient CreateEmailClient ( anmar.SharpWebMail.EmailServerInfo server, System.String username, System.String password ) {
			if ( server==null )
				return null;
			switch ( server.Protocol ) {
				case anmar.SharpWebMail.ServerProtocol.Pop3:
					return new anmar.SharpWebMail.CTNSimplePOP3Client ( server.Host, server.Port, username, password );
			}
			return null;
		}
	}
}
