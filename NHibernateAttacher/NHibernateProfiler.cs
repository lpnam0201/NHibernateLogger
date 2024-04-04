using NHibernateProfilerLibrary.Util;
using System.Data;
using System.Reflection;

namespace NHibernateProfilerLibrary
{
    public static class NHibernateProfiler
    {
        public static void Initialize()
        {
            RegisterProfiler();
            NHibernateProfilerStateManager.IsEnabled = true;
        }

        private static void RegisterProfiler()
        {
            var embeddedSources = new List<string>
            {
                "NHibernateLoggerWithPublishEvent.cs",
                "NHibernateLoggerWithPublishEventFactory.cs",
                "LoggerFactorySetup.cs",
            };

            var nhibernateAssembly = GetAssembly("NHibernate");

            var referenceAssemblies = new List<string>
            {
                typeof(NHibernateProfiler).Assembly.Location,
                nhibernateAssembly.Location,
                typeof(IDbConnection).Assembly.Location
            };

            var compiled = GenerateAssembly.Compile("NHibernateProfilerLibrary.EmbeddedSources", embeddedSources, referenceAssemblies);
            var loggerFactorySetupType = compiled.GetType("NHibernateProfilerLibrary.LoggerFactorySetup");
            loggerFactorySetupType
                .GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod)
                .Invoke(null, null);
        }

        private static Assembly GetAssembly(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault((Assembly asm) => asm.GetName().Name == name);
        }
    }
}