﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.DataAccess.Xml
{
  public class XmlEnvironmentInfoRepository : IEnvironmentInfoRepository
  {
    #region Nested types

    public class EnvironmentInfosXml
    {
      public List<EnvironmentInfoXml> EnvironmentInfos { get; set; }
    }

    public class EnvironmentInfoXml
    {
      public string Name { get; set; }

      public string ConfigurationTemplateName { get; set; }

      public string AppServerMachineName { get; set; }

      public string FailoverClusterMachineName { get; set; }

      public List<string> WebServerMachineNames { get; set; }

      public string TerminalServerMachineName { get; set; }

      public List<string> SchedulerServerTasksMachineNames { get; set; }       
      
      public List<string> SchedulerServerBinariesMachineNames { get; set; }       

      public string NtServicesBaseDirPath { get; set; }

      public string WebAppsBaseDirPath { get; set; }

      public string SchedulerAppsBaseDirPath { get; set; }

      public string TerminalAppsBaseDirPath { get; set; }

      public bool EnableFailoverClusteringForNtServices { get; set; }

      public List<EnvironmentUserXml> EnvironmentUsers { get; set; }

      public List<AppPoolInfoXml> AppPoolInfos { get; set; }

      public List<DatabaseServerXml> DatabaseServers { get; set; }

      public List<WebAppProjectConfigurationXml> WebAppProjectConfigurations { get; set; }

      public List<ProjectToFailoverClusterGroupMappingXml> ProjectToFailoverClusterGroupMappings { get; set; }

      public List<DbProjectConfigurationXml> DbProjectConfigurations { get; set; }

      public string TerminalAppsShortcutFolder { get; set; }

      public string ManualDeploymentPackageDirPath { get; set; }
    }

    public class EnvironmentUserXml
    {
      public string Id { get; set; }

      public string UserName { get; set; }
    }

    public class AppPoolInfoXml
    {
      public string Name { get; set; }

      public IisAppPoolVersion Version { get; set; }

      public IisAppPoolMode Mode { get; set; }
    }

    public class DatabaseServerXml
    {
      public string Id { get; set; }

      public string MachineName { get; set; }
    }

    public class WebAppProjectConfigurationXml
    {
      [XmlAttribute("projectName")]
      public string ProjectName { get; set; }

      public string AppPoolId { get; set; }

      public string WebSiteName { get; set; }

      public string WebAppDirName { get; set; }

      public string WebAppName { get; set; }
    }

    public class ProjectToFailoverClusterGroupMappingXml
    {
      public string ProjectName { get; set; }

      public string ClusterGroupName { get; set; }
    }

    public class DbProjectConfigurationXml
    {
      [XmlAttribute("projectName")]
      public string ProjectName { get; set; }

      public string DatabaseServerId { get; set; }
    }

    #endregion

    private readonly string _xmlFilePath;

    private EnvironmentInfosXml _environmentInfosXml;
    private Dictionary<string, EnvironmentInfo> _environmentInfosByName;

    #region Constructor(s)

    public XmlEnvironmentInfoRepository(string xmlFilePath)
    {
      if (string.IsNullOrEmpty(xmlFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "xmlFilePath");
      }

      _xmlFilePath = xmlFilePath;
    }

    #endregion

    #region IEnvironmentInfoRepository Members

    public IEnumerable<EnvironmentInfo> GetAll()
    {
      LoadXmlIfNeeded();

      return _environmentInfosByName.Values
        .OrderByDescending(ei => ei.Name); // TODO IMM HI: OrderBy instead of OrderByDescending
    }

    public EnvironmentInfo FindByName(string environmentName)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      LoadXmlIfNeeded();

      EnvironmentInfo environmentInfo;

      if (!_environmentInfosByName.TryGetValue(environmentName, out environmentInfo))
      {
        return null;
      }

      return environmentInfo;
    }

    #endregion

    #region Private helper methods

    private void LoadXmlIfNeeded()
    {
      if (_environmentInfosXml != null)
      {
        return;
      }

      var xmlSerializer = new XmlSerializer(typeof(EnvironmentInfosXml));

      using (var fs = File.OpenRead(_xmlFilePath))
      {
        _environmentInfosXml = (EnvironmentInfosXml)xmlSerializer.Deserialize(fs);
      }

      _environmentInfosByName =
        _environmentInfosXml.EnvironmentInfos
          .Select(
            eiXml =>
            new EnvironmentInfo(
              eiXml.Name,
              eiXml.ConfigurationTemplateName,
              eiXml.AppServerMachineName,
              eiXml.FailoverClusterMachineName,
              eiXml.WebServerMachineNames,
              eiXml.TerminalServerMachineName,
              eiXml.SchedulerServerTasksMachineNames,
              eiXml.SchedulerServerBinariesMachineNames,
              eiXml.NtServicesBaseDirPath,
              eiXml.WebAppsBaseDirPath,
              eiXml.SchedulerAppsBaseDirPath,
              eiXml.TerminalAppsBaseDirPath,
              eiXml.EnableFailoverClusteringForNtServices,
              eiXml.EnvironmentUsers.Select(
                e =>
                new EnvironmentUser(
                  e.Id,
                  e.UserName)),
              eiXml.AppPoolInfos.Select(
                e =>
                new IisAppPoolInfo(
                  e.Name,
                  e.Version,
                  e.Mode)),
              eiXml.DatabaseServers.Select(
                e =>
                new DatabaseServer(
                  e.Id,
                  e.MachineName)),
              eiXml.WebAppProjectConfigurations.Select(
                e =>
                new WebAppProjectConfiguration(
                  e.ProjectName,
                  e.AppPoolId,
                  e.WebSiteName,
                  e.WebAppDirName,
                  e.WebAppName)),
              eiXml.ProjectToFailoverClusterGroupMappings.Select(
                e =>
                new ProjectToFailoverClusterGroupMapping(
                  e.ProjectName,
                  e.ClusterGroupName)),
              eiXml.DbProjectConfigurations.Select(
                e =>
                new DbProjectConfiguration(
                  e.ProjectName,
                  e.DatabaseServerId)),
              eiXml.TerminalAppsShortcutFolder,
              eiXml.ManualDeploymentPackageDirPath
              ))
          .ToDictionary(ei => ei.Name);
    }

    #endregion
  }
}
