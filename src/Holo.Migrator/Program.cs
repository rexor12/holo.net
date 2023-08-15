using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Holo.Migrator.Commands;
using Holo.Migrator.Commands.UpdateDatabase;
using Holo.Migrator.Enums;

namespace Holo.Migrator;

public sealed class Program
{
    private static readonly IReadOnlyDictionary<Type, Type> CommandsByOptions = new Dictionary<Type, Type>
    {
        { typeof(UpdateDatabaseOptions), typeof(UpdateDatabaseCommand) }
    };

    public static int Main(string[] args)
    {
        var parser = new Parser(c =>
        {
            c.AutoHelp = true;
            c.AutoVersion = true;
        });

        var resultCode = ResultCodes.Ok;
        var result = parser
            .ParseArguments(args, CommandsByOptions.Keys.ToArray())
            .WithParsed(options => resultCode = ExecuteCommand(options))
            .WithNotParsed(errors => resultCode = HandleParseError(errors));

        return (int)resultCode;
    }

    private static ResultCodes ExecuteCommand(object options)
    {
        if (!CommandsByOptions.TryGetValue(options.GetType(), out var commandType))
            throw new ArgumentException(
                $"There is no command associated to options type '{options.GetType()}'.",
                nameof(options));

        try
        {
            if (Activator.CreateInstance(commandType) is not ICommand command)
                throw new InvalidOperationException($"Failed to instantiate the command of type '{commandType}'.");

            command.ExecuteAsync(options).Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while executing the command:{Environment.NewLine}{e}");
            return ResultCodes.Error;
        }

        return ResultCodes.Ok;
    }

    private static ResultCodes HandleParseError(IEnumerable<Error> errors)
    {
        Console.WriteLine("Encountered one or more errors while parsing the input arguments");

        foreach (var error in errors)
        {
            Console.WriteLine(error.ToString());
        }

        return ResultCodes.Error;
    }
}