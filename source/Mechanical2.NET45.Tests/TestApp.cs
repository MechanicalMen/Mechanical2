using System;
using Mechanical.Bootstrap;
using Mechanical.Common;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.Tests
{
    public class TestApp : AppCore
    {
        private TestApp()
            : base()
        {
        }

        private static IMagicBag BuildMagicBag()
        {
            return new Mechanical.MagicBag.MagicBag.Inherit(
                AppCore.MagicBag,
                Map<IDateTimeProvider>.To(() => new TestDateTimeProvider()).AsThreadLocal()); // thread local should allow testing in parallel
        }

        static TestApp()
        {
            // exceptions sources
            AppCore.Register(new AppDomainExceptionSource());
            AppCore.Register(new UnobservedTaskExceptionSource());

            // exception sinks
            var logSink = new LogExceptionSink();
            AppCore.Register(logSink, isFallback: false);
            AppCore.Register(logSink, isFallback: true);
            AppCore.Register(new TraceExceptionSink(), isFallback: true);

            // magic bag
            AppCore.MagicBag = BuildMagicBag();

            // UI
            AppCore.Register((IUIThreadHandler)null);
        }

        public static void Initialize()
        {
        }
    }
}
