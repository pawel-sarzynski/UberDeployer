﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class UpdateApplicationShortcutDeploymentStepTests
  {
    [Test]
    public void shortcut_is_created_and_named_after_terminal_app()
    {
      var step = new UpdateApplicationShortcutDeploymentStep(
        "TestData/Shortcuts",
        new Lazy<string>(() => "TestData/VersionedFolders/TestProject/1.0.3.5"),
        "FolderMattersFile.dummy",
        "TestProject");

      step.PrepareAndExecute(DeploymentInfoGenerator.GetDbDeploymentInfo());

      Assert.IsTrue(File.Exists("TestData/Shortcuts/TestProject.lnk"));
      File.Delete("TestData/ShortCuts/TestProject.lnk");
    }

    [Test]
    public void shortcut_can_be_modified()
    {
      var step = new UpdateApplicationShortcutDeploymentStep(
        "TestData/Shortcuts",
        new Lazy<string>(() => "TestData/VersionedFolders/TestProject/1.0.3.5"),
        "FolderMattersFile.dummy",
        "ExistingAppShortcut");

      step.PrepareAndExecute(DeploymentInfoGenerator.GetDbDeploymentInfo());

      var modifiedDate = File.GetLastWriteTime("TestData/Shortcuts/ExistingAppShortcut.lnk");
      Assert.LessOrEqual((DateTime.Now - modifiedDate).TotalMilliseconds, 10);
    }
  }
}
 