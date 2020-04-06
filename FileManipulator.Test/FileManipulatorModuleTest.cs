using System;
using Autofac;
using Autofac.Core;
using Messerli.CompositionRoot;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.Test.Utility;
using Xunit;

namespace Messerli.FileManipulator.Test
{
    public sealed class FileManipulatorModuleTest : IDisposable
    {
        private readonly IContainer _container = new CompositionRootBuilder()
            .RegisterModule<FileManipulatorModule>()
            .RegisterModule(new ModuleBuilder()
                .RegisterMock<DependencyAssets>()
                .Build())
            .RegisterModule(NotToBeRegisteredModule)
            .Build();

        private static readonly IModule NotToBeRegisteredModule =
            new ModuleBuilder()
                .RegisterMock<NugetPackageSourceManipulationAction>()
                .Build();

        [Theory]
        [TypesThatNeedToBeImplementedInAssemblyData("Messerli.FileManipulatorAbstractions")]
        public void TypesFromAssemblyCanBeResolved(Type type)
        {
            _container.Resolve(type);
        }

        [Fact]
        public void RegisteredTypesCanBeResolved()
        {
            Assert.All(_container.GetRegisteredTypes(), type => _container.Resolve(type));
        }

        public void Dispose() => _container.Dispose();
    }
}
