using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinClientStateDisconnected.
	/// </summary>
	internal class MelvinClientStateDisconnected : MelvinClientStateBase
	{
		public MelvinClientStateDisconnected (MelvinClient m_melvinClient) : base(m_melvinClient) {}

		public override MelvinClientState State
		{
			get { return MelvinClientState.Disconnected; }
		}

		public override void Connect ()
		{
			MelvinMessage message = MelvinMessageFactory.CreateConnectionMessage();
			SendMessage(message);
		}

		public override void ConnectionAcknowledgementReceived ()
		{
			TransitionState(MelvinClientState.Connected);
		}
	}
}
