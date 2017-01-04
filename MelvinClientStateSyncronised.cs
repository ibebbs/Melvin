using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinClientStateSyncronised.
	/// </summary>
	internal class MelvinClientStateSyncronised : MelvinClientStateBase
	{
		public MelvinClientStateSyncronised (MelvinClient m_melvinClient) : base(m_melvinClient) {}

		public override MelvinClientState State
		{
			get { return MelvinClientState.Syncronised; }
		}

		public override void SyncronisationItemAdded(object key, object value)
		{
			MelvinClient.CacheBase.Add(key, value);
		}
		
		public override void SyncronisationItemUpdated(object key, object value)
		{
			object updatedItem = MelvinClient.CacheBase[key];

			MelvinClient.CacheBase.Update(key, value, updatedItem);
		}

		public override void SyncronisationItemRemoved(object key, object value)
		{
			MelvinClient.CacheBase.Remove(key);
		}

		public override void Disconnect()
		{
			TransitionState(MelvinClientState.Disconnecting);

			MelvinMessage message = MelvinMessageFactory.CreateSyncronisationEndMessage();
			SendMessage(message);

			message = MelvinMessageFactory.CreateDisconnectionMessage();
			SendMessage(message);
		}

	}
}
