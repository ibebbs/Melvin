using System;
using System.Collections;
using System.Collections.Specialized;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinClientMessageFactory.
	/// </summary>
	public class MelvinClientMessageFactory
	{
		private IMelvinMessageAdapter m_messageAdapter;

		private MelvinMessageRoutingParameter[] RoutingParameters(object value)
		{
			ListDictionary routingParameters = new ListDictionary();
			m_messageAdapter.PopulateRoutingParameters(value, ref routingParameters);
			
			MelvinMessageRoutingParameter[] routingParametersArray = new MelvinMessageRoutingParameter[routingParameters.Count];
			
			int count = 0;

			foreach (DictionaryEntry entry in routingParameters)
			{
				MelvinMessageRoutingParameter routingParameter = new MelvinMessageRoutingParameter();
				routingParameter.Name = (string) entry.Key;
				routingParameter.Value = (string) entry.Value;

				routingParametersArray[count++] = routingParameter;
			}

			return routingParametersArray;
		}

		private MelvinMessageBody MessageBody(object key, object value)
		{
			MelvinMessageBody messageBody = new MelvinMessageBody();

			string serialisedKey = m_messageAdapter.SerialiseItemKey(key);
			string serialisedValue = m_messageAdapter.SerialiseItemValue(value);

			messageBody.Key = serialisedKey;
			messageBody.Value = serialisedValue;

			return messageBody;
		}

		public MelvinClientMessageFactory (IMelvinMessageAdapter messageAdapter )
		{
			m_messageAdapter = messageAdapter;
		}
		
		internal MelvinMessage CreateConnectionMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Connection;
			message.Operation = MelvinMessageOperation.Begin;

			return message;
		}

		internal MelvinMessage CreateDisconnectionMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Connection;
			message.Operation = MelvinMessageOperation.End;

			return message;
		}

		internal MelvinMessage CreateImageRequestStartMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.ImageRequest;
			message.Operation = MelvinMessageOperation.Begin;

			return message;
		}

		internal MelvinMessage CreateImageRequestEndMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.ImageRequest;
			message.Operation = MelvinMessageOperation.End;

			return message;
		}

		internal MelvinMessage CreateSyncronisationBeginMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Syncronisation;
			message.Operation = MelvinMessageOperation.Begin;

			return message;
		}

		internal MelvinMessage CreateSyncronisationEndMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Syncronisation;
			message.Operation = MelvinMessageOperation.End;

			return message;
		}

		internal MelvinMessage CreateUnknownOperationMessage (MelvinMessage errorMessage)
		{
			MelvinMessage message = new MelvinMessage();

			return message;
		}

		internal MelvinMessage CreateUnknownCategoryMessage (MelvinMessage errorMessage)
		{
			MelvinMessage message = new MelvinMessage();

			return message;
		}
	}
}
