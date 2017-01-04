using SolutionForge.Caching;
using Melvin;
using System;

namespace Test.Melvin
{
	/// <summary>
	/// Summary description for MelvinCompatibleOrderedHashtableCache.
	/// </summary>
	public class MelvinCompatibleOrderedHashtableCache : OrderedHashtableCache, IMelvinCache
	{
		public MelvinCompatibleOrderedHashtableCache(int lockTimeoutMs) : base(lockTimeoutMs) {}
	}
}
