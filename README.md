# Phaka

## Synopsis

Phaka Scheduling facilitate optimally execution of activities.

## Code Examples

### Scheduling Entire Deployment 

Lets assume that a deployment consists of five activities, t1, t2, t3, t4 and t5. These activities could be anything, from creating Azure, Amazon AWS or VMware resources to running scripts. 

|Activity|Time|Antecedent|
|:---|:---|:---|
| t1 | 10ms..100ms | |
| t2 | 10ms..100ms |t1 |
| t3 | 10ms..100ms | t2 |
| t4 | 100ms..200ms | t1 |
| t5 | 10ms..100ms | t4, t3 |

If we assume an activity is defined as 

```csharp
public class Activity : IEquatable<Activity>
{
    public Activity(string name) : this(name, TimeSpan.FromMilliseconds(10))
    {
    }

    public Activity(string name, TimeSpan delay)
    {
        Name = name;
        Delay = delay;
    }

    public string Name { get; }

    public TimeSpan Delay { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        // Do something real here
        return Task.Delay(Delay, cancellationToken);
    }

    public override string ToString()
    {
        return Name;
    }

    public bool Equals(Activity other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Name, other.Name);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Activity) obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    public static bool operator ==(Activity left, Activity right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Activity left, Activity right)
    {
        return !Equals(left, right);
    }
}
```

And we created a number of activities

```csharp
Activity t1 = new Activity("t1"); 
Activity t2 = new Activity("t2"); 
Activity t3 = new Activity("t3"); 
Activity t4 = new Activity("t4", TimeSpan.FromMilliseconds(50)); 
Activity t5 = new Activity("t5");
```

And we configured some dependencies between them.

```csharp
var graph = new DependencyGraph<FakeActivity>();
graph.AddDependency(t4, t5);
graph.AddDependency(t3, t5);
graph.AddDependency(t1, t4);
graph.AddDependency(t2, t3);
graph.AddDependency(t1, t2);
```

```csharp
var target = new Scheduler();
await target.ScheduleAsync(graph, t => t.ExecuteAsync());
```

It will then execute the tasks in the correct order, e.g. t1, t2, t4, t3 and t5. 

## Installation

To use Phaka Scheduling in your project, add the nuget package to your project using Visual Studio

    Install-Package Phaka.Scheduling 

## Tests

Tests are implemented in xUnit

## Contributors

- Werner Strydom

    - Twitter: @bloudraak
    - Email: hello at wernerstrydom.com

## License

The MIT License(MIT)

Copyright(c) 2016 Werner Strydom

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
