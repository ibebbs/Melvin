using SolutionForge.Mobile.Caching;
using SolutionForge.Mobile.Melvin;
using SolutionForge.Mobile.Melvin.Net;
using NUnit.Framework;
using System;
using System.Collections;
using System.Threading;

namespace Test.Mobile.Melvin.Net
{
	/// <summary>
	/// Summary description for MelvinNetTest.
	/// </summary>
	[TestFixture]
	public class MelvinNetTest
	{
		private const int CACHE_LOCKTIMEOUT = 1000;

		private const string SERVER_ADDRESS = "127.0.0.1";
		private const int SERVER_PORT = 7791;

		private static readonly DictionaryEntry[] TestItems = new DictionaryEntry[] 
			{
				new DictionaryEntry("Ian", new MelvinTestItem("Ian", 28)),
				new DictionaryEntry("Ben", new MelvinTestItem("Ben", 28)),
				new DictionaryEntry("Dave", new MelvinTestItem("Dave", 25)),
				new DictionaryEntry("Vince", new MelvinTestItem("Vince", 27))
			};

		private AutoResetEvent m_clientStateChangeEvent = new AutoResetEvent(false);

		private MelvinTestItemAdapter m_itemAdapter;

		private OrderedHashtableCache m_serverCache;
		private MelvinNetworkServer m_melvinServer;

		private OrderedHashtableCache m_clientCache;
		private MelvinNetworkClient m_melvinClient;

		[TestFixtureSetUp]
		public void Setup ()
		{
			m_itemAdapter = new MelvinTestItemAdapter();

			m_serverCache = new OrderedHashtableCache(CACHE_LOCKTIMEOUT);
			m_melvinServer = new MelvinNetworkServer(7791, m_serverCache, m_itemAdapter);

			m_clientCache = new OrderedHashtableCache(CACHE_LOCKTIMEOUT);
			m_melvinClient = new MelvinNetworkClient (SERVER_ADDRESS, SERVER_PORT,m_clientCache, m_itemAdapter);
			m_melvinClient.MelvinNetworkClientStateChanged += new EventHandler(melvinClient_MelvinNetworkClientStateChanged);
		}

		[TestFixtureTearDown]
		public void Teardown ()
		{
			m_melvinClient.MelvinNetworkClientStateChanged -= new EventHandler(melvinClient_MelvinNetworkClientStateChanged);
		}
		
		[Test]
		public void MelvinMainTest()
		{
			#region Setup

			foreach (DictionaryEntry entry in TestItems)
				m_serverCache.Add(entry.Key, entry.Value);
			
			m_melvinServer.Open();

			Thread.Sleep(500);

			m_melvinClient.Open();

			if ( !WaitForClientState(MelvinNetworkClientState.Connected) )
				Assert.Fail("Client did not reach the connected state");

			#endregion Setup

			#region Syncronisation

			if ( !WaitForClientState(MelvinNetworkClientState.Syncronised) )
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

			if ( !WaitForClientState(MelvinNetworkClientState.Disconnected) )
				Assert.Fail("Client did not reach the disconnected state");

			m_melvinServer.Close();

			#endregion Tear down
		}

		private bool WaitForClientState(MelvinNetworkClientState state)
		{
			while ( m_melvinClient.CurrentState != state )
				if ( !m_clientStateChangeEvent.WaitOne(10000, true) )
					return false;

			return true;
		}

		private void melvinClient_MelvinNetworkClientStateChanged(object sender, EventArgs e)
		{
			m_clientStateChangeEvent.Set();
		}
	}
}
