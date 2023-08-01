using System.Threading.Tasks;

namespace Holo.Migrator.Commands;

public interface ICommand
{
    Task ExecuteAsync(object options);
}

public interface ICommand<TOptions> : ICommand
{
    Task OnExecuteAsync(TOptions options);
}