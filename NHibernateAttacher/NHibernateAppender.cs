using NHibernateAppender.Util;
using System.Data;
using System.Reflection;

namespace NHibernateAppender
{
    public static class NHibernateAppender
    {
        public static void Initialize()
        {
            RegisterAppender();
        }

        private static void RegisterAppender()
        {
            var embeddedSources = new List<string>
            {
                "NHibernateAppenderLogger.cs",
                "NHibernateAppenderLoggerFactory.cs",
                "WrapNHibernateLoggers.cs",
                "NHibernateLoggerBridge.cs"
            };

            var nhibernateAssembly = GetAssembly("NHibernate");

            var referenceAssemblies = new List<string>
            {
                typeof(NHibernateAppender).Assembly.Location,
                nhibernateAssembly.Location,
                typeof(IDbConnection).Assembly.Location
            };

            var compiled = GenerateAssembly.Compile("NHibernateAppender.EmbeddedSources", embeddedSources, referenceAssemblies);
            var type = compiled.GetType("NHibernateAppenderLogger");
        }

        private static Assembly GetAssembly(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault((Assembly asm) => asm.GetName().Name == name);
        }
    }
}