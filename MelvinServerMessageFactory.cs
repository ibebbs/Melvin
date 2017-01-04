using System;
using System.Collections;
using System.Collections.Specialized;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinMessageFactory.
	/// </summary>
	internal class MelvinServerMessageFactory
	{
		private IMelvinMessageAdapter m_messageAdapter;

		public MelvinServerMessageFactory (IMelvinMessageAdapter messageAdapter )
		{
			m_messageAdapter = messageAdapter;
		}

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

		public MelvinMessage CreateConnectionAcknowledgement ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Connection;
			message.Operation = MelvinMessageOperation.Begin;
			//message.RoutingParameters = RoutingParameters(value);
			//message.Body = MessageBody(key, value);

			return message;
		}

		public MelvinMessage CreateDisconnectionAcknowledgement ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Connection;
			message.Operation = MelvinMessageOperation.End;
			//message.RoutingParameters = RoutingParameters(value);
			//message.Body = MessageBody(key, value);

			return message;
		}

		public MelvinMessage CreateSyncronisedItemAddedMessage (object key, object value)
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Syncronisation;
			message.Operation = MelvinMessageOperation.Add;
			message.RoutingParameters = RoutingParameters(value);
			message.Body = MessageBody(key, value);

			return message;
		}

		public MelvinMessage CreateSyncronisedItemUpdatedMessage (object key, object update, object updatedItem)
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Syncronisation;
			message.Operation = MelvinMessageOperation.Update;
			message.RoutingParameters = RoutingParameters(update);
			message.Body = MessageBody(key, update);

			return message;
		}

		public MelvinMessage CreateSyncronisedItemRemovedMessage (object key, object value)
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Syncronisation;
			message.Operation = MelvinMessageOperation.Remove;
			message.RoutingParameters = RoutingParameters(value);
			message.Body = MessageBody(key, value);

			return message;
		}

		public MelvinMessage CreateImageResponseStartMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.ImageRequest;
			message.Operation = MelvinMessageOperation.Begin;
			//message.RoutingParameters = new MelvinMessageRoutingParameter[0];
			//message.Body = new MelvinMessageBody();

			return message;
		}

		public MelvinMessage CreateImageResponseItemMessage (object key, object value)
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.ImageRequest;
			message.Operation = MelvinMessageOperation.Add;
			message.RoutingParameters = RoutingParameters(value);
			message.Body = MessageBody(key, value);

			return message;
		}
	
		public MelvinMessage CreateImageResponseEndMessage ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.ImageRequest;
			message.Operation = MelvinMessageOperation.End;
			//message.RoutingParameters = RoutingParameters(update);
			//message.Body = MessageBody(key, update);

			return message;
		}

		public MelvinMessage CreateUnknownOperationMessage (MelvinMessage errorMessage)
		{
			MelvinMessage message = new MelvinMessage();

			return message;
		}

		public MelvinMessage CreateUnknownCategoryMessage (MelvinMessage errorMessage)
		{
			MelvinMessage message = new MelvinMessage();

			return message;
		}
	}
}
