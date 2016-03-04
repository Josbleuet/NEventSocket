﻿namespace NEventSocket.Examples
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac;

    using Net.Autofac.CommandLine;

    using NEventSocket.Logging;

    using Serilog;
    using Serilog.Debugging;
    using Serilog.Enrichers;
    using Serilog.Events;

    public class CommandLineTaskRunner : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;

        private IContainer container;

        private Task Run(CancellationToken cancellationToken)
        {
            return this.container.Resolve<DisplayCommandLineTasks>().Run(cancellationToken);
        }

        public int Run()
        {
            var builder = CreateContainerBuilder();

            SetupLogging(builder);

            this.container = builder.Build();

            var cancellationToken = this.CreateCancellationToken();

            Task.WaitAll(this.Run(cancellationToken));

            return 0;
        }

        private static ContainerBuilder CreateContainerBuilder()
        {
            var assembliesToScan = new[] { typeof(Program).Assembly, typeof(DisplayCommandLineTasks).Assembly };

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembliesToScan).AsImplementedInterfaces().AsSelf();

            return builder;
        }

        private static void SetupLogging(ContainerBuilder builder)
        {
            SelfLog.Out = Console.Out;

            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.ColoredConsole(LogEventLevel.Debug)
                .CreateLogger();

            builder.RegisterInstance(Log.Logger);
        }

        private CancellationToken CreateCancellationToken()
        {
            Console.CancelKeyPress += (sender, args) =>
            {
                this.cancellationTokenSource.Cancel();
                args.Cancel = true;
            };

            this.cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = this.cancellationTokenSource.Token;
            //cancellationToken.Register(() => container.Resolve<ILogger>().Warning("Canceling"));
            return cancellationToken;
        }

        public void Dispose()
        {
            try
            {
                this.container.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}