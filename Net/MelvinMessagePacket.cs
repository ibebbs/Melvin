using SolutionForge.Mobile.Melvin;
using System;

namespace SolutionForge.Mobile.Melvin.Net
{
	/// <summary>
	/// Summary description for MelvinMessagePacket.
	/// </summary>
	public class MelvinMessagePacket
	{
		private MelvinMessage m_melvinMessage;

		public MelvinMessagePacket(MelvinMessage message)
		{
			m_melvinMessage = message;
		}
	}
}
