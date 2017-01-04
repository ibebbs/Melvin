using System;
using System.Collections;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinServerStateSyncronising.
	/// </summary>
	internal class MelvinServerStateSyncronising : MelvinServerStateBase
	{
		public MelvinServerStateSyncronising (MelvinServer melvinServer) : base(melvinServer) {}

		public override MelvinServerState State
		{
			get
			{
				return MelvinServerState.Syncronising;
			}
		}

		public override void EnterState()
		{
			SendImageResponseStart();
			SendImageResponseItems();
			SendImageResponseEnd();
		}

		private void SendImageResponseStart()
		{
			MelvinMessage message = MelvinMessageFactory.CreateImageResponseStartMessage();
			SendMessage(message);
		}

		private void SendImageResponseItems()
		{
			foreach (DictionaryEntry item in MelvinServer.CacheBase.Entries)
			{
				MelvinMessage message = MelvinMessageFactory.CreateImageResponseItemMessage(item.Key, item.Value);
				SendMessage(message);
			}
		}

		private void SendImageResponseEnd()
		{
			MelvinMessage message = MelvinMessageFactory.CreateImageResponseEndMessage();
			SendMessage(message);
		}

		public override void SyncronisationStart()
		{
			TransitionState(MelvinServerState.Syncronised);
		}

	}
}
