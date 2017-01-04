using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinClientStateConnected.
	/// </summary>
	internal class MelvinClientStateConnected : MelvinClientStateBase
	{
		public MelvinClientStateConnected (MelvinClient m_melvinClient) : base(m_melvinClient) {}

		public override MelvinClientState State
		{
			get { return MelvinClientState.Connected; }
		}

		public override void EnterState(MelvinClientState lastState)
		{
			//MelvinClient.StartHeartbeatTimer();
			MelvinMessage message = MelvinMessageFactory.CreateImageRequestStartMessage();
			SendMessage(message);
		}

		public override void ImageResposeStartRecieved()
		{
			TransitionState(MelvinClientState.Syncronising);
		}

		public override void Disconnect()
		{
			TransitionState(MelvinClientState.Disconnecting);

			MelvinMessage message = MelvinMessageFactory.CreateDisconnectionMessage();
			SendMessage(message);
		}
	}
}
