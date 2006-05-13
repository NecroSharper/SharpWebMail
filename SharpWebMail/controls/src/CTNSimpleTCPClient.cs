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

namespace anmar.SharpWebMail
{

	internal class CTNSimpleTCPClient {
		// Create a logger for use in this class
		private static log4net.ILog log  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected System.String lastErrorMessage;
		protected System.Net.Sockets.TcpClient client;
		protected long timeoutResponse = 20000;

		public CTNSimpleTCPClient() {
		}
		
		public CTNSimpleTCPClient( long timeout ) {
			this.timeoutResponse = timeout;
		}

		public bool connect( System.String host, System.Int32 port ) {
			bool error = false;
			this.init();
			try {
				if ( log.IsDebugEnabled )
					log.Debug(System.String.Concat("Connecting to host: ", host, " port: ", port));
				client.Connect( host, port );
			} catch ( System.ArgumentNullException e ) {
				error = true;
				lastErrorMessage = "Please provide a valid hostname";
				if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
			} catch ( System.ArgumentOutOfRangeException e ) {
				error = true;
				lastErrorMessage = "Please provide a valid port number";
				if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
			} catch ( System.Net.Sockets.SocketException e ) {
				error = true;
				lastErrorMessage = "Error while trying to open socket";
				if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
			}
			return !error;
		}
		public bool closeConnection () {
			bool error = false;
			try {
				client.Close();
			} catch ( System.Net.Sockets.SocketException e ) {
				error = true;
				lastErrorMessage = "Error while trying to close socket";
				if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
			}
			return !error;
		}

		// Get NetworkStream Object from TCPClient
		protected bool getStream ( ref System.Net.Sockets.NetworkStream ns ) {
			bool error = false;
			try {
				ns = client.GetStream();
			} catch ( System.Exception e ) {
				error = true;
				lastErrorMessage = "Connection is not propertly established";
				if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
			}
			return !error;
		}
		protected void init () {
			client = new System.Net.Sockets.TcpClient();
		}
		public bool readResponse ( System.IO.MemoryStream response, System.String waitFor, bool machresponseend ) {
			bool error = false;
			System.Net.Sockets.NetworkStream ns = null;

			//Get NetworkStream
			error = !this.getStream( ref ns );

			//Get response from NetworkStream
			error = ( error )?error:!readBytes( ns, response, waitFor, machresponseend );

			return !error;
		}
		public bool readResponse ( ref System.String response, System.String waitFor, bool machresponseend ) {
			bool error = false;
			System.Net.Sockets.NetworkStream ns = null;

			//Get NetworkStream
			error = !this.getStream( ref ns );

			//Get response from NetworkStream
			error = ( error )?error:!this.readString( ns, ref response, waitFor, machresponseend );

			return !error;
		}
		protected bool readBytes ( System.Net.Sockets.NetworkStream ns, System.IO.MemoryStream response, System.String waitFor, bool machresponseend ) {
			bool error = false;
			byte[] readBytes = new byte[client.ReceiveBufferSize];
			int nbytes = 0;
			System.String lastBoundary = System.String.Empty;
			WaitState state = new WaitState(true);
			System.Threading.Timer aTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.StopWaiting), state, System.Threading.Timeout.Infinite,  System.Threading.Timeout.Infinite);

			if ( log.IsDebugEnabled ) log.Debug ( "Reading response" );
			// We wait until data is available but only if Stream is open
			// We setup a timer that stops the loop after x seconds
			for ( aTimer.Change(this.timeoutResponse,  System.Threading.Timeout.Infinite); !error && ns.CanRead && ns.CanWrite && !ns.DataAvailable && state.Status; ){System.Threading.Thread.Sleep(50);}
			state.Status = true;

			// If I can read from NetworkStream and there is
			// some data, I get it
			for ( aTimer.Change(this.timeoutResponse,  System.Threading.Timeout.Infinite); !error && ns.CanRead && state.Status && (ns.DataAvailable || !(lastBoundary.Equals(waitFor)) ) ; nbytes = 0) {
				try {
					if ( ns.DataAvailable ) {
#if MONO
						// Reinitialize buffer to make mono happy
						readBytes = new byte[client.ReceiveBufferSize];
#endif
						nbytes = ns.Read( readBytes, 0, client.ReceiveBufferSize );
					} else
						System.Threading.Thread.Sleep(50);
				} catch ( System.Exception e ) {
					error = true;
					nbytes = 0;
					lastErrorMessage = "Read error";
					if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
				}
				if ( !error && nbytes>0 ) {
					if ( log.IsDebugEnabled ) log.Debug ( "Read " + nbytes + " bytes" );
					response.Write( readBytes, 0, nbytes );
					// Only test waitfor secuence if there is no data for reading
					// and there are enouth data available for comparing
					if ( !ns.DataAvailable && response.Length>waitFor.Length ) {
						// The waitfor text must be the last portion of the response
						if ( machresponseend ) {
							response.Seek(response.Length - waitFor.Length, System.IO.SeekOrigin.Begin);
							response.Read(readBytes,  0, waitFor.Length);
							lastBoundary = System.Text.Encoding.ASCII.GetString(readBytes, 0, waitFor.Length);
						// The waitfor text must be in the begining of the last line of the response
						} else {
							response.Seek(0, System.IO.SeekOrigin.Begin);
							System.IO.StreamReader reader = new System.IO.StreamReader(response);
							System.String line = System.String.Empty;
							for ( System.String tmp=reader.ReadLine(); tmp!=null ; line=tmp, tmp=reader.ReadLine() ) {}
							if ( line!=null && line.Length>=waitFor.Length )
								lastBoundary = line.Substring(0, waitFor.Length);
							reader.DiscardBufferedData();
							reader=null;
							response.Seek (0, System.IO.SeekOrigin.End);
						}
					}
					// Reset timer
					aTimer.Change(this.timeoutResponse,  System.Threading.Timeout.Infinite);
					state.Status = true;
				}
			}
			response.Flush();
			if ( log.IsDebugEnabled ) log.Debug ( System.String.Concat("Reading response finished. Error: ", error) );
			// Discard response if there has been a read error.
			if ( error )
				response.SetLength(0);
			else if ( response.Length==0 )
				error = true;
			return !error;
		}
		protected bool readString ( System.Net.Sockets.NetworkStream ns, ref System.String response, System.String waitFor, bool machresponseend) {
			bool error = false;
			if ( log.IsDebugEnabled ) log.Debug ( "Reading response string" );
			response = System.String.Empty;
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			error = !this.readBytes(ns, stream, waitFor, machresponseend);
			if ( !error ) {
				response = System.Text.Encoding.ASCII.GetString(stream.GetBuffer(), 0, (int)stream.Length );
				response = response.Trim();
				if ( log.IsDebugEnabled ) log.Debug ( "Response string read: " + response );
			}
			error = (error||response.Length==0)?true:false;
			return !error;
		}
		public bool sendCommand ( System.String cmd, System.String end ) {
			bool error = false;
			System.Net.Sockets.NetworkStream ns = null;

			//Get NetWork Stream
			error = !this.getStream( ref ns );

			// Send the command
			error = ( !error && cmd.Length>0 )?!this.sendString ( ns, System.String.Concat( cmd, end ) ):true;

			return !error;
		}
		protected bool sendString ( System.Net.Sockets.NetworkStream ns, System.String cmd ) {
			bool error = false;
			byte[] sendBytes;

			// Check string length
			if ( !(cmd.Length>0) ) {
				error = true;
				lastErrorMessage = "There should be something to send";
			} else {
				if ( log.IsDebugEnabled ) log.Debug ( "Sending string " + cmd);
				sendBytes = System.Text.Encoding.ASCII.GetBytes( cmd );
				// Check previous error and if network stream is writable
				if ( ns.CanWrite ){
					try {
						ns.Write( sendBytes, 0, sendBytes.Length );
					} catch ( System.IO.IOException e ) {
						error = true;
						lastErrorMessage = "Write error";
						if ( log.IsErrorEnabled ) log.Error ( lastErrorMessage, e );
					}
				}
				if ( log.IsDebugEnabled ) log.Debug (System.String.Concat ("String sent. error: ", error));
			}
			return !error;
		}
		protected void StopWaiting (System.Object state) {
			((WaitState)state).Status = false;
			return;
		}

		public string errormessage {
			get {
				return this.lastErrorMessage;
			}
		}
		private class WaitState {
			private bool wait;
			public WaitState (bool wait) {
				this.wait = wait;
			}
			public bool Status {
				get { return this.wait;}
				set { this.wait=value;}
			}
		}
	}
	
}
