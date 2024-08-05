using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using KsWare.Presentation;

namespace KsWare.AppVeyorClient.Shared {

	public static class TaskExtensions {

		private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None,
			TaskContinuationOptions.None, TaskScheduler.Default);

		private static SynchronizationContext s_uiContext;
		
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

		public static void ContinueWithDispatcher<T>(this Task<T> task, Dispatcher dispatcher, Action<TaskInfo<T>> continuationAction) {
			task.ContinueWith(t => { dispatcher.BeginInvoke(continuationAction, new TaskInfo<T>(t)); });
		}

		public static void ContinueWithDispatcher(this Task task, Dispatcher dispatcher, Action<TaskInfo> continuationAction) {
			task.ContinueWith(t => { dispatcher.BeginInvoke(continuationAction, new TaskInfo(t)); });
		}

		public static void ContinueWithDispatcher<T>(this Task<T> task,
			Action<TaskInfo<T>> continuationAction) {
			task.ContinueWith(t => { Dispatcher.CurrentDispatcher.BeginInvoke(continuationAction, new TaskInfo<T>(t)); });
		}

		public static void ContinueWithDispatcher(this Task task, Action<TaskInfo> continuationAction) {
			task.ContinueWith(t => { Dispatcher.CurrentDispatcher.BeginInvoke(continuationAction, new TaskInfo(t)); });
		}

		public static void ContinueWithUIDispatcher<T>(this Task<T> task, Action<TaskInfo<T>> continuationAction) {
			task.ContinueWith(t => { ApplicationDispatcher.BeginInvoke(continuationAction, new TaskInfo<T>(t)); });
		}

		public static void ContinueWithUIDispatcher(this Task task, Action<TaskInfo> continuationAction) {
			task.ContinueWith(t => { ApplicationDispatcher.BeginInvoke(continuationAction, new TaskInfo(t)); });
		}

		// experimental
		public static void ContinueOnUIThread(this Task task, Action<TaskInfo> continuationAction) {
			if (UIContext == null) throw new InvalidOperationException("UI context not initialized. Call InitializeUIContext from the UI thread.");
			task.ContinueWith(t => {
				UIContext.Post(_ => continuationAction(new TaskInfo(t)), null);
			}, TaskScheduler.Default);
		}

		// experimental
		public static void ContinueOnUIThread<T>(this Task<T> task, Action<TaskInfo<T>> continuationAction) {
			if (UIContext == null) throw new InvalidOperationException("UI context not initialized. Call InitializeUIContext from the UI thread.");
			task.ContinueWith(t => {
				UIContext.Post(_ => continuationAction(new TaskInfo<T>(t)), null);
			}, TaskScheduler.Default);
		}

		public static SynchronizationContext UIContext {
			get {
				if (s_uiContext == null) {
					#pragma warning disable CS0618
					var dispatcher = ApplicationDispatcher.ThreadDispatcher;
					#pragma warning restore CS0618
					dispatcher.Invoke(() => s_uiContext = SynchronizationContext.Current);
				}
				return s_uiContext;
			}
			set => s_uiContext = value;
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

	public class TaskInfo {

		public TaskInfo(Task task) {
			IsCompleted = task.IsCompleted;
			IsCanceled = task.IsCanceled;
			IsFaulted = task.IsFaulted;

			if (task.IsFaulted) {
				Exception = task.Exception;
			}
			else if (!task.IsCanceled) {
				var taskType = task.GetType();
				if (taskType.IsGenericType) {
					var resultProperty = taskType.GetProperty("Result");
					Result = resultProperty?.GetValue(task);
				}
			}
		}

		public object Result { get; }
		public bool IsCompleted { get;}
		public bool IsCanceled { get;}
		public bool IsFaulted { get;}
		public Exception Exception { get;}

	}

	public class TaskInfo<T> : TaskInfo{

		public TaskInfo(Task<T> task) : base(task) { }
		public new T Result => (T)base.Result;
	}
}
