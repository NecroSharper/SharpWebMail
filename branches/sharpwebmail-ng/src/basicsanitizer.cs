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

namespace anmar.SharpWebMail
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class BasicSanitizer {
		private static System.Object[] comment = new System.Object[] {
			// Comment all <script></script> occurrences
			new System.Text.RegularExpressions.Regex(@"(<script([^<]*(?:(?!</)<){0,1})*</script[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment everything before <body>
			new System.Text.RegularExpressions.Regex(@"(.*<body[^>]*>)", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment everything after </body>
			new System.Text.RegularExpressions.Regex(@"(</body[^>]*>.*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment all <applet></applet> occurrences
			new System.Text.RegularExpressions.Regex(@"(<applet([^<]*(?:(?!</)<){0,1})*</applet[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment all <embed></embed> occurrences
			new System.Text.RegularExpressions.Regex(@"(<embed([^<]*(?:(?!</)<){0,1})*</embed[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment all <object></object> occurrences
			new System.Text.RegularExpressions.Regex(@"(<object([^<]*(?:(?!</)<){0,1})*</object[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment all <layer></layer> occurrences
			new System.Text.RegularExpressions.Regex(@"(<layer([^<]*(?:(?!</)<){0,1})*</layer[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment all <ilayer></ilayer> occurrences
			new System.Text.RegularExpressions.Regex(@"(<ilayer([^<]*(?:(?!</)<){0,1})*</ilayer[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline),
			// Comment all <iframe></iframe> occurrences
			new System.Text.RegularExpressions.Regex(@"(<iframe([^<]*(?:(?!</)<){0,1})*</iframe[^>]*>)+", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline)
		};
		private static System.Text.RegularExpressions.Regex events = new System.Text.RegularExpressions.Regex(@"(?:\b(on)(Abort|Blur|Change|Click|DblClick|DblClick|Error|Focus|KeyDown|KeyPress|KeyPress|Load|MouseDown|MouseMove|MouseMove|MouseOver|MouseUp|Move|Reset|Resize|Select|Submit|Unload)(\s*=))", System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline);
		/// <summary>
		/// 
		/// </summary>
		public static System.String SanitizeHTML ( System.String htmlstring, anmar.SharpWebMail.SanitizerMode mode ) {
			if ( (mode&anmar.SharpWebMail.SanitizerMode.CommentBlocks)==anmar.SharpWebMail.SanitizerMode.CommentBlocks ) {
				foreach ( System.Text.RegularExpressions.Regex item in  anmar.SharpWebMail.BasicSanitizer.comment ) {
					htmlstring = item.Replace(htmlstring, "<!-- Commented by SharpWebMail \r\n$1\r\n -->");
				}
			}
			if ( (mode&anmar.SharpWebMail.SanitizerMode.RemoveEvents)==anmar.SharpWebMail.SanitizerMode.RemoveEvents ) {
				htmlstring = anmar.SharpWebMail.BasicSanitizer.events.Replace(htmlstring, "$1_$2$3");
			}
			return htmlstring;
		}
	}
	/// <summary>
	/// 
	/// </summary>	
	[Flags]
	public enum SanitizerMode {
		/// <summary>
		/// 
		/// </summary>
		CommentBlocks,
		/// <summary>
		/// 
		/// </summary>
		RemoveEvents
	}
}
