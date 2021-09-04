using System;

namespace KsWare.AppVeyor.Api.Shared {

	internal class CacheEntryWrapper<T> : ICacheEntry<T> {

		readonly ICacheEntry _entry;

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
