using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using KsWare.Presentation;
using KsWare.Presentation.Core.Logging;

namespace KsWare.AppVeyorClient.Shared {

	public static class TaskExtensions {

		private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None,
			TaskContinuationOptions.None, TaskScheduler.Default);

		public static T RunSync<T>(Func<Task<T>> func) {
			var cultureUi = CultureInfo.CurrentUICulture;
			var culture   = CultureInfo.CurrentCulture;
			return MyTaskFactory.StartNew<Task<T>>(delegate {
				Thread.CurrentThread.CurrentCulture   = culture;
				Thread.CurrentThread.CurrentUICulture = cultureUi;
				return func();
			}).Unwrap<T>().GetAwaiter().GetResult();
		}
		// Helper.RunSync(new Func<Task<ReturnTypeGoesHere>>(async () => await AsyncCallGoesHere(myparameter)));

		public static RunSyncResult<T> RunSync2<T>(Func<Task<T>> func) {
			try {
				var cultureUi = CultureInfo.CurrentUICulture;
				var culture   = CultureInfo.CurrentCulture;
				var r = MyTaskFactory.StartNew<Task<T>>(delegate {
					Thread.CurrentThread.CurrentCulture   = culture;
					Thread.CurrentThread.CurrentUICulture = cultureUi;
					return func();
				}).Unwrap<T>().GetAwaiter().GetResult();
				return new RunSyncResult<T>(r);
			}
			catch (Exception ex) {
				return new RunSyncResult<T>(ex);
			}
		}

		public static T RunSync<T>(Func<Task<T>> func, out Exception exception) {
			exception = null;
			try {
				var cultureUi = CultureInfo.CurrentUICulture;
				var culture   = CultureInfo.CurrentCulture;
				return MyTaskFactory.StartNew<Task<T>>(delegate {
					Thread.CurrentThread.CurrentCulture   = culture;
					Thread.CurrentThread.CurrentUICulture = cultureUi;
					return func();
				}).Unwrap<T>().GetAwaiter().GetResult();
			}
			catch (Exception ex) {
				exception = ex;
				return default(T);
			}
		}

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

	public class RunSyncResult<T> {

		public RunSyncResult(T result) { Result = result; }

		public RunSyncResult(Exception exception) {
			Exception = exception ?? throw new ArgumentNullException(nameof(exception));
		}

		public T Result { get; }

		public Exception Exception { get; }
	}
}
