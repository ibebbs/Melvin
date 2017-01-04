using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinServerStateConnected.
	/// </summary>
	internal class MelvinServerStateConnected : MelvinServerStateBase
	{
		public MelvinServerStateConnected (MelvinServer melvinServer) : base(melvinServer) {}

		public override MelvinServerState State
		{
			get { return MelvinServerState.Connected; }
		}

		public override void EnterState ()
		{
			// Acknowledge that the connection has been successful
			MelvinMessage message = MelvinMessageFactory.CreateConnectionAcknowledgement();
			SendMessage(message);

			// Start heartbeats
			MelvinServer.StartHeartbeats();
		}

		public override void Disconnect()
		{
			TransitionState(MelvinServerState.Disconnected);
		}

		public override void ImageRequestStart ()
		{
			TransitionState(MelvinServerState.Syncronising);
		}
	}
}
