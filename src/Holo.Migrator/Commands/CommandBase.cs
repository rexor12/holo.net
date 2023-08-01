using System;
using System.Threading.Tasks;

namespace Holo.Migrator.Commands;

public abstract class CommandBase<TOptions> : ICommand<TOptions>
{
    public Task ExecuteAsync(object options)
    {
        if (options is not TOptions typedOptions)
            throw new ArgumentException(
                $"Invalid options type '{options.GetType()}'. Expected: {typeof(TOptions)}",
                nameof(options));

        return OnExecuteAsync(typedOptions);
    }

    public abstract Task OnExecuteAsync(TOptions options);
}
