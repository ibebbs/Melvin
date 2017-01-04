using SolutionForge.Mobile.Melvin;
using System;

namespace Test.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinTestItemAdapter.
	/// </summary>
	public class MelvinTestItemAdapter : IMelvinMessageAdapter
	{
		#region IMelvinMessageAdapter Members

		public string SerialiseItemKey(object key)
		{
			return (string) key;
		}

		public object DeserialiseItemValue(string value)
		{
			return MelvinTestItem.Deserialise(value);
		}

		public object DeserialiseItemKey(string key)
		{
			return key;
		}

		public void PopulateRoutingParameters(object item, ref System.Collections.Specialized.ListDictionary routingParameters)
		{
			// TODO:  Add MelvinTestItemAdapter.PopulateRoutingParameters implementation
		}

		public string SerialiseItemValue(object value)
		{
			return MelvinTestItem.Serialise((MelvinTestItem) value);
		}

		#endregion
	}
}
