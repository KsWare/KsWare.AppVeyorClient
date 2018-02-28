using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using KsWare.Presentation;

namespace KsWare.AppVeyorClient.Shared {

	public static class TaskExtension {

		public static void ContinueWithDispatcher<T>(this Task<T> task, Dispatcher dispatcher, Action<Task<T>> continuationAction) {
			task.ContinueWith(task1 => { dispatcher.BeginInvoke(continuationAction, task1); });
		}

		public static void ContinueWithDispatcher(this Task task,
			Dispatcher dispatcher,
			Action<Task> continuationAction) {
			task.ContinueWith(task1 => { dispatcher.BeginInvoke(continuationAction, task1); });
		}

		public static void ContinueWithDispatcher<T>(this Task<T> task,
			Action<Task<T>> continuationAction) {
			task.ContinueWith(task1 => { Dispatcher.CurrentDispatcher.BeginInvoke(continuationAction, task1); });
		}

		public static void ContinueWithDispatcher(this Task task, Action<Task> continuationAction) {
			task.ContinueWith(task1 => { Dispatcher.CurrentDispatcher.BeginInvoke(continuationAction, task1); });
		}

		public static void ContinueWithUIDispatcher<T>(this Task<T> task, Action<Task<T>> continuationAction) {
			task.ContinueWith(task1 => { ApplicationDispatcher.BeginInvoke(continuationAction, task1); });
		}

		public static void ContinueWithUIDispatcher(this Task task, Action<Task> continuationAction) {
			task.ContinueWith(task1 => { ApplicationDispatcher.BeginInvoke(continuationAction, task1); });
		}
	}
}
