using SolutionForge.Mobile.Melvin;
using SolutionForge.Mobile.Net;
using System;

namespace SolutionForge.Mobile.Melvin.Net
{
	/// <summary>
	/// Summary description for MelvinMessageSocketInterface.
	/// </summary>
	public class MelvinMessageSocketInterface : IMelvinMessageInterface
	{
		private const string MESSAGEWRAPPER_START = "<MelvinMessagePacket>";
		private const string MESSAGEWRAPPER_END = "</MelvinMessagePacket>";

		private AsyncSocketManager m_socketManager;
		private PacketBuilder m_packetBuilder;

		public MelvinMessageSocketInterface (AsyncSocketManager socketManager)
		{
			m_packetBuilder = new PacketBuilder();
			m_packetBuilder.PacketStart = MESSAGEWRAPPER_START;
			m_packetBuilder.PacketEnd = MESSAGEWRAPPER_END;
			m_packetBuilder.PacketComplete += new PacketCompleteHandler(packetBuilder_PacketComplete);

			m_socketManager = socketManager;
			m_socketManager.Received += new AsyncSocketManagerReceivedHandler(socketManager_Received);
		}

		#region IMelvinMessageInterface Members

		public void SendMessage(string message)
		{
			m_socketManager.Send(string.Format("{0}{1}{2}", MESSAGEWRAPPER_START, message, MESSAGEWRAPPER_END));
		}

		public event Melvin.ReceiveMessageHandler ReceiveMessage;

		#endregion

		private void socketManager_Received(AsyncSocketManager asyncSocketClient, string data)
		{
			m_packetBuilder.Add(data);
		}

		private void packetBuilder_PacketComplete(PacketBuilder packetBuilder, string packet)
		{
			string melvinMessage = packet.Replace(MESSAGEWRAPPER_START, string.Empty).Replace(MESSAGEWRAPPER_END, string.Empty);

			if ( ReceiveMessage != null )
				ReceiveMessage(melvinMessage);
		}
	}
}
