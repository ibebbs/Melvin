using System;
using System.Collections;

namespace Mobile.Melvin
{
	/// <summary>
	/// Cache cleared event delegate
	/// </summary>
	/// <param name="cache">Cache that has just been cleared.</param>
	public delegate void CacheClearedHandler (IMelvinCache cache);

	/// <summary>
	/// Cache loaded event delegate
	/// </summary>
	/// <param name="cache">Cache that has just completed loading.</param>
	public delegate void CacheLoadCompletedHandler (IMelvinCache cache);

	/// <summary>
	/// Cache added event delegate
	/// </summary>
	/// <param name="cache">The cache the change has occurred in.</param>
	/// <param name="key">Key for the object in the cache that the change refers to.</param>
	/// <param name="item">Changed object.</param>
	public delegate void CacheItemAddedHandler (IMelvinCache cache, object key, object item);
	
	/// <summary>
	/// Cache updated event delegate
	/// </summary>
	/// <param name="cache">The cache the change has occurred in.</param>
	/// <param name="key">Key for the object in the cache that the change refers to.</param>
	/// <param name="update">Update object that resulted in the change.</param>
	/// <param name="updatedItem">Updated object.</param>
	public delegate void CacheItemUpdatedHandler (IMelvinCache cache, object key, object update, object updatedItem);

	/// <summary>
	/// Cache removed event delegate
	/// </summary>
	/// <param name="cache">The cache the change has occurred in.</param>
	/// <param name="key">Key for the object that was deleted.</param>
	/// <param name="item">Cached item that was deleted.</param>
	public delegate void CacheItemRemovedHandler (IMelvinCache cache, object key, object item);

	/// <summary>
	/// Summary description for IMelvinCacheBase.
	/// </summary>
	public interface IMelvinCache
	{
		void AcquireReaderLock ();

		void AcquireWriterLock ();

		void ReleaseReaderLock ();

		void ReleaseWriterLock ();

		bool IsReaderLockHeld
		{
			get;
		}

		bool IsWriterLockHeld
		{
			get;
		}

		/// <summary>
		/// Occurs after the cache has been cleared.
		/// </summary>
		event CacheClearedHandler Cleared;

		/// <summary>
		/// Occurs when a cache load has completed.
		/// </summary>
		event CacheLoadCompletedHandler LoadCompleted;

		/// <summary>
		/// Occurs when an item has been added to the cache.
		/// </summary>
		event CacheItemAddedHandler ItemAdded;

		/// <summary>
		/// Occurs just before an item is removed from the cache.
		/// </summary>
		event CacheItemRemovedHandler ItemRemoved;

		/// <summary>
		/// Occurs when an item within the cache has been updated.
		/// </summary>
		event CacheItemUpdatedHandler ItemUpdated;

		/// <summary>
		/// Removes all items from the cache.
		/// </summary>
		/// <remarks>Raises the <see cref="Cleared"/> event when the cache has been cleared.</remarks>
		/// <see cref="CacheClearedHandler"/>
		void Clear ();

		/// <summary>
		/// Instructs the cache that all subsequent Add calls are as a result of
		/// an initial load operation.  Use the <see cref="EndLoad"/> method
		/// to indicate that the load operation has been completed.
		/// </summary>
		void StartLoad ();

		/// <summary>
		/// Used to indicate that an initial cache load operation has completed.
		/// Causes the <see cref="LoadCompleted"/> event to fire.
		/// </summary>
		void EndLoad ();

		/// <summary>
		/// Adds an element with the specified key and value into the cache.
		/// </summary>
		/// <param name="key">Key to be used as a reference to the cache entry.</param>
		/// <param name="item">Item being cached.</param>
		/// <remarks>An exception is thrown if the item could not be added to the cache.
		/// Raises the <see cref="ItemAdded"/> event when the item has been added to the cache.</remarks>
		/// <see cref="CacheItemAddedHandler"/>
		void Add (object key, object item);

		/// <summary>
		/// Removes the element with the specified key from the cache.
		/// </summary>
		/// <param name="key">Key to be used as a reference to the cache entry.</param>
		/// <remarks>An exception is thrown if the item could not be removed from the cache.
		/// Raises the <see cref="ItemRemoved"/> event when the item has been added to the cache.</remarks>
		/// <see cref="CacheItemRemovedHandler"/>
		void Remove (object key);

		/// <summary>
		/// Updates a previously cached object.
		/// </summary>
		/// <param name="key">Key to be used as a reference to the cache entry.</param>
		/// <param name="update">Object used to update the entry.</param>
		/// <param name="updatedItem">Updated item.</param>
		/// <remarks>An exception is thrown if the item could not be found within the cache,
		/// or if the update could not be applied.
		/// Raises the <see cref="ItemUpdated"/> event when the item has been updated.</remarks>
		/// <see cref="CacheItemUpdatedHandler"/>
		void Update (object key, object update, object updatedItem);

		/// <summary>
		/// Gets the item associated with the specified key.
		/// </summary>
		/// <param name="key">Key to be used as a reference to the cache entry.</param>
		/// <returns>Cached item referenced by the specified key.</returns>
		object this [object key]
		{
			get;
		}

		/// <summary>
		/// Number of items in the cache.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Indicates whether the cache is currently being loaded.
		/// </summary>
		bool IsLoading
		{
			get;
		}		
		
		/// <summary>
		/// Returns an enumerator that can iterate over the cache.
		/// </summary>
		/// <returns>Cache enumerator.</returns>
		IEnumerable Entries
		{
			get;
		}
	}
}
