using System;

namespace SolutionForge.Mobile.Melvin
{
	internal enum MelvinServerState
	{
		Disconnected,
		Connected,
		Syncronising,
		Syncronised
	}

	/// <summary>
	/// Summary description for MelvinServerState.
	/// </summary>
	internal abstract class MelvinServerStateBase
	{
		private MelvinServer m_melvinServer;

		public MelvinServerStateBase (MelvinServer melvinServer)
		{
			m_melvinServer = melvinServer;
		}

		public abstract MelvinServerState State
		{
			get;
		}

		protected MelvinServer MelvinServer
		{
			get { return m_melvinServer; }
		}

		protected MelvinServerMessageFactory MelvinMessageFactory
		{
			get { return MelvinServer.MelvinMessageFactory; }
		}

		protected void TransitionState(MelvinServerState state)
		{
			m_melvinServer.TransitionState(state);
		}

		protected void SendMessage(MelvinMessage message)
		{
			m_melvinServer.SendMessage(message);
		}

		#region State Management

		public virtual void LeaveState ()
		{
		}

		public virtual void EnterState ()
		{
		}

		#endregion StateManagement

		#region Cache Events

		public virtual void ItemAdded (object key, object value)
		{
			// Do nothing. Overide in appropriate states
		}

		public virtual void ItemUpdated (object key, object update, object updatedItem)
		{
			// Do nothing. Overide in appropriate states
		}

		public virtual void ItemRemoved (object key, object value)
		{
			// Do nothing. Overide in appropriate states
		}

		#endregion Cache Events

		#region Melvin Client Events

		public virtual void Connect ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void ImageRequestStart ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void ImageRequestEnd ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void SyncronisationStart ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void SyncronisationEnd ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		public virtual void Disconnect ()
		{
			// Throw an exception. Override in appropriate states
			throw new ApplicationException("Invalid state for this operation");
		}

		#endregion Melvin Client Events
	}
}
