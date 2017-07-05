// Copyright (c) Werner Strydom. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Phaka.Scheduling.UnitTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Collections;
    using Xunit;
    using Xunit.Abstractions;

    public class SchedulerTests
    {
        FakeActivity t1 = new FakeActivity("t1"); 
        FakeActivity t2 = new FakeActivity("t2"); 
        FakeActivity t3 = new FakeActivity("t3"); 
        FakeActivity t4 = new FakeActivity("t4", TimeSpan.FromMilliseconds(50)); 
        FakeActivity t5 = new FakeActivity("t5");

        public SchedulerTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        private readonly ITestOutputHelper _output;

        [Fact]
        public async Task Schedule_Serial()
        {
            // Arrange
            var graph = new DependencyGraph<FakeActivity>();
            graph.AddDependency(t4, t5);
            graph.AddDependency(t3, t4);
            graph.AddDependency(t2, t3);
            graph.AddDependency(t1, t2);
            var expected = new[] {t1, t2, t3, t4, t5};

            var target = new Scheduler();
            var list = new BlockingCollection<FakeActivity>();

            // Act
            await target.ScheduleAsync(graph, async t =>
            {
                _output.WriteLine(t.ToString());
                list.Add(t);
                await t.ExecuteAsync();
            });

            var actual = list.ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Schedule_Graph()
        {
            // Arrange
            var graph = new DependencyGraph<FakeActivity>();
            graph.AddDependency(t4, t5);
            graph.AddDependency(t3, t5);
            graph.AddDependency(t1, t4);
            graph.AddDependency(t2, t3);
            graph.AddDependency(t1, t2);
            var expected = new[] {t1, t2, t4, t3, t5};

            var target = new Scheduler();
            var list = new BlockingCollection<FakeActivity>();

            // Act
            await target.ScheduleAsync(graph, async t =>
            {
                _output.WriteLine(t.ToString());
                list.Add(t);

                await t.ExecuteAsync();
            });

            var actual = list.ToArray();
            Assert.Equal(expected, actual);
        }
    }
}