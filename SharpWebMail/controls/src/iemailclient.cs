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
	public interface IEmailClient {
		bool getInboxIndex ( anmar.SharpWebMail.CTNInbox inbox, int npage, int npagesize, bool askserver );
		bool purgeInbox ( anmar.SharpWebMail.CTNInbox inbox, bool all );
		bool getMessage ( System.IO.MemoryStream Message, int mindex, System.String uidl );
		System.String UserName { get;}
		System.String Password { get;}
	}
}
