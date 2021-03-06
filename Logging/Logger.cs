﻿using System;
using System.Collections.Generic;

namespace Logging
{
  /// <summary>
  /// Interface to logging functionality
  /// 
  /// Initialise logging by adding something like the following at process start
  /// 
  /// var logEncoder = new XmlLogEncoder();
  /// var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
  /// var logWriter = new LogWriter(logEncoder, byteWriter);
  /// Logger.AddLogWriter(logWriter);
  /// </summary>
  public static class Logger
  {
    /// <summary>
    /// Log calls with level above this value will be discarded
    /// </summary>
    public static LogLevel LogLevel
    {
      get { return mLogLevel; }
      set { mLogLevel = value; }
    }


    private static List<Lazy<ILogWriter>> LogWriters { get; } = new List<Lazy<ILogWriter>>();

    private static volatile LogLevel mLogLevel = LogLevel.Warning;


    private static readonly object mLock = new object();


    /// <summary>
    /// A log writer instance is only added once. Subsequent calls to add the same log writer will have no effect.
    /// </summary>
    public static void AddLogWriter(Lazy<ILogWriter> logWriter)
    {
      if (logWriter == null)
        return;

      lock (mLock)
      {
        if (LogWriters.Contains(logWriter))
          return;

        LogWriters.Add(logWriter);
      }
    }


    /// <summary>
    /// This function will have no effect if the log writer has not been added
    /// </summary>
    public static void RemoveLogWriter(Lazy<ILogWriter> logWriter)
    {
      if (logWriter == null)
        return;

      lock (mLock)
      {
        if (LogWriters.Contains(logWriter) == false)
          return;

        LogWriters.Remove(logWriter);
      }
    }


    public static void ClearAllLogWriters()
    {
      lock (mLock)
      {
        LogWriters.Clear();
      }
    }


    /// <summary>
    /// Log a message if the level is equal to or less than the logger level.
    /// If the level is above the logger level the message will be disgarded.
    /// </summary>
    public static void Log(LogLevel level, string id, params object[] parameters)
    {
      // The current level is checked first because this is a common early return scenario
      if (level > LogLevel)
        return;

      if (id == null)
        return;

      // It is anticipated that execution where the lock is not available
      // will be so rare that performance impact will be negligible
      lock (mLock)
      {
        foreach (var logwriter in LogWriters)
        {
          try
          {
            logwriter.Value.Log(id, parameters);
          }
          catch (Exception)
          {
            // Do nothing. Can't log because it's logging that's failed. Can't throw because
            // an application failure due to debug logging would be ridiculous.
          }
        }
      }
    }
  }
}