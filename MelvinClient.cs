using SolutionForge.Mobile.Caching;
using System;

namespace SolutionForge.Mobile.Melvin
{
	public delegate void MelvinClientStateChangeHandler (MelvinClient melvinClient, MelvinClientState state);

	/// <summary>
	/// Summary description for MelvinClient.
	/// </summary>
	public class MelvinClient
	{
		private CacheBase m_cacheBase;
		private IMelvinMessageInterface m_melvinMessageInterface;
		private IMelvinMessageAdapter m_melvinMessageAdapter;
		private MelvinClientMessageFactory m_melvinMessageFactory;
		private MelvinClientStateBase m_currentState;

		public event MelvinClientStateChangeHandler MelvinClientStateChanged;

		public MelvinClient(CacheBase cacheBase, IMelvinMessageInterface melvinMessageInterface, IMelvinMessageAdapter melvinMessageAdapter)
		{
			m_cacheBase = cacheBase;
			m_melvinMessageAdapter = melvinMessageAdapter;
			m_melvinMessageInterface = melvinMessageInterface;
			m_melvinMessageInterface.ReceiveMessage += new ReceiveMessageHandler(melvinMessageInterface_ReceiveMessage);

			m_melvinMessageFactory = new MelvinClientMessageFactory(melvinMessageAdapter);

			m_currentState = new MelvinClientStateDisconnected(this);
		}

		#region Message Events

		private void ProcessConnection (MelvinMessage message)
		{
			switch (message.Operation)
			{
				case MelvinMessageOperation.Begin:
					ConnectionAcknowledgementReceived();
					break;
				case MelvinMessageOperation.End:
					DisconnectionAcknowledgementReceived();
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
					ImageResposeStartRecieved();
					break;
				case MelvinMessageOperation.Add:
					ImageResponseItemReceived(message);
					break;
				case MelvinMessageOperation.End:
					ImageResponseEndReceived();
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
				case MelvinMessageOperation.Add:
					SyncronisationItemAdded(message);
					break;
				case MelvinMessageOperation.Update:
					SyncronisationItemUpdated(message);
					break;			
				case MelvinMessageOperation.Remove:
					SyncronisationItemRemoved(message);
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

		internal void OnStateChange (MelvinClientState state)
		{
			if ( MelvinClientStateChanged != null )
				MelvinClientStateChanged(this, state);
		}

		internal void TransitionState(MelvinClientState state)
		{
			MelvinClientState lastState = m_currentState.State;
			m_currentState.LeaveState(state);

			m_currentState = null;

			switch (state)
			{
				case MelvinClientState.Connected:
					m_currentState = new MelvinClientStateConnected(this);
					break;
				case MelvinClientState.Disconnecting:
					m_currentState = new MelvinClientStateDisconnecting(this);
					break;
				case MelvinClientState.Disconnected:
					m_currentState = new MelvinClientStateDisconnected(this);
					break;
				case MelvinClientState.Syncronised:
					m_currentState = new MelvinClientStateSyncronised(this);
					break;
				case MelvinClientState.Syncronising:
					m_currentState = new MelvinClientStateSyncronising(this);
					break;
			}

			if ( m_currentState == null )
				throw new ApplicationException("Invalid state transition");

			m_currentState.EnterState(lastState);
			OnStateChange(m_currentState.State);
		}

		internal CacheBase CacheBase
		{
			get { return m_cacheBase; }
		}

		internal MelvinClientMessageFactory MelvinMessageFactory
		{
			get { return m_melvinMessageFactory; }
		}

		internal void SendMessage (MelvinMessage message)
		{
			string serialisedMessage = MelvinMessageSerialiser.Serialise(message);

			m_melvinMessageInterface.SendMessage(serialisedMessage);
		}

		#endregion Message State Accessors

		#region MelvinServer Events

		public void ConnectionAcknowledgementReceived ()
		{
			m_currentState.ConnectionAcknowledgementReceived();
		}

		public void ImageResposeStartRecieved ()
		{
			m_currentState.ImageResposeStartRecieved();
		}

		public void ImageResponseEndReceived ()
		{
			m_currentState.ImageResponseEndReceived();
		}

		public void ImageResponseItemReceived (MelvinMessage message)
		{
			object key = m_melvinMessageAdapter.DeserialiseItemKey(message.Body.Key);
			object value = m_melvinMessageAdapter.DeserialiseItemValue(message.Body.Value);

			m_currentState.ImageResponseItemReceived(key, value);
		}

		public virtual void SyncronisationItemAdded (MelvinMessage message)
		{
			object key = m_melvinMessageAdapter.DeserialiseItemKey(message.Body.Key);
			object value = m_melvinMessageAdapter.DeserialiseItemValue(message.Body.Value);

			m_currentState.SyncronisationItemAdded(key, value);
		}

		public virtual void SyncronisationItemUpdated (MelvinMessage message)
		{
			object key = m_melvinMessageAdapter.DeserialiseItemKey(message.Body.Key);
			object value = m_melvinMessageAdapter.DeserialiseItemValue(message.Body.Value);

			m_currentState.SyncronisationItemUpdated(key, value);
		}

		public virtual void SyncronisationItemRemoved (MelvinMessage message)
		{
			object key = m_melvinMessageAdapter.DeserialiseItemKey(message.Body.Key);
			object value = m_melvinMessageAdapter.DeserialiseItemValue(message.Body.Value);

			m_currentState.SyncronisationItemRemoved(key, value);
		}

		public virtual void DisconnectionAcknowledgementReceived ()
		{
			m_currentState.DisconnectionAcknowledgementReceived();
		}
		
		#endregion MelvinServer Events

		#region Public Accessors

		public void Open ()
		{
			m_currentState.Connect();
		}

		public void Close ()
		{
			m_currentState.Disconnect();
		}

		public MelvinClientState CurrentState
		{
			get { return m_currentState.State; }
		}

		#endregion Public Accessors
	}
}
