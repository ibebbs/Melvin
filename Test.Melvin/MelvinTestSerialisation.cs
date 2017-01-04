using SolutionForge.Mobile.Melvin;
using NUnit.Framework;
using System;

namespace Test.Mobile.Melvin
{
	/// <summary>
	/// Summary description for TestMelvinSerialisation.
	/// </summary>
	[TestFixture]
	public class MelvinTestSerialisation
	{
		[Test]
		public void MelvinMessageSerialisation ()
		{
			MelvinMessage message = new MelvinMessage();
			message.Category = MelvinMessageCategory.Connection;
			message.Operation = MelvinMessageOperation.Add;

			MelvinMessageRoutingParameter param1 = new MelvinMessageRoutingParameter();
			param1.Name = "Param1Name";
			param1.Value = "Param1Value";
			
			MelvinMessageRoutingParameter param2 = new MelvinMessageRoutingParameter();
			param2.Name = "Param2Name";
			param2.Value = "Param2Value";

			message.RoutingParameters = new MelvinMessageRoutingParameter[] {param1, param2};

			MelvinMessageBody messageBody = new MelvinMessageBody();
			messageBody.Key = "MessageBodyKey";
			messageBody.Value = "<Body>Some data</Body>";

			message.Body = messageBody;

			string serialisedMessage = MelvinMessageSerialiser.Serialise(message);

			MelvinMessage deserialisedMessage = MelvinMessageSerialiser.Deserialise(serialisedMessage);

			Assert.AreEqual(message.Category, deserialisedMessage.Category, "Categories do not match");
			Assert.AreEqual(message.Operation, deserialisedMessage.Operation, "Operations do not match");

			Assert.AreEqual(message.RoutingParameters.Length, deserialisedMessage.RoutingParameters.Length, "Incorrect number of routing parameters");
			
			for (int count = 0; count < message.RoutingParameters.Length; count++ )
			{
				Assert.AreEqual(message.RoutingParameters[count].Name, deserialisedMessage.RoutingParameters[count].Name, "Routing Parameter Names do not match");
				Assert.AreEqual(message.RoutingParameters[count].Value, deserialisedMessage.RoutingParameters[count].Value, "Routing Parameter Values do not match");
			}

			Assert.AreEqual(message.Body.Key, deserialisedMessage.Body.Key, "Message Body Keys do not match");
			Assert.AreEqual(message.Body.Value, deserialisedMessage.Body.Value, "Message Body Values do not match");
		}
	}
}
