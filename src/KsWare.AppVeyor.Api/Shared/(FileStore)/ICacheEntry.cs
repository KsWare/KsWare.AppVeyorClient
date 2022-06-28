using System;
using System.IO;

namespace KsWare.AppVeyor.Api.Shared {

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

		new TData Data { get; set; }
	}

}
