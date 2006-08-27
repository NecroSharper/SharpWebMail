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

namespace anmar.SharpWebMail.Config
{
	/// <summary>
	/// 
	/// </summary>	
	public enum EmailServerProtocol {
		/// <summary>
		/// IMAP. Read RFC 3501
		/// </summary>
		Imap,
		/// <summary>
		/// POP3. Read RFC 3461
		/// </summary>
		Pop3,
		/// <summary>
		/// SMTP. Read RFC 2821
		/// </summary>
		Smtp,
		/// <summary>
		/// SMTP combined with SMTP AUTH
		/// </summary>
		SmtpAuth,
		/// <summary>
		/// Unknown protocol
		/// </summary>
		Unknown
	}
}
