// Copyright (c) Werner Strydom. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Phaka.Scheduling
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;

    public class Scheduler : IScheduler
    {
        public async Task ScheduleAsync<T>(DependencyGraph<T> graph, Func<T, Task> activity)
        {
            var mapping = new ConcurrentDictionary<T, Status>();
            var running = new ConcurrentSet<Task>();

            do
            {
                var pendingNodes = graph.GetNodes()
                    .Where(arg => IsPending(arg, mapping));

                foreach (var node in pendingNodes)
                {
                    var canSchedule = graph.GetAntecedents(node)
                        .All(antecedent => IsComplete(antecedent, mapping));

                    if (!canSchedule)
                        continue;

                    mapping.AddOrUpdate(node, Status.Running, (arg1, status) => Status.Running);
                    var task = ExecuteAsync(node, mapping, activity);
                    running.Add(task);
                }

                await Task.WhenAny(running);

            } while (graph.GetNodes().Any(arg => IsPending(arg, mapping)));

            await Task.WhenAll(running);
        }

        private async Task ExecuteAsync<T>(T item, ConcurrentDictionary<T, Status> mapping,
            Func<T, Task> activity)
        {
            await activity(item);
            mapping.AddOrUpdate(item, Status.Completed, (arg1, status) => Status.Completed);
        }

        private bool IsComplete<T>(T item, ConcurrentDictionary<T, Status> mapping)
        {
            var status = mapping.GetOrAdd(item, Status.Pending);
            return status == Status.Completed;
        }

        private bool IsPending<T>(T item, ConcurrentDictionary<T, Status> mapping)
        {
            var status = mapping.GetOrAdd(item, Status.Pending);
            return status == Status.Pending;
        }

        private enum Status
        {
            Pending,
            Running,
            Completed
        }
    }
}