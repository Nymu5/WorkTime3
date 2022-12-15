using DynamicData;
using WorkTime3.Core;

namespace WorkTime3.Model;

public class Profile : ControllerBase
{
    public Profile()
    {
        _employers = new SourceCache<Employer, string>(e => e.Id);
        _times = new SourceCache<Time, string>(i => i.Id);
    }

    public Profile(SourceCache<Employer, string> employers, SourceCache<Time, string> times)
    {
        _employers = employers;
        _times = times;
    }

    private SourceCache<Employer, string> _employers;
    public SourceCache<Employer, string> Employers
    {
        get => _employers;
        set => SetProperty(ref _employers, value);
    }

    private SourceCache<Time, string> _times;
    public SourceCache<Time, string> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }
}