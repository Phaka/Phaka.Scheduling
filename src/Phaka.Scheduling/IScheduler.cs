// Copyright (c) Werner Strydom. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Phaka.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using Collections;

    /// <summary>
    /// Represents a scheduler
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        ///     Schedules an activity against each node
        /// </summary>
        /// <typeparam name="T">The node type</typeparam>
        /// <param name="graph">A dependency graph</param>
        /// <param name="activity">The activity</param>
        /// <returns>A task that can be awaited upon</returns>
        Task ScheduleAsync<T>(DependencyGraph<T> graph, Func<T, Task> activity);
    }
}