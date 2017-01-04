using SolutionForge.Mobile.Caching;
using SolutionForge.Mobile.Melvin;
using SolutionForge.Mobile.Net;
using System;

namespace SolutionForge.Mobile.Melvin.Net
{
	public enum MelvinNetworkClientState
	{
		Disconnected,
		Connected,
		Syncronising,
		Syncronised
	}

	/// <summary>
	/// Summary description for MelvinClientManager.
	/// </summary>
	public class MelvinNetworkClient
	{
		private const int CLIENT_BUFFERSIZE = 4096;

		private MelvinMessageSocketInterface m_melvinMessageSocketInterface;
		private MelvinClient m_melvinClient;
		private MelvinClientState m_melvinClientState = MelvinClientState.Disconnected;
		private AsyncSocketManager m_clientSocket;
		private string m_serverAddress;
		private int m_serverPort;

		public event EventHandler MelvinNetworkClientStateChanged;

		public MelvinNetworkClient(string serverAddress, int serverPort, CacheBase cacheBase, IMelvinMessageAdapter melvinMessageAdapter)
		{
			m_serverAddress = serverAddress;
			m_serverPort = serverPort;

			m_clientSocket = new AsyncSocketManager(CLIENT_BUFFERSIZE);
			m_clientSocket.StateChanged += new EventHandler(clientSocket_StateChanged);

			m_melvinMessageSocketInterface = new MelvinMessageSocketInterface(m_clientSocket);
			m_melvinClient = new MelvinClient(cacheBase, m_melvinMessageSocketInterface, melvinMessageAdapter);
			m_melvinClient.MelvinClientStateChanged += new MelvinClientStateChangeHandler(melvinClient_MelvinClientStateChanged);
		}

		private void SocketConnected ()
		{
			m_melvinClient.Open();
		}

		private void SocketDisconnected()
		{
		}

		private void clientSocket_StateChanged(object sender, EventArgs e)
		{
			switch ( m_clientSocket.CurrentState )
			{
				case AsyncSocketManagerState.Connected:
					SocketConnected();
					break;
				case AsyncSocketManagerState.Disconnnected:
					SocketDisconnected();
					break;
			}
		}

		private void melvinClient_MelvinClientStateChanged(MelvinClient melvinClient, MelvinClientState state)
		{
			MelvinNetworkClientState oldState = CurrentState;

			m_melvinClientState = state;

			if ( state == MelvinClientState.Disconnected && m_clientSocket.CurrentState != AsyncSocketManagerState.Disconnnected )
				m_clientSocket.Disconnect();

			if ( MelvinNetworkClientStateChanged != null && oldState != CurrentState )
				MelvinNetworkClientStateChanged(this, EventArgs.Empty);
		}

		public void Open ()
		{
			m_clientSocket.Connect(m_serverAddress, m_serverPort);
		}

		public void Close ()
		{
			if ( CurrentState != MelvinNetworkClientState.Disconnected )
				m_melvinClient.Close();

			if ( m_clientSocket.CurrentState != AsyncSocketManagerState.Disconnnected )
				m_clientSocket.Disconnect();
		}

		public MelvinNetworkClientState CurrentState
		{
			get
			{
				switch ( m_melvinClientState )
				{
					case MelvinClientState.Connected:
						return MelvinNetworkClientState.Connected;
					case MelvinClientState.Syncronising:
						return MelvinNetworkClientState.Syncronising;
					case MelvinClientState.Syncronised:
						return MelvinNetworkClientState.Syncronised;
					default:
						return MelvinNetworkClientState.Disconnected;
				}
			}
		}
	}
}
