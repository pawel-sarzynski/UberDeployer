﻿using System;

namespace UberDeployer.WebApp.Core
{
  [Serializable]
  public class InternalException : Exception
  {
    #region Constructor(s)

    public InternalException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public InternalException(string message)
      : this(message, null)
    {
    }

    #endregion
  }
}
