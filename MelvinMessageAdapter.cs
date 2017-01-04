using System;
using System.Collections;
using System.Collections.Specialized;

namespace SolutionForge.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinMessageSerialiser.
	/// </summary>
	public interface IMelvinMessageAdapter
	{
		string SerialiseItemKey(object key);

		string SerialiseItemValue(object value);

		object DeserialiseItemKey(string key);

		object DeserialiseItemValue(string value);

		void PopulateRoutingParameters(object item, ref ListDictionary routingParameters);
	}
}
