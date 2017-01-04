using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinClientStateSyncronising.
	/// </summary>
	internal class MelvinClientStateSyncronising : MelvinClientStateBase
	{
		public MelvinClientStateSyncronising (MelvinClient m_melvinClient) : base(m_melvinClient) {}

		public override MelvinClientState State
		{
			get { return MelvinClientState.Syncronising; }
		}

		public override void ImageResponseItemReceived(object key, object value)
		{
			MelvinClient.CacheBase.Add(key, value);
		}

		public override void ImageResponseEndReceived()
		{
			MelvinMessage message = MelvinMessageFactory.CreateSyncronisationBeginMessage();
			SendMessage(message);

			TransitionState(MelvinClientState.Syncronised);
		}

		public override void Disconnect()
		{
			TransitionState(MelvinClientState.Disconnecting);

			MelvinMessage message = MelvinMessageFactory.CreateImageRequestEndMessage();
			SendMessage(message);

			message = MelvinMessageFactory.CreateDisconnectionMessage();
			SendMessage(message);
		}
	}
}
