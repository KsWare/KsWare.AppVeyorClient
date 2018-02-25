using System;

namespace KsWare.AppVeyorClient.Shared {

	public interface ICacheEntry {
		DateTime Created { get; set; }
		object Data { get; set; }

		bool HasData { get; set; }
		bool IsUsable { get; }
		void Invalidate();
	}

	public interface ICacheEntry<TData> : ICacheEntry {
		new TData Data { get; set; }
	}

	public class CacheEntry : ICacheEntry {
		private object _data;
		private TimeSpan _cacheTime = TimeSpan.FromMinutes(5);
		public DateTime Created { get; set; }

		public object Data {
			get => _data;
			set {
				// TODO revise NULL as valid data
				_data = value;
				HasData = true;
				Created=DateTime.Now;
			}
		}
		public bool HasData { get; set; }
		public bool IsUsable => HasData && Created.Add(_cacheTime) > DateTime.Now;

		public void Invalidate() {
			Created=DateTime.MinValue;
		}
	}

	public class CacheEntry<TData> : CacheEntry, ICacheEntry<TData>, ICacheEntry {
		public new TData Data { get => (TData) base.Data; set => base.Data=value; }
		object ICacheEntry.Data { get => base.Data; set => base.Data = value; }
	}
}
