namespace KsWare.AppVeyor.Api.Shared {

	public interface IFileStore {

		T GetValue<T>(string name, bool throwIfNotExists = false);

		ICacheEntry<T> GetEntry<T>(string name, bool throwIfNotExists=false);

		void SetValue<T>(string name, T value);

		void SetEntry<T>(string name, ICacheEntry<T> value);

		/// <summary>Writes all persistent entries to disc.</summary>
		void Flush();

		void Flush(string name);

		/// <summary>
		/// Deletes the entry with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		void Delete(string name);
	}

}
