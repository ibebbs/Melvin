using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace SolutionForge.Mobile.Melvin
{
	public class MelvinMessageCategory
	{
		public const string Connection = "Connection";
		public const string ImageRequest = "ImageRequest";
		public const string Syncronisation = "Syncronisation";
		public const string Error	= "Error";
	}

	public class MelvinMessageOperation
	{
		public const string Begin = "Begin";
		public const string End = "End";
		public const string Add = "Add";
		public const string Update = "Update";
		public const string Remove = "Remove";
		public const string Error = "Error";
	}

	public struct MelvinMessageRoutingParameter
	{
		public string Name;

		public string Value;
	}

	public struct MelvinMessageBody
	{
		public string Key;

		public string Value;
	}

	public struct MelvinMessage
	{
		public string Category;

		public string Operation;

		public MelvinMessageRoutingParameter[] RoutingParameters;

		public MelvinMessageBody Body;
	}

	public class MelvinMessageSerialiser
	{
		private const string ROOT_ELEMENT = "MelvinMessage";
		private const string CATEGORY_ATTRIBUTE = "category";
		private const string OPERATION_ATTRIBUTE = "operation";
		private const string ROUTINGPARAMETERS_ELEMENT = "RoutingParameters";
		private const string ROUTINGPARAMETER_ELEMENT = "Param";
		private const string NAME_ATTRIBUTE = "name";
		private const string VALUE_ATTRIBUTE = "value";
		private const string BODY_ELEMENT = "MessageBody";
		private const string KEY_ATTRIBUTE = "key";
		private const string VALUE_ELEMENT = "Value";

		private static MelvinMessageRoutingParameter[] ReadRoutingParamters(XmlReader xmlReader)
		{
			ArrayList routingParameters = new ArrayList();

			while ( xmlReader.Read() && xmlReader.NodeType != XmlNodeType.EndElement )
				if ( xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == ROUTINGPARAMETER_ELEMENT )
				{
					MelvinMessageRoutingParameter routingParameter = new MelvinMessageRoutingParameter();

					routingParameter.Name = xmlReader.GetAttribute(NAME_ATTRIBUTE);
					routingParameter.Value = xmlReader.GetAttribute(VALUE_ATTRIBUTE);

					routingParameters.Add(routingParameter);
				}

			MelvinMessageRoutingParameter[] routingParameterArray = new MelvinMessageRoutingParameter[routingParameters.Count];
 
			routingParameters.CopyTo(routingParameterArray, 0);

			return routingParameterArray;
		}
		
		private static MelvinMessageBody ReadMessageBody(XmlReader xmlReader)
		{
			MelvinMessageBody messageBody = new MelvinMessageBody();
			messageBody.Key = xmlReader.GetAttribute(KEY_ATTRIBUTE);

			while ( xmlReader.Read() && xmlReader.NodeType != XmlNodeType.EndElement )
				if ( xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == VALUE_ELEMENT )
					messageBody.Value = xmlReader.ReadElementString();

			return messageBody;
		}
		
		public static MelvinMessage Deserialise(string message)
		{
			MelvinMessage newMessage = new MelvinMessage();
			StringReader stringReader = new StringReader(message);
			XmlTextReader xmlMessage = new XmlTextReader(stringReader);

			xmlMessage.MoveToContent();
			
			if ( xmlMessage.NodeType != XmlNodeType.Element || xmlMessage.Name != ROOT_ELEMENT )
				throw new ApplicationException("Invalid MelvinMessage");

			newMessage.Category = xmlMessage.GetAttribute(CATEGORY_ATTRIBUTE);
			newMessage.Operation = xmlMessage.GetAttribute(OPERATION_ATTRIBUTE);

			while ( xmlMessage.Read() )
				if ( xmlMessage.NodeType == XmlNodeType.Element )
					switch ( xmlMessage.Name )
					{
						case ROUTINGPARAMETERS_ELEMENT:
							newMessage.RoutingParameters = ReadRoutingParamters(xmlMessage);
							break;
						case BODY_ELEMENT:
							newMessage.Body = ReadMessageBody(xmlMessage);
							break;
					}

			return newMessage;
		}

		public static string Serialise (MelvinMessage message)
		{
			StringWriter stringWriter = new StringWriter();

			XmlTextWriter xmlMessage = new XmlTextWriter(stringWriter);
			xmlMessage.Formatting = Formatting.None;
			xmlMessage.WriteStartDocument(true);
			xmlMessage.WriteStartElement(ROOT_ELEMENT);
			xmlMessage.WriteAttributeString(CATEGORY_ATTRIBUTE, message.Category);
			xmlMessage.WriteAttributeString(OPERATION_ATTRIBUTE, message.Operation);
			
			if ( message.RoutingParameters != null && message.RoutingParameters.Length > 0 )
			{
				xmlMessage.WriteStartElement(ROUTINGPARAMETERS_ELEMENT);

				foreach (MelvinMessageRoutingParameter routingParameter in message.RoutingParameters )
				{
					xmlMessage.WriteStartElement(ROUTINGPARAMETER_ELEMENT);
					xmlMessage.WriteAttributeString(NAME_ATTRIBUTE, routingParameter.Name);
					xmlMessage.WriteAttributeString(VALUE_ATTRIBUTE, routingParameter.Value);
					xmlMessage.WriteEndElement();
				}

				xmlMessage.WriteEndElement();
			}

			if ( message.Body.Key != string.Empty && message.Body.Value != string.Empty )
			{
				xmlMessage.WriteStartElement(BODY_ELEMENT);
				xmlMessage.WriteAttributeString(KEY_ATTRIBUTE, message.Body.Key);
				xmlMessage.WriteElementString(VALUE_ELEMENT, message.Body.Value);
				xmlMessage.WriteEndElement();
			}

			xmlMessage.WriteEndElement();
			xmlMessage.WriteEndDocument();

			xmlMessage.Close();

			return stringWriter.ToString();
		}
	}
}
