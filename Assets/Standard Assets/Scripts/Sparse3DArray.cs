using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sparse 3D array for storage of objects that exist in 3D space. The underlying storage object does not shrink when objects are removed from this sparse array, so this class is not well suited for arrays that frequently change size or may grow well beyond their average size.
/// </summary>
public class Sparse3DArray<T>
{
	/// <summary>
	/// The size of the underlying array. This can change as objects are added to the array. This differs from the logical size as some of the
	/// objects may be null placeholders.
	/// </summary>
	protected int physicalSize = 0;

	/// <summary>
	/// The number of empty spaces in the underlying array immediately after a resize. Making this larger will help prevent resizes when more objects are added, but will take up more space. Also, growing the array is kinda slow.
	/// </summary>
	protected static int bufferSize = 1;

	protected T[] contents;
	protected Dictionary<Index, int> mapper;

	protected T filler;

	private System.Object threadLock = new System.Object();

	public int Size	{ get; protected set; }
	
	public T this[Index logical]
	{
		get
		{
			return GetItem(logical);
		}
		set
		{
			Add(logical, value);
		}
	}
	
	public T this[int x, int y, int z]
	{
		get
		{
			return GetItem(new Index(x, y, z));
		}
		set
		{
			Add(new Index(x, y, z), value);
		}
	}
	
	public Sparse3DArray()
	{
		lock (threadLock)
		{
			contents = new T[bufferSize];
			mapper = new Dictionary<Index, int>();
			filler = default(T);
		}
	}
	
	protected bool GetPhysicalAddress(Index logical, out int physical)
	{
		lock (threadLock)
		{
			return mapper.TryGetValue (logical, out physical);
		}
	}

	public void SetDefault(T newDefault)
	{
		lock (threadLock)
		{
			filler = newDefault;
		}
	}
	
	public T GetItem(Index logical)
	{
		int physical;

		lock (threadLock)
		{
			if (GetPhysicalAddress(logical, out physical))
			{
				return contents[physical];
			}
			else
			{
				return filler;
			}
		}
	}
	
	/// <summary>
	/// Add the given item into the 3D array at the given address.
	/// </summary>
	/// <param name="newChunk">New item.</param>
	public void Add(Index logicalAddress, T newItem)
	{
		lock (threadLock)
		{
			// find an empty spot
			int physicalAddress = GetEmptyPhysicalAddress();
			
			AddAtPhysical(newItem, logicalAddress, physicalAddress);
		}
	}

	protected void AddAtPhysical(T newItem, Index logical, int physicalAddress)
	{
		lock (threadLock)
		{
			// store the chunk in given index
			contents[physicalAddress] = newItem;
			
			// map the chunk coords to the stored spot
			mapper.Add(logical, physicalAddress);

			Size++;
		}
	}

	/// <summary>
	/// Removes the first occurrence of the given item. This is slow as it has to search for the item, consider using RemoveAt(Index).
	/// </summary>
	/// <param name="item">Item to remove.</param>
	public bool Remove(T item)
	{
		lock (threadLock)
		{
			foreach (KeyValuePair<Index, int> mapEntry in mapper)
			{
				if (contents[mapEntry.Value].Equals(item))
				{
					contents[mapEntry.Value] = default(T); // consider removing
					mapper.Remove(mapEntry.Key);
					Size--;
					return true;
				}
			}
		}

		Debug.LogError(String.Format("Tried to remove non-existant object from Sparse3Darray: {0}", item.ToString()));
		return false;
	}

	/// <summary>
	/// Removes the item at the given Index.
	/// </summary>
	/// <param name="item">Item to remove.</param>
	public bool RemoveAt(Index logical)
	{
		int physicalAddress;

		lock (threadLock)
		{
			if (GetPhysicalAddress(logical, out physicalAddress))
			{
				contents[physicalAddress] = default(T);
				mapper.Remove(logical);
				
				Size--;
		
				return true;
			}
			else
			{
				// TODO deal with this
				return false;
			}
		}

	}
	
	protected int GetEmptyPhysicalAddress()
	{
		int emptyAddress;

		lock (threadLock)
		{
			for (int x = 0; x < physicalSize; x++)
			{
				if (contents[x] == null)
				{
					return x;
				}
			}
		
			// store this value as the index to return after we make some space
			emptyAddress = physicalSize;

			// make some space
			ResizeArray(physicalSize + bufferSize + 1);
		}

		return emptyAddress;
	}
	
	/// <summary>
	/// Gets all chunks in no particular order. Ths indeces of the returned array do not match up with the chunk coordinates.
	/// </summary>
	/// <returns>All chunks stored.</returns>
	public T[] ToArray()
	{
		lock (threadLock)
		{
			return contents;
		}
	}

	/// <summary>
	/// Resizes the array. Cannot shrink the array. Growing the array is slow, so use this if you know the desired size instead of having it grow after every add.
	/// </summary>
	/// <param name="newSize">New size.</param>
	public void ResizeArray(int newSize)
	{
		lock (threadLock)
		{
			//Debug.Log (String.Format("New Sparse3DArray size of {0}. Old size was {1}.", newSize, physicalSize));
			if (newSize > physicalSize)
			{
				// store old array
				T[] oldContents = contents;

				// create larger array
				contents = new T[newSize];
				physicalSize = newSize;

				// copy contents into new array
				for (int x = 0; x < oldContents.Length; x++)
				{
					contents[x] = oldContents[x];
				}
			}
			else
			{
				Debug.Log (String.Format("New Sparse3DArray size of {0} is invalid. Old size was {1}.", newSize, physicalSize));
			}
		}
	}
}
