using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Messerli.CommandLine;
using Messerli.CommandLineAbstractions;
using Messerli.NativeProjects;
using Messerli.OneCoreProjects;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;
using Messerli.ProjectGenerator.UserInput;
using Messerli.TfsClient;
using Stubble.Core.Builders;
using UserInputDescriptionBuilder = Messerli.ProjectGenerator.UserInput.UserInputDescriptionBuilder;

namespace Messerli.ProjectGenerator
{
    internal class CompositionRoot
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<SystemConsoleWriter>().As<IConsoleWriter>();
            builder.RegisterType<SystemConsoleReader>().As<IConsoleReader>();
            builder.RegisterType<UserInputProvider>().As<IUserInputProvider>().SingleInstance();
            builder.RegisterType<UserInputDescriptionBuilder>().AsSelf();
            builder.RegisterType<TemplateLoader>().As<ITemplateLoader>();

            builder.RegisterType<ConsoleClient>().As<IClient>();
            builder.RegisterType<FileGenerator>().As<IFileGenerator>();
            builder.RegisterType<StubbleBuilder>().AsSelf();

            builder.RegisterModule<NativeProjectsModule>();
            builder.RegisterModule<OneCoreProjectsModule>();

            builder.RegisterType<ExecutingPluginAssemblyProvider>().As<IExecutingPluginAssemblyProvider>().SingleInstance();

            builder.RegisterType<StringRequester>();
            builder.RegisterType<BoolRequester>();
            builder.RegisterType<SelectionRequester>();
            builder.Register(VariableRequesterFactory).As<IVariableRequester>();

            return builder.Build();
        }

        private static IVariableRequester VariableRequesterFactory(IComponentContext context, IEnumerable<Parameter> paremeter)
        {
            var variableType = paremeter.TypedAs<VariableType>();

            return variableType switch
            {
                VariableType.String => context.Resolve<StringRequester>(),
                VariableType.Bool => context.Resolve<BoolRequester>(),
                VariableType.Integer => context.Resolve<IntegerRequester>(),
                VariableType.Double => context.Resolve<DoubleRequester>(),
                VariableType.Selection => context.Resolve<SelectionRequester>(),
                VariableType.Path => context.Resolve<PathRequester>(),
                VariableType.ExistingPath => context.Resolve<ExistingPathRequester>(),
                VariableType.Date => context.Resolve<DateRequester>(),
                VariableType.DateTime => context.Resolve<DateTimeRequester>(),
                VariableType.Time => context.Resolve<TimeRequester>(),

                _ => throw new NotImplementedException($"The VariableType '{variableType}' is not supported at the moment..."),
            };
        }
    }
}