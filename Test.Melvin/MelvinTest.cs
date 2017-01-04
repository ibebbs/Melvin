using SolutionForge.Mobile.Caching;
using SolutionForge.Mobile.Melvin;
using NUnit.Framework;
using System;
using System.Collections;
using System.Threading;

namespace Test.Mobile.Melvin
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class MelvinTest
	{
		private const int CACHE_LOCKTIMEOUT = 1000;

		private AutoResetEvent m_clientStateChangeEvent = new AutoResetEvent(false);

		private MelvinTestItemAdapter m_itemAdapter;

		private DirectMelvinMessageInterface m_serverInterface;
		private OrderedHashtableCache m_serverCache;
		private MelvinServer m_melvinServer;

		private DirectMelvinMessageInterface m_clientInterface;
		private OrderedHashtableCache m_clientCache;
		private MelvinClient m_melvinClient;

		private static readonly DictionaryEntry[] TestItems = new DictionaryEntry[] 
			{
				new DictionaryEntry("Ian", new MelvinTestItem("Ian", 28)),
				new DictionaryEntry("Ben", new MelvinTestItem("Ben", 28)),
				new DictionaryEntry("Dave", new MelvinTestItem("Dave", 25)),
				new DictionaryEntry("Vince", new MelvinTestItem("Vince", 27))
			};

		[TestFixtureSetUp]
		public void Setup ()
		{
			m_itemAdapter = new MelvinTestItemAdapter();

			m_serverInterface = new DirectMelvinMessageInterface();
			m_clientInterface = new DirectMelvinMessageInterface();

			m_serverInterface.DeliverTo = m_clientInterface;
			m_clientInterface.DeliverTo = m_serverInterface;

			m_serverInterface.EnableDelivery();
			m_clientInterface.EnableDelivery();

			m_serverCache = new OrderedHashtableCache(CACHE_LOCKTIMEOUT);
			m_melvinServer = new MelvinServer(m_serverCache, m_serverInterface, m_itemAdapter);

			m_clientCache = new OrderedHashtableCache(CACHE_LOCKTIMEOUT);
			m_melvinClient = new MelvinClient(m_clientCache, m_clientInterface, m_itemAdapter);
			m_melvinClient.MelvinClientStateChanged += new MelvinClientStateChangeHandler(melvinClient_MelvinClientStateChanged);
		}

		[Test]
		public void MelvinMainTest()
		{
			#region Setup

			foreach (DictionaryEntry entry in TestItems)
				m_serverCache.Add(entry.Key, entry.Value);

			m_melvinClient.Open();

			if ( !WaitForClientState(MelvinClientState.Connected) )
				Assert.Fail("Client did not reach the connected state");

			#endregion Setup

			#region Syncronisation

			if ( !WaitForClientState(MelvinClientState.Syncronised) )
				Assert.Fail("Client did not reach the syncronised state");

			Assert.AreEqual(m_serverCache.Count, m_clientCache.Count, "Client cache does not contain the same number of items as the server cache");

			foreach (DictionaryEntry entry in m_serverCache.Entries)
			{
				MelvinTestItem clientItem = null;

				clientItem = (MelvinTestItem) m_clientCache[entry.Key];

				MelvinTestItem serverItem = (MelvinTestItem) entry.Value;

				if ( clientItem == null )
					Assert.Fail("Client cache did not receive a server cache item");

				Assert.AreEqual(serverItem.Name, clientItem.Name, "Server and client items do not match");
				Assert.AreEqual(serverItem.Age, clientItem.Age, "Server and client items do not match");
			}

			#endregion Syncronisation

			#region Removal

			m_serverCache.Remove(TestItems[0].Key);

			Thread.Sleep(500);

			Assert.AreEqual(m_serverCache.Count, m_clientCache.Count, "TestItem was not removed from the client cache");

			#endregion Removal

			#region Addition

			m_serverCache.Add(TestItems[0].Key, TestItems[0].Value);

			Thread.Sleep(500);

			Assert.AreEqual(m_serverCache.Count, m_clientCache.Count, "TestItem was not added to the client cache");

			#endregion Addition

			#region Update

			MelvinTestItem update = (MelvinTestItem) TestItems[0].Value;
			MelvinTestItem updatedItem = MelvinTestItem.Deserialise(MelvinTestItem.Serialise(update));
			updatedItem.Age = 29;

			m_serverCache.Update(TestItems[0].Key, update, updatedItem);

			Thread.Sleep(500);

			MelvinTestItem clientUpdateItem = null;

			clientUpdateItem = (MelvinTestItem) m_clientCache[TestItems[0].Key];

			Assert.AreEqual(update.Age, clientUpdateItem.Age, "TestItem was not updated in the client cache");

			#endregion Update

			#region Tear down

			m_melvinClient.Close();

			if ( !WaitForClientState(MelvinClientState.Disconnected) )
				Assert.Fail("Client did not reach the disconnected state");

			#endregion Tear down
		}

		private bool WaitForClientState(MelvinClientState state)
		{
			while ( m_melvinClient.CurrentState != state )
                if ( !m_clientStateChangeEvent.WaitOne(10000, true) )
					return false;

			return true;
		}

		private void melvinClient_MelvinClientStateChanged(MelvinClient melvinClient, MelvinClientState state)
		{
			m_clientStateChangeEvent.Set();
		}
	}
}
