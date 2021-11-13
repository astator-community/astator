using System;
using System.Threading.Tasks;

namespace astator.Core.Threading
{
    internal class ScriptTask
    {
        public async Task<TResult> Run<TResult>(Func<TResult> task)
        {
            return new Task<TResult>(task).Result;

        }
    }
}
