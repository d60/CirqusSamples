using System;
using System.Configuration;
using System.Threading;
using d60.Cirqus;
using d60.Cirqus.Aggregates;
using d60.Cirqus.Logging;
using d60.Cirqus.Logging.Console;
using d60.Cirqus.MongoDb.Events;
using d60.Cirqus.MongoDb.Views;
using d60.Cirqus.MsSql.Events;
using d60.Cirqus.MsSql.Views;
using d60.Cirqus.Views;
using d60.Cirqus.Views.ViewManagers;
using MongoDB.Driver;
using ToDoomList.Commands;
using ToDoomList.Views;

namespace ToDoomList
{
    class Program
    {
        static void Main()
        {
            //var processor = GetCommandProcessorForMongoDb();
            var processor = GetCommandProcessorForMsSql();

            processor.Initialize();

            var listId = Guid.NewGuid();

            processor.ProcessCommand(new CreateToDoomList(listId,
                "Steps To World Domination",
                new[]
                {
                    "Create awesome framework",
                    "Release framework as FOSS",
                    "Make people addicted to framework",
                    "Figure out how to profit from giving away stuff for free"
                }));

            processor.ProcessCommand(new MarkAsComplete(listId, 0));

            Thread.Sleep(1000);

            processor.ProcessCommand(new MarkAsComplete(listId, 1));

            Console.WriteLine("Waiting for events to be delivered to the slow view....");
            Thread.Sleep(10000);
        }

        static CommandProcessor GetCommandProcessorForMongoDb()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["mongodb"];
            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException("Could not find 'mongodb' connection string in app.config!");
            }
            var mongoUrl = new MongoUrl(connectionStringSettings.ConnectionString);
            var database = new MongoClient(mongoUrl).GetServer()
                .GetDatabase(mongoUrl.DatabaseName ?? "cirqus_samples_todoomlist");

            database.Drop();

            var eventStore = new MongoDbEventStore(database, "Events");
            var aggregateRootRepository = new DefaultAggregateRootRepository(eventStore);

            var syncEventDispatcher = new ViewManagerEventDispatcher(aggregateRootRepository,
                new MongoDbViewManager<ToDoomListView>(database, "ToDoomListView"),
                new MongoDbViewManager<TimeToCompletionView>(database, "TimeToCompletionView"));

            var asyncEventDispatcher = new ViewManagerEventDispatcher(aggregateRootRepository,
                new MongoDbViewManager<SlowView>(database, "SlowViews"))
                .Asynchronous();

            var eventDispatcher = new CompositeEventDispatcher(syncEventDispatcher, asyncEventDispatcher);

            var processor = new CommandProcessor(eventStore, aggregateRootRepository, eventDispatcher)
            {
                Options =
                {
                    GlobalLoggerFactory = new ConsoleLoggerFactory(minLevel: Logger.Level.Debug),
                }
            };
            return processor;
        }

        static CommandProcessor GetCommandProcessorForMsSql()
        {
            var eventStore = new MsSqlEventStore("mssql", "Events");
            var aggregateRootRepository = new DefaultAggregateRootRepository(eventStore);

            var syncEventDispatcher = new ViewManagerEventDispatcher(aggregateRootRepository,
                new MsSqlViewManager<ToDoomListView>("mssql", "ToDoomListView"),
                new MsSqlViewManager<TimeToCompletionViewAdaptedForMsSql>("mssql", "TimeToCompletionView"));

            var asyncEventDispatcher = new ViewManagerEventDispatcher(aggregateRootRepository,
                new MsSqlViewManager<SlowView>("mssql", "SlowViews"))
                .Asynchronous();

            var eventDispatcher = new CompositeEventDispatcher(syncEventDispatcher, asyncEventDispatcher);

            var processor = new CommandProcessor(eventStore, aggregateRootRepository, eventDispatcher)
            {
                Options =
                {
                    GlobalLoggerFactory = new ConsoleLoggerFactory(minLevel: Logger.Level.Debug),
                }
            };
            return processor;
        }
    }
}
