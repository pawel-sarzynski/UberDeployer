﻿using System;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class DeploymentRequest
  {
    public DateTime DateRequested { get; set; }

    public string RequesterIdentity { get; set; }

    public string ProjectName { get; set; }

    public string TargetEnvironmentName { get; set; }

    public bool FinishedSuccessfully { get; set; }
  }
}
