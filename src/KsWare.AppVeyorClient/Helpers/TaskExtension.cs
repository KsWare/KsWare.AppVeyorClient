using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KsWare.AppVeyorClient.Helpers {

	public static class TaskExtension {

		[DebuggerStepThrough, DebuggerHidden]
		public static T WaitForResult<T>(this Task<T> task) {
			// TODO optimize call
			var n = new ManualResetEventSlim(false);
			task.ContinueWith(t => { n.Set();});
			n.Wait();
			if (task.Exception != null) throw task.Exception;
			return task.Result;

/* System.InvalidOperationException:
   RunSynchronously kann nicht für eine Aufgabe aufgerufen werden, die nicht an einen Delegat gebunden ist, 
   wie beispielsweise eine Aufgabe, die von einer asynchronen Methode zurückgegeben wird.
   bei System.Threading.Tasks.Task.InternalRunSynchronously(TaskScheduler scheduler, Boolean waitForCompletion)
   bei System.Threading.Tasks.Task.RunSynchronously()*/
//			task.RunSynchronously();
//			task.Wait();
//			return task.Result;

		}

		public static T WaitForResult<T>(this Task<T> task, out Exception ex) {
			ex = null;
			// TODO optimize call
			var n = new ManualResetEventSlim(false);
			task.ContinueWith(t => { n.Set(); });
			n.Wait();
			if (task.Exception != null) {
				ex = task.Exception;
				return default(T);
			}
			return task.Result;
		}
	}
}
