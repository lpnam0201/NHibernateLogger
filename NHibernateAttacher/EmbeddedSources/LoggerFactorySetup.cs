using System;
using System.Diagnostics;
using System.Reflection;
using NHibernate;

namespace NHibernateProfilerLibrary
{
    public class LoggerFactorySetup
    {
        public static void Initialize()
        {
            var originalLoggerFactory = GetOriginalLoggerFactory();

            NHibernateLoggerWithPublishEventFactory.OriginalLoggerFactory = originalLoggerFactory;
            NHibernateLogger.SetLoggersFactory(new NHibernateLoggerWithPublishEventFactory());
        }

        public static void Shutdown()
        {
            NHibernateLogger.SetLoggersFactory(NHibernateLoggerWithPublishEventFactory.OriginalLoggerFactory);
        }

        private static INHibernateLoggerFactory GetOriginalLoggerFactory()
        {
            var loggerProvider = typeof(NHibernateLogger);
            var instanceField = loggerProvider.GetField("_loggerFactory", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
            var loggerFactory = (INHibernateLoggerFactory)instanceField.GetValue(null);
            if (loggerFactory != null)
            {
                return loggerFactory;
            }

            return loggerFactory;
        }
    }
}
