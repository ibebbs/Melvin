using System;

namespace SolutionForge.Mobile.Melvin
{
	public enum MelvinClientState
	{
		Disconnected,
		Connected,
		Syncronising,
		Syncronised,
		Disconnecting
	}

	/// <summary>
	/// Summary description for MelvinClientStateBase.
	/// </summary>
	internal abstract class MelvinClientStateBase
	{
		private MelvinClient m_melvinClient;

		public MelvinClientStateBase (MelvinClient melvinClient)
		{
			m_melvinClient = melvinClient;
		}

		public abstract MelvinClientState State
		{
			get;
		}

		protected MelvinClient MelvinClient
		{
			get { return m_melvinClient; }
		}

		protected MelvinClientMessageFactory MelvinMessageFactory
		{
			get { return MelvinClient.MelvinMessageFactory; }
		}

		protected void TransitionState(MelvinClientState state)
		{
			m_melvinClient.TransitionState(state);
		}

		protected void SendMessage(MelvinMessage message)
		{
			m_melvinClient.SendMessage(message);
		}

		#region State Management

		public virtual void LeaveState (MelvinClientState newState)
		{
		}

		public virtual void EnterState (MelvinClientState lastState)
		{
		}

		#endregion StateManagement

		#region State Methods

		public virtual void Connect ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void Disconnect ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		#endregion State Methods

		#region MelvinServer Events

		public virtual void ConnectionAcknowledgementReceived ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void ImageResposeStartRecieved ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void ImageResponseEndReceived ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void ImageResponseItemReceived (object key, object value)
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void SyncronisationItemAdded (object key, object value)
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void SyncronisationItemUpdated (object key, object value)
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void SyncronisationItemRemoved (object key, object value)
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void DisconnectionAcknowledgementReceived ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}
		
		#endregion MelvinServer Events
	}
}
