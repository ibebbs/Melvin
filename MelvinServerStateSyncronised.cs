using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinServerStateSyncronised.
	/// </summary>
	internal class MelvinServerStateSyncronised : MelvinServerStateBase
	{
		public MelvinServerStateSyncronised (MelvinServer melvinServer) : base(melvinServer) {}

		public override MelvinServerState State
		{
			get
			{
				return MelvinServerState.Syncronised;
			}
		}

		public override void ImageRequestStart()
		{
			TransitionState(MelvinServerState.Syncronising);
		}

		public override void Disconnect()
		{
			TransitionState(MelvinServerState.Disconnected);
		}

		public override void ItemAdded(object key, object value)
		{
			MelvinMessage message = MelvinMessageFactory.CreateSyncronisedItemAddedMessage(key, value);
			SendMessage(message);
		}

		public override void ItemUpdated(object key, object update, object updatedItem)
		{
			MelvinMessage message = MelvinMessageFactory.CreateSyncronisedItemUpdatedMessage(key, update, updatedItem);
			SendMessage(message);
		}

		public override void ItemRemoved(object key, object value)
		{
			MelvinMessage message = MelvinMessageFactory.CreateSyncronisedItemRemovedMessage(key, value);
			SendMessage(message);
		}

		public override void SyncronisationEnd()
		{
			// Do nothing - protocol error
		}

	}
}
