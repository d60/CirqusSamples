using System;
using System.Configuration;
using System.Threading;
using d60.Cirqus;
using d60.Cirqus.Config;
using d60.Cirqus.Logging;
using d60.Cirqus.MongoDb.Config;
using d60.Cirqus.MongoDb.Views;
using d60.Cirqus.MsSql.Config;
using d60.Cirqus.MsSql.Views;
using d60.Cirqus.Views;
using d60.Cirqus.Views.ViewManagers;
using ToDoomList.Commands;
using ToDoomList.Views;

namespace ToDoomList
{
    class Program
    {
        static void Main()
        {
            try
            {
                Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void Run()
        {
            var waitHandle = new ViewManagerWaitHandle();

            var processor = GetCommandProcessorForMongoDb(waitHandle);
            //var processor = GetCommandProcessorForMsSql(waitHandle);

            var listId = Guid.NewGuid();

            Print("Creating new to-doom list...");

            processor.ProcessCommand(new CreateToDoomList(listId,
                "Steps To World Domination",
                new[]
                {
                    "Create awesome framework",
                    "Release framework as FOSS",
                    "Make people addicted to framework",
                    "Figure out how to profit from giving away stuff for free"
                }));

            Print("Marking bullet 0 as complete...");

            processor.ProcessCommand(new MarkAsComplete(listId, 0));

            // just let async views do their thing
            Thread.Sleep(1000);

            Print("Marking bullet 1 as complete...");

            var lastResult = processor.ProcessCommand(new MarkAsComplete(listId, 1));

            Print("Waiting for events to be delivered to the slow view....");

            waitHandle.WaitForAll(lastResult, TimeSpan.FromSeconds(30)).Wait();
        }

        static void Print(string text)
        {
            var horizontalBar = new string('=', text.Length);

            Console.WriteLine(@"{0}
{1}
{2}", horizontalBar, text, horizontalBar);
        }

        static ICommandProcessor GetCommandProcessorForMongoDb(ViewManagerWaitHandle waitHandle)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["mongodb"];
            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException("Could not find 'mongodb' connection string in app.config!");
            }

            var mongoConnectionString = connectionStringSettings.ConnectionString;

            var viewManagers = new IViewManager[]
            {
                new MongoDbViewManager<ToDoomListView>(mongoConnectionString),
                new MongoDbViewManager<TimeToCompletionView>(mongoConnectionString)
            };

            return CommandProcessor.With()
                .Logging(l => l.UseConsole(minLevel: Logger.Level.Debug))
                .EventStore(e => e.UseMongoDb(mongoConnectionString, "Events"))
                .EventDispatcher(e =>
                {
                    e.UseViewManagerEventDispatcher(waitHandle, viewManagers);
                    e.UseViewManagerEventDispatcher(waitHandle, new MongoDbViewManager<SlowView>(mongoConnectionString));
                })
                .Create();
        }

        static ICommandProcessor GetCommandProcessorForMsSql(ViewManagerWaitHandle waitHandle)
        {
            var viewManagers = new IViewManager[]
            {
                new MsSqlViewManager<ToDoomListView>("mssql"),
                new MsSqlViewManager<TimeToCompletionView>("mssql")
            };

            return CommandProcessor.With()
                .Logging(l => l.UseConsole(minLevel: Logger.Level.Debug))
                .EventStore(e => e.UseSqlServer("mssql", "Events"))
                .EventDispatcher(e =>
                {
                    e.UseViewManagerEventDispatcher(waitHandle, viewManagers);
                    e.UseViewManagerEventDispatcher(waitHandle, new MsSqlViewManager<SlowView>("mssql"));
                })
                .Create();
        }
    }
}
