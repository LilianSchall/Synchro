using System;
using Synchro.Core;

namespace Synchro
{
    class Program
    {
        /// <summary>
        /// Programs entrypoint: Start the Bots process by launching the process Kernel.
        /// </summary>
        static void Main() => new ProcessKernel().Main().GetAwaiter().GetResult();

    }
}