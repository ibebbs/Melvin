using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinServerStateDisconnected.
	/// </summary>
	internal class MelvinServerStateDisconnected : MelvinServerStateBase
	{
		public MelvinServerStateDisconnected (MelvinServer melvinServer) : base(melvinServer) {}

		public override MelvinServerState State
		{
			get { return MelvinServerState.Disconnected; }
		}

		public override void EnterState()
		{
			MelvinMessage message = MelvinMessageFactory.CreateDisconnectionAcknowledgement();
			SendMessage(message);
		}

		public override void Connect ()
		{
			TransitionState(MelvinServerState.Connected);
		}
	}
}
