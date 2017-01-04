using SolutionForge.Mobile.Caching;
using System;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class MelvinServer : IDisposable
	{
		private CacheBase m_cacheBase;
		private IMelvinMessageInterface m_melvinMessageInterface;
		private MelvinServerMessageFactory m_melvinMessageFactory;
		private MelvinServerStateBase m_currentState;

		public MelvinServer(CacheBase cacheBase, IMelvinMessageInterface melvinMessageInterface, IMelvinMessageAdapter melvinMessageAdapter)
		{
			m_cacheBase = cacheBase;
			m_melvinMessageInterface = melvinMessageInterface;
			m_melvinMessageInterface.ReceiveMessage += new ReceiveMessageHandler(melvinMessageInterface_ReceiveMessage);

			m_melvinMessageFactory = new MelvinServerMessageFactory(melvinMessageAdapter);

			m_cacheBase.ItemAdded += new CacheItemAddedHandler(cacheBase_ItemAdded);
			m_cacheBase.ItemUpdated += new CacheItemUpdatedHandler(cacheBase_ItemUpdated);
			m_cacheBase.ItemRemoved += new CacheItemRemovedHandler(cacheBase_ItemRemoved);

			m_currentState = new MelvinServerStateDisconnected(this);
		}

		#region IDisposable Members

		public void Dispose()
		{
			m_cacheBase.ItemAdded -= new CacheItemAddedHandler(cacheBase_ItemAdded);
			m_cacheBase.ItemUpdated -= new CacheItemUpdatedHandler(cacheBase_ItemUpdated);
			m_cacheBase.ItemRemoved -= new CacheItemRemovedHandler(cacheBase_ItemRemoved);
		}

		#endregion

		#region Cache Events

		private void cacheBase_ItemAdded(ICache cache, object key, object value)
		{
			m_currentState.ItemAdded(key, value);
		}

		private void cacheBase_ItemUpdated(ICache cache, object key, object update, object updatedItem)
		{
			m_currentState.ItemUpdated(key, update, updatedItem);
		}

		private void cacheBase_ItemRemoved(ICache cache, object key, object value)
		{
			m_currentState.ItemRemoved(key, value);
		}

		#endregion Cache Events

		#region State Methods

		private void Connect ()
		{
			m_currentState.Connect();
		}

		private void ImageRequestStart ()
		{
			m_currentState.ImageRequestStart();
		}

		private void ImageRequestEnd ()
		{
			m_currentState.ImageRequestEnd();
		}

		private void SyncronisationStart ()
		{
			m_currentState.SyncronisationStart();
		}

		private void SyncronisationEnd ()
		{
			m_currentState.SyncronisationEnd();
		}

		private void Disconnect ()
		{
			m_currentState.Disconnect();
		}

		#endregion State Methods

		#region Message Events

		private void ProcessConnection (MelvinMessage message)
		{
			switch (message.Operation)
			{
				case MelvinMessageOperation.Begin:
					Connect();
					break;
				case MelvinMessageOperation.End:
					Disconnect ();
					break;
				default:
					MelvinMessage response = MelvinMessageFactory.CreateUnknownOperationMessage(message);
					SendMessage(message);
					break;
			}
		}
		
		private void ProcessImageRequest (MelvinMessage message)
		{
			switch (message.Operation)
			{
				case MelvinMessageOperation.Begin:
					ImageRequestStart();
					break;
				case MelvinMessageOperation.End:
					ImageRequestEnd();
					break;
				default:
					MelvinMessage response = MelvinMessageFactory.CreateUnknownOperationMessage(message);
					SendMessage(message);
					break;
			}
		}
		
		private void ProcessSyncronisation (MelvinMessage message)
		{
			switch (message.Operation)
			{
				case MelvinMessageOperation.Begin:
					SyncronisationStart();
					break;
				case MelvinMessageOperation.End:
					SyncronisationEnd();
					break;
				default:
					MelvinMessage response = MelvinMessageFactory.CreateUnknownOperationMessage(message);
					SendMessage(message);
					break;
			}
		}

		private void ProcessMessage (MelvinMessage message)
		{
			switch (message.Category)
			{
				case MelvinMessageCategory.Connection:
					ProcessConnection(message);
					break;
				case MelvinMessageCategory.ImageRequest:
					ProcessImageRequest(message);
					break;
				case MelvinMessageCategory.Syncronisation:
					ProcessSyncronisation(message);
					break;
				default:
					MelvinMessage response = MelvinMessageFactory.CreateUnknownCategoryMessage(message);
					SendMessage(response);
					break;
			}
		}

		private void melvinMessageInterface_ReceiveMessage(string message)
		{
			MelvinMessage deserialisedMessage = MelvinMessageSerialiser.Deserialise(message);

			lock(this)
			{
				ProcessMessage(deserialisedMessage);
			}
		}

		#endregion Message Events

		#region Message State Accessors

		internal void TransitionState(MelvinServerState state)
		{
			if ( m_currentState != null )
				m_currentState.LeaveState();

			m_currentState = null;

			switch (state)
			{
				case MelvinServerState.Connected:
					m_currentState = new MelvinServerStateConnected(this);
					break;
				case MelvinServerState.Disconnected:
					m_currentState = new MelvinServerStateDisconnected(this);
					break;
				case MelvinServerState.Syncronised:
					m_currentState = new MelvinServerStateSyncronised(this);
					break;
				case MelvinServerState.Syncronising:
					m_currentState = new MelvinServerStateSyncronising(this);
					break;
			}

			if ( m_currentState != null )
				m_currentState.EnterState();
		}

		internal CacheBase CacheBase
		{
			get { return m_cacheBase; }
		}

		internal MelvinServerMessageFactory MelvinMessageFactory
		{
			get { return m_melvinMessageFactory; }
		}

		internal void StartHeartbeats ()
		{
			// Start heart beat thread
		}

		internal void StopHeartbeats ()
		{
			// Stop heart beat thread
		}

		internal void SendMessage (MelvinMessage message)
		{
			string serialisedMessage = MelvinMessageSerialiser.Serialise(message);

			m_melvinMessageInterface.SendMessage(serialisedMessage);
		}

		#endregion Message State Accessors
	}
}
