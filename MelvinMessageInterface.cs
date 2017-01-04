using System;

namespace SolutionForge.Mobile.Melvin
{
	public delegate void ReceiveMessageHandler (string message);

	/// <summary>
	/// Summary description for MelvinMessageInterface.
	/// </summary>
	public interface IMelvinMessageInterface
	{
		event ReceiveMessageHandler ReceiveMessage;
		void SendMessage(string message);
	}
}
