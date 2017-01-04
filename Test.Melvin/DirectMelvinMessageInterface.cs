using SolutionForge.Mobile.Melvin;
using System;
using System.Collections;
using System.Threading;

namespace Test.Mobile.Melvin
{
	/// <summary>
	/// Summary description for DirectMelvinMessageInterface.
	/// </summary>
	public class DirectMelvinMessageInterface : IMelvinMessageInterface
	{
		private DirectMelvinMessageInterface m_destinationMessageInterface;
		private AutoResetEvent m_messageAvailable = new AutoResetEvent(false);
		private Queue m_deliveryQueue = new Queue();
		private Thread m_deliveryThread;
		private bool m_enabled = false;

		private void DeliveryThreadEntryPoint ()
		{
			while (m_enabled)
			{
				while (m_deliveryQueue.Count > 0)
				{
					string message = (string) m_deliveryQueue.Dequeue();

					m_destinationMessageInterface.InboundMessage(message);
				}

				m_messageAvailable.WaitOne();
			}
		}

		#region IMelvinMessageInterface Members

		public event ReceiveMessageHandler ReceiveMessage;

		void IMelvinMessageInterface.SendMessage(string message)
		{
			lock(this)
			{
				m_deliveryQueue.Enqueue(message);
				m_messageAvailable.Set();
			}
		}

		#endregion

		public void InboundMessage (string message)
		{
			if ( ReceiveMessage != null )
				ReceiveMessage(message);
		}

		public DirectMelvinMessageInterface DeliverTo
		{
			get { return m_destinationMessageInterface; }
			set
			{
				if ( m_enabled )
					throw new ApplicationException("Cannot change DeliverTo interface until delivery has been disabled");

				m_destinationMessageInterface = value;
			}
		}

		public void EnableDelivery ()
		{
			if ( m_destinationMessageInterface == null )
				throw new ApplicationException("Cannot enable delivery until a DeliverTo interface has been set");

			m_enabled = true;
			m_deliveryThread = new Thread(new ThreadStart(DeliveryThreadEntryPoint));
			m_deliveryThread.Start();
		}

		public void DisableDelivery ()
		{
			if ( m_enabled )
			{
				m_enabled = false;
				m_messageAvailable.Set();
			}
		}
	}
}
