using System;
using NHibernate;
using NHibernateProfilerLibrary.Messages;

namespace NHibernateProfilerLibrary
{
	public class NHibernateLoggerWithPublishEvent : INHibernateLogger
	{
		private readonly INHibernateLogger internalLogger;

		public NHibernateLoggerWithPublishEvent(INHibernateLogger internalLogger)
		{
			this.internalLogger = internalLogger;
		}

		public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
		{
			internalLogger.Log(logLevel, state, exception);

			PublishEvent(state.ToString());
		}

		public bool IsEnabled(NHibernateLogLevel logLevel)
		{
			switch (logLevel)
			{
				case NHibernateLogLevel.None:
				case NHibernateLogLevel.Trace:
				case NHibernateLogLevel.Debug:
				case NHibernateLogLevel.Info:
					return internalLogger.IsEnabled(logLevel);
				case NHibernateLogLevel.Warn:
				case NHibernateLogLevel.Error:
				case NHibernateLogLevel.Fatal:
					return true;
				default:
					throw new ArgumentOutOfRangeException("logLevel", logLevel, null);
			}
		}

		private void PublishEvent(string message)
        {
			var publishedEvent = new PublishedEvent();
			publishedEvent.Message = message;
			NHibernateProfilerStateManager.Add(publishedEvent);

		}
	}
}
    