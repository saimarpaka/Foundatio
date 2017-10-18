﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Jobs;
using Microsoft.Extensions.Logging;

namespace Foundatio.Tests.Jobs {
    public class HelloWorldJob : JobBase {
        private readonly string _id;

        public HelloWorldJob() : base(null) {
            _id = Guid.NewGuid().ToString("N").Substring(0, 10);
        }

        public static int GlobalRunCount;
        public int RunCount { get; set; }

        protected override Task<JobResult> RunInternalAsync(JobContext context) {
            RunCount++;
            Interlocked.Increment(ref GlobalRunCount);

            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace("HelloWorld Running: instance={Id} runs={RunCount} global={GlobalRunCount}", _id, RunCount, GlobalRunCount);

            return Task.FromResult(JobResult.Success);
        }
    }

    public class LongRunningJob : JobBase {
        private readonly string _id;
        private int _iterationCount;

        public LongRunningJob(ILoggerFactory loggerFactory) : base(loggerFactory) {
            _id = Guid.NewGuid().ToString("N").Substring(0, 10);
        }

        public int IterationCount => _iterationCount;

        protected override Task<JobResult> RunInternalAsync(JobContext context) {
            do {
                Interlocked.Increment(ref _iterationCount);
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                
                if (_iterationCount % 10000 == 0 && _logger.IsEnabled(LogLevel.Trace))
                    _logger.LogTrace("LongRunningJob Running: instance={Id} iterations={IterationCount}", _id, IterationCount);
            } while (true);

            return Task.FromResult(JobResult.Success);
        }
    }
}