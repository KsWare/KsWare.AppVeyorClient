using System;

namespace KsWare.AppVeyor.Api.Shared {

	internal class CacheEntry : ICacheEntry {

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

	internal class CacheEntry<TData> : CacheEntry, ICacheEntry<TData>, ICacheEntry {

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

}
