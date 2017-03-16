using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OneLauncher.Tests.Framework
{
    public class WaitUtils
    {
        public static void WaitFor(Func<bool> predicate, int timeoutInSeconds, int repeatPredicateTimingInSeconds = 1)
        {
            //if(repeatPredicateTiming>timeout)
            //    throw new InvalidOperationException("The repeatPredicateTiming should not be bigger than");

            for (int i = 0; i < timeoutInSeconds / repeatPredicateTimingInSeconds; i++)
            {
                if (predicate())
                    return;

                Thread.Sleep(repeatPredicateTimingInSeconds * 1000);
            }

            Assert.Fail("Condition was not met");
        }
    }
}
