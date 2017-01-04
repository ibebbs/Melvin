using SolutionForge.Mobile.Caching;
using SolutionForge.Mobile.Melvin;
using SolutionForge.Mobile.Net;
using System;
using System.Collections;

namespace SolutionForge.Mobile.Melvin.Net
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class MelvinNetworkServer
	{
		private const int SERVER_BUFFERSIZE = 4096;
		private IMelvinMessageAdapter m_melvinMessageAdapter;
		private AsyncSocketManager m_serverListener;
		private CacheBase m_cacheBase;
		private Hashtable m_connections;
		private int m_serverPort;

		public MelvinNetworkServer(int serverPort, CacheBase cacheBase, IMelvinMessageAdapter melvinMessageAdapter)
		{
			m_cacheBase = cacheBase;
			m_serverPort = serverPort;
			m_melvinMessageAdapter = melvinMessageAdapter;
			
			m_connections = new Hashtable();

			m_serverListener = new AsyncSocketManager(SERVER_BUFFERSIZE);
			m_serverListener.NewConnection += new AsyncSocketManagerNewConnectionHandler(NewMelvinConnection);
		}

		private void NewMelvinConnection(AsyncSocketManager asyncSocketClient, AsyncSocketManager newConnection)
		{
			MelvinMessageSocketInterface melvinMessageSocketInterface = new MelvinMessageSocketInterface(newConnection);
			MelvinServer newConnectionServer = new MelvinServer(m_cacheBase, melvinMessageSocketInterface, m_melvinMessageAdapter);
			newConnection.StateChanged += new EventHandler(connection_StateChanged);

			m_connections.Add(newConnection, newConnectionServer);
		}

		private void ConnectionSocketConnected (AsyncSocketManager socket)
		{
		}

		private void ConnectionSocketDisconnected(AsyncSocketManager socket)
		{
			MelvinServer socketServer = (MelvinServer) m_connections[socket];
			
			if ( socketServer != null )
			{
				m_connections.Remove(socket);
				socketServer.Dispose();
			}			
		}

		private void connection_StateChanged(object sender, EventArgs e)
		{
			AsyncSocketManager socket = (sender as AsyncSocketManager);
			
			if ( socket != null )
				switch ( socket.CurrentState )
				{
					case AsyncSocketManagerState.Connected:
						ConnectionSocketConnected(socket);
						break;
					case AsyncSocketManagerState.Disconnnected:
						ConnectionSocketDisconnected(socket);
						break;
				}
		}

		public void Open()
		{	
			m_serverListener.Listen(m_serverPort);
		}

		public void Close()
		{
			m_serverListener.Disconnect();
		}
	}
}