using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OKX.UI.Extend
{
    public static class ExecuteUIThread
    {
        private static Action<System.Action> executor = action => action();

        /// <summary>
        /// Initializes the framework using the current dispatcher.
        /// </summary>
        public static void InitializeWithDispatcher()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            executor = action =>
            {
                if (dispatcher.CheckAccess())
                    action();
                else dispatcher.Invoke(action);
            };
        }

        /// <summary>
        /// Resets the executor to use a non-dispatcher-based action executor.
        /// </summary>
        public static void ResetWithoutDispatcher()
        {
            executor = action => action();
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void OnUIThread(this System.Action action)
        {
            executor(action);
        }
    }
}
