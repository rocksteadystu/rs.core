using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace rs.core.cli;

public abstract class Feature
{
    private List<Action<IConfigurator<CommandSettings>>> _configs = new List<Action<IConfigurator<CommandSettings>>>();

    protected Feature(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public virtual void Setup(IServiceCollection services)
    {
    }

    public void Add(IConfigurator configurator)
    {
        configurator.AddBranch<CommandSettings>(Name, add =>
        {
            foreach (var config in _configs)
            {
                config(add);
            }
        });
    }

    protected void AddCommand<TCommand>(string name) where TCommand : class, ICommandLimiter<CommandSettings>
    {
        _configs.Add(x => x.AddCommand<TCommand>(name));
    }
}