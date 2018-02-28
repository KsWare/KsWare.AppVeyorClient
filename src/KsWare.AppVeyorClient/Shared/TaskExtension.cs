using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KsWare.AppVeyorClient.Shared {

	public static class TaskExtension {

		public static void ContinueWithDispatcher<T>(this Task<T> task, Dispatcher dispatcher, Action<Task<T>> continuationAction) {
			task.ContinueWith(task1 => { dispatcher.BeginInvoke(continuationAction, task1); });
		}
	}
}
