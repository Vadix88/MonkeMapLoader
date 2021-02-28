using System;
using System.Collections;
using System.Threading.Tasks;

namespace VmodMonkeMapLoader.Helpers
{
    public static class AsyncExtensions
    {
        public static IEnumerator AsIEnumerator(this Task task, Action<Exception> error)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                error(task.Exception);

                yield break;
            }

            error(null);
        }
    }
}