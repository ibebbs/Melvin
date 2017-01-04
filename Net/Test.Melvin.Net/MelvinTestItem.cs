using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Test.Mobile.Melvin
{
	/// <summary>
	/// Summary description for MelvinTestItem.
	/// </summary>
	[XmlRoot("MelvinTestItem")]
	public class MelvinTestItem
	{
		#region Static Serialisation

		private static XmlSerializer m_serializer = new XmlSerializer(typeof(MelvinTestItem));

		public static string Serialise(MelvinTestItem item)
		{
			StringWriter stringWriter = new StringWriter();
			m_serializer.Serialize(stringWriter, item);

			return stringWriter.ToString();
		}

		public static MelvinTestItem Deserialise(string item)
		{
			StringReader stringReader = new StringReader(item);
			return (MelvinTestItem) m_serializer.Deserialize(stringReader);
		}

		#endregion Static Serialisation

		private string m_name;
		private int m_age;

		/// <summary>
		/// Default constructor for serialisation
		/// </summary>
		public MelvinTestItem () {}

		/// <summary>
		/// Constructor for initialisation
		/// </summary>
		/// <param name="name"></param>
		/// <param name="age"></param>
		public MelvinTestItem (string name, int age)
		{
			m_name = name;
			m_age = age;
		}

		[XmlAttribute("name")]
		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		[XmlAttribute("age")]
		public int Age
		{
			get { return m_age; }
			set { m_age = value; }
		}
	}
}
