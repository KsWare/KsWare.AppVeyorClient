using System;
using System.IO;

namespace KsWare.AppVeyorClient.Shared {

	public interface ICacheEntry {

		/// <inheritdoc cref="FileSystemInfo.CreationTime"/>
		DateTime CreationTime { get; set; }

		/// <inheritdoc cref="FileSystemInfo.LastWriteTime"/>
		DateTime LastWriteTime { get; set; }
		
		/// <inheritdoc cref="FileInfo.LastAccessTime"/>
		DateTime LastAccessTime { get; set; }

		/// <summary>
		/// Gets or sets the cache time.
		/// </summary>
		/// <value>The cache time.</value>
		TimeSpan CacheTime { get; set; }

		/// <summary>
		/// Gets the date until the data is valid.
		/// </summary>
		/// <value>The valid thru.</value>
		DateTime ValidThru { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has data.
		/// </summary>
		/// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
		bool HasData { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is usable.
		/// </summary>
		/// <value><c>true</c> if this instance is usable; otherwise, <c>false</c>.</value>
		bool IsUsable { get; }

		/// <summary>
		/// Invalidates this instance.
		/// </summary>
		void Invalidate();

		/// <summary>
		/// Gets or sets a value indicating whether this instance is persistent.
		/// </summary>
		/// <value><c>true</c> if this instance is persistent; otherwise, <c>false</c>.</value>
		bool IsPersistent { get; set; }

		/// <summary>
		/// Gets or sets the type of the data.
		/// </summary>
		/// <value>The type of the data.</value>
		string DataType { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
		object Data { get; set; }

	}

	public interface ICacheEntry<TData> : ICacheEntry {

		/// <inheritdoc/>
		new TData Data { get; set; }
	}

	public class CacheEntry : ICacheEntry {

		private object _data;
		private string _dataType;

		/// <inheritdoc/>
		public DateTime CreationTime { get; set; }

		/// <inheritdoc/>
		public DateTime LastWriteTime { get; set; }

		/// <inheritdoc/>
		public DateTime LastAccessTime { get; set; }

		/// <inheritdoc/>
		public TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(5); //TODO default value

		/// <inheritdoc/>
		public DateTime ValidThru => LastWriteTime.Add(CacheTime);

		public string DataType {
			get =>  (HasData && Data!=null ? Data.GetType() : typeof(void)).AssemblyQualifiedName;
			set => _dataType = value;
		}

		/// <inheritdoc/>
		public object Data {
			get => _data;
			set {
				// TODO revise NULL as valid data
				if (HasData) {
					LastWriteTime=DateTime.Now;
				}
				else {
					HasData = true;
					CreationTime=LastWriteTime = DateTime.Now;					
				}
				_data = value;
			}
		}

		/// <inheritdoc/>
		public bool HasData { get; set; }

		/// <inheritdoc/>
		public bool IsUsable => HasData && ValidThru > DateTime.Now;

		/// <inheritdoc/>
		public void Invalidate() {
			CreationTime = LastWriteTime = LastAccessTime = DateTime.MinValue;
			HasData = false;
			_data = null;
		}

		public bool IsPersistent { get; set; }
	}

	public class CacheEntry<TData> : CacheEntry, ICacheEntry<TData>, ICacheEntry {

		/// <inheritdoc/>
		public new TData Data {
			get => HasData ? (TData) base.Data : default(TData);
			set => base.Data=value;
		}

		/// <inheritdoc/>
		object ICacheEntry.Data {
			get => HasData ? base.Data : default(TData);
			set => base.Data = value;
		}
	}

	public class CacheEntryWrapper<T> : ICacheEntry<T> {
		ICacheEntry _entry;

		public CacheEntryWrapper(ICacheEntry entry) { _entry = entry ?? throw new ArgumentNullException(nameof(entry)); }

		/// <inheritdoc cref="ICacheEntry.CreationTime"/>
		public DateTime CreationTime { get => _entry.CreationTime; set => _entry.CreationTime = value; }

		/// <inheritdoc cref="ICacheEntry.LastWriteTime"/>
		public DateTime LastWriteTime { get => _entry.LastWriteTime; set => _entry.LastWriteTime = value; }

		/// <inheritdoc cref="ICacheEntry.LastAccessTime"/>
		public DateTime LastAccessTime { get => _entry.LastAccessTime; set => _entry.LastAccessTime = value; }

		/// <inheritdoc cref="ICacheEntry.CacheTime"/>
		public TimeSpan CacheTime { get => _entry.CacheTime; set => _entry.CacheTime = value; }

		/// <inheritdoc cref="ICacheEntry.ValidThru"/>
		public DateTime ValidThru => _entry.ValidThru;

		public string DataType { get => _entry.DataType; set => _entry.DataType = value; }

		/// <inheritdoc cref="ICacheEntry.Data"/>
		public T Data { get => HasData ? (T) _entry.Data : default(T); set => _entry.Data = value; }

		object ICacheEntry.Data { get => HasData ? _entry.Data : default(T); set => _entry.Data = (T) value; }

		/// <inheritdoc cref="ICacheEntry.HasData"/>
		public bool HasData { get => _entry.HasData; set => _entry.HasData = value; }

		/// <inheritdoc cref="ICacheEntry.IsUsable"/>
		public bool IsUsable => _entry.IsUsable;

		/// <inheritdoc cref="ICacheEntry.Invalidate"/>
		public void Invalidate() => _entry.Invalidate();

		/// <inheritdoc cref="ICacheEntry.IsPersistent"/>
		public bool IsPersistent { get => _entry.IsPersistent; set => _entry.IsPersistent = value; }
	}
}
