using System;
using NHibernate;

namespace NHibernateProfilerLibrary
{
    public class NHibernateLoggerWithPublishEventFactory : INHibernateLoggerFactory
    {
        public static INHibernateLoggerFactory OriginalLoggerFactory;

        public INHibernateLogger LoggerFor(string keyName)
        {
            return new NHibernateLoggerWithPublishEvent(OriginalLoggerFactory.LoggerFor(keyName));
        }

        public INHibernateLogger LoggerFor(Type type)
        {
            return new NHibernateLoggerWithPublishEvent(OriginalLoggerFactory.LoggerFor(type));
        }
    }
}