using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BotManagementSystem.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("BotManagementSystem.Tests.SkippableFactDiscoverer", "BotManagementSystem.Tests")]
    public class SkippableFactAttribute : FactAttribute { }

    public class SkippableFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public SkippableFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield return new SkippableFactTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }
    }

    public class SkippableFactTestCase : XunitTestCase
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public SkippableFactTestCase() { }

        public SkippableFactTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[]? testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.None, testMethod, testMethodArguments) { }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var skipMessageBus = new SkippableMessageBus(messageBus);
            var result = await base.RunAsync(diagnosticMessageSink, skipMessageBus, constructorArguments, aggregator, cancellationTokenSource);
            if (skipMessageBus.SkippedTestCount > 0)
            {
                result.Failed -= skipMessageBus.SkippedTestCount;
                result.Skipped += skipMessageBus.SkippedTestCount;
            }
            return result;
        }
    }

    public class SkippableMessageBus : IMessageBus
    {
        private readonly IMessageBus _innerBus;
        public int SkippedTestCount { get; private set; }

        public SkippableMessageBus(IMessageBus innerBus)
        {
            _innerBus = innerBus;
        }

        public void Dispose() { }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            if (message is TestFailed failed && failed.ExceptionTypes.FirstOrDefault() == typeof(SkipException).FullName)
            {
                SkippedTestCount++;
                return _innerBus.QueueMessage(new TestSkipped(failed.Test, failed.Messages.FirstOrDefault() ?? "Test skipped"));
            }
            return _innerBus.QueueMessage(message);
        }
    }
}
