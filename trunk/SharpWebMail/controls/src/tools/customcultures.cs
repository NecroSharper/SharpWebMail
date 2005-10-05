// -----------------------------------------------------------------------
//
//   Copyright (C) 2003-2005 Angel Marin
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

namespace anmar.SharpWebMail.Tools
{
	class CustomCultures
	{
		public static void Main ( System.String[] args ) {
			if ( args.Length!=1 || args[0]==null || args[0].Length==0 ) {
				Console.WriteLine("Usage: CustomCultures.exe [culture-name]");
				return;
			}
			InstallCulture(args[0]);
		}
		public static void InstallCulture ( System.String name ) {
			System.Globalization.CultureAndRegionInfoBuilder customculture = new System.Globalization.CultureAndRegionInfoBuilder(name, System.Globalization.CultureAndRegionModifiers.Neutral);
			customculture.LoadDataFromCultureInfo(System.Globalization.CultureInfo.InvariantCulture);
			customculture.Parent = System.Globalization.CultureInfo.InvariantCulture;

			if ( name.Equals("my") ) {
				customculture.CultureNativeName = "Burmese";
				customculture.CultureEnglishName = "Burmese";
				customculture.ThreeLetterISOLanguageName = "bur";
				customculture.ThreeLetterWindowsLanguageName = "BUR";
				customculture.TwoLetterISOLanguageName = "my";
				customculture.IetfLanguageTag = "my";
				customculture.IsRightToLeft = true;
			} else {
				customculture = null;
			}
			if ( customculture!=null ) {
				System.Globalization.CultureInfo culture = null;
				try {
					culture = new System.Globalization.CultureInfo(name);
				} catch ( System.Exception ) {
				}
				try {
					if ( culture!=null ) {
						System.Globalization.CultureAndRegionInfoBuilder.Unregister(name);
						Console.WriteLine(System.String.Concat("Uninstalled custom culture [", name, "]"));
					} else {
						customculture.Register();
						Console.WriteLine(System.String.Concat("Installed custom culture [", name, "]"));
					}
				} catch ( System.Exception ) {
					if ( culture!=null )
						Console.WriteLine(System.String.Concat("Error uninstalling custom culture [", name, "]"));
					else
						Console.WriteLine(System.String.Concat("Error installing custom culture [", name, "]"));
				}
			} else
				Console.WriteLine(System.String.Concat("Unsupported custom culture [", name, "]"));
			
				
		}
	}
}
