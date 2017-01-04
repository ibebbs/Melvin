using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinClientStateDisconnecting.
	/// </summary>
	internal class MelvinClientStateDisconnecting : MelvinClientStateBase
	{
		public MelvinClientStateDisconnecting (MelvinClient m_melvinClient) : base(m_melvinClient) {}

		public override MelvinClientState State
		{
			get { return MelvinClientState.Disconnecting; }
		}

		public override void EnterState(MelvinClientState lastState)
		{
            //MelvinClient.StopHeartbeatTimer();
		}

		public override void DisconnectionAcknowledgementReceived()
		{
			TransitionState(MelvinClientState.Disconnected);
		}
	}
}
