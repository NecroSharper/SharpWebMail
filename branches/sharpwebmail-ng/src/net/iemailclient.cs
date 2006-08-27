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
	public interface IEmailClient {
		bool GetFolderIndex ( anmar.SharpWebMail.SharpInbox inbox, int npage, int npagesize, bool askserver );
		bool PurgeInbox ( anmar.SharpWebMail.SharpInbox inbox, bool all );
		bool GetMessage ( System.IO.MemoryStream Message, int mindex, System.String uidl );
		System.String UserName { get;}
		System.String Password { get;}
	}
}
