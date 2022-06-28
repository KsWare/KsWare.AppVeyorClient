using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace KsWare.AppVeyor.Api.Shared {

	internal class FileStore : IFileStore {

		private readonly string _baseFolder;

		public static FileStore Instance { get; set; }

		private readonly Dictionary<string,ICacheEntry> _cache=new Dictionary<string, ICacheEntry>(StringComparer.OrdinalIgnoreCase);

		public FileStore(string baseFolder) {
			_baseFolder = baseFolder;
			if (Directory.Exists(baseFolder)) {
				var folderNames = Directory.GetDirectories(_baseFolder);
				var fileNames   = Directory.GetFiles(_baseFolder);
				if (folderNames.Length == 0 && fileNames.Length == 0) {
					// empty folder found, create new store
					Save(".filestore", "{\"@version\":\"1.0\"}");
				}
				else if (File.Exists(Path.Combine(_baseFolder, ".filestore"))) {
					// ok use the existing store
				}
				else {
					throw new ArgumentException("The specified directory is not empty and not a file store.");
				}
			}
			else {
				Directory.CreateDirectory(_baseFolder);
				Save(".filestore", "{\"@version\":\"1.0\"}");
			}
		}

		public bool EncryptNames { get; set; }

		public bool EncryptContent { get; set; }

		public T GetValue<T>(string name, bool throwIfNotExists = false) {
			return GetEntry<T>(name, throwIfNotExists).Data;
		}

		public ICacheEntry<T> GetEntry<T>(string name, bool throwIfNotExists = false) {
			if (!_cache.TryGetValue(name, out var entry)) {
				var fn = Path.Combine(_baseFolder, name);
				entry = File.Exists(fn) ? LoadEntry<T>(name) : new CacheEntry<T>();
				_cache.Add(name, entry);
			}
			var entryT = (entry as ICacheEntry<T>) ?? new CacheEntryWrapper<T>(entry); //TODO maybe clone to new CacheEntry<T>
			return entryT;
		}

		public void SetValue<T>(string name, T value) {
			var entry = GetEntry<T>(name);
			entry.Data = value;
			SaveEntry(name,entry);
		}

		public void SetEntry<T>(string name, ICacheEntry<T> entry) {
			if (_cache.ContainsKey(name)) _cache[name] = entry;
			else _cache.Add(name, entry);
			SaveEntry(name, entry);
		}

		private void Save(string name, string contents) {
			var fn = Path.Combine(_baseFolder, name);
			File.WriteAllText(fn, contents);
		}

		private void SaveEntry(string name, ICacheEntry entry) { Save(name, JsonConvert.SerializeObject(entry)); }

		private string Load(string name) {
			var fn = Path.Combine(_baseFolder, name);
			return File.ReadAllText(fn);
		}

		private CacheEntry<T> LoadEntry<T>(string name) { return JsonConvert.DeserializeObject<CacheEntry<T>>(Load(name)); }

		private void EncryptAndSave(string name, object o) {
			if (o == null) throw new ArgumentNullException(nameof(o));
			var fn = Path.Combine(_baseFolder, name);

			var plaintext=Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o));

			byte[] entropy = new byte[20];
			using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())rng.GetBytes(entropy);

			byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

			int chk = 0;
			foreach (var b in ciphertext)chk += b;
			Debug.WriteLine(chk);

			using (var isoStream = File.Create(fn)) {

				using (var writer = new BinaryWriter(isoStream)) {
					writer.Write(entropy);
					writer.Write(ciphertext.Length);
					writer.Write(ciphertext);
					writer.Flush();
				}
			}
		}

		private T LoadAndDecrypt<T>(string name) {
			var fn = Path.Combine(_baseFolder, name);

			byte[] entropy;
			byte[] ciphertext;
			using (var isoStream = File.OpenRead(fn)) {
				using (var reader = new BinaryReader(isoStream)) {
					entropy    = reader.ReadBytes(20);
					ciphertext = reader.ReadBytes(reader.ReadInt32());
				}
			}

			byte[] plaintext;
			try {
				plaintext = ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser);
			}
			catch (Exception ex) {
				throw new SecurityException("Decrypt failed",ex); //TODO message.
			}
			return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(plaintext));
		}

		// Löschen aller Einträge (TRUNCATE TABLE) im RAM / auf der Festplatte
		// Löschen des kompletten FileStore (DROP TABLE)

		/// <summary>Writes all persistent entries to disc.</summary>
		public void Flush() {
			foreach (var v in _cache) Flush(v.Key,v.Value);
		}

		private void Flush(string name, ICacheEntry value) {
			if (!value.IsPersistent || !value.IsUsable)
				DeleteInternal(name);
			else
				SaveEntry(name, value);
		}

		public void Flush(string name) {
			if(!_cache.TryGetValue(name,out var entry )) return; // no exception if not exists
			SaveEntry(name, entry);
		}

		/// <summary>
		/// Deletes the entry with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Delete(string name) { DeleteInternal(name); }

		public void DeleteInternal(string name) {
			var fn = Path.Combine(_baseFolder, name);
			if (File.Exists(fn)) File.Delete(fn);
		}
	}

}
