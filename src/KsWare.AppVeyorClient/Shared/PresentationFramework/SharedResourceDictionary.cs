using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KsWare.AppVeyorClient.Shared.PresentationFramework {

	public class SharedResourceDictionary : ResourceDictionary {

		private static readonly Dictionary<Uri, WeakReference> SharedCache = new Dictionary<Uri, WeakReference>();

		private Uri _sourceCore;

		public bool DisableCache { get; set; }

		public new Uri Source {
			get { return _sourceCore; }
			set {
				_sourceCore = value;
				if (!SharedCache.ContainsKey(_sourceCore) || DisableCache) {
					base.Source = _sourceCore;
					if (DisableCache) return;
					CacheSource();
				}
				else {
					ResourceDictionary target = (ResourceDictionary) SharedCache[_sourceCore].Target;
					if (target != null) {
						MergedDictionaries.Add(target);
						_sourceCore = target.Source;
					}
					else {
						base.Source = _sourceCore;
						CacheSource();
					}
				}
			}
		}

		private void CacheSource() {
			if (SharedCache.ContainsKey(_sourceCore)) SharedCache.Remove(_sourceCore);
			SharedCache.Add(_sourceCore, new WeakReference((object) this, false));
		}
	}
}