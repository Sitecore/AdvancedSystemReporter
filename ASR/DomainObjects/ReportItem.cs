using System;
using System.Collections.Generic;
using CorePoint.DomainObjects;
using Sitecore.Data;
using System.Linq;
using ASR.Interface;
using Sitecore.Collections;

namespace ASR.DomainObjects
{
  using System.Collections.Specialized;

  using CorePoint.DomainObjects.SC;

  using Sitecore;
  using Sitecore.Diagnostics;

  [Serializable]
  [Template("System/ASR/Report")]
  public class ReportItem : CorePoint.DomainObjects.SC.StandardTemplate
  {

    [Field("__icon")]
    public string Icon
    {
      get;
      set;
    }

    [Field("scanners")]
    private List<Guid> _scanner
    {
      get;
      set;
    }

    public ScannerItem[] _scanners;
    public IEnumerable<ScannerItem> Scanners
    {
      get
      {
        if (_scanners == null)
        {
          _scanners = _scanner.ConvertAll<ScannerItem>(id => this.Director.GetObjectByIdentifier<ScannerItem>(id)).ToArray();
        }
        return _scanners;
      }
    }

    [Field("viewers")]
    private List<Guid> _viewer
    {
      get;
      set;
    }

    private ViewerItem[] _viewers = null;
    public IEnumerable<ViewerItem> Viewers
    {
      get
      {
        if (_viewers == null)
        {
          _viewers = _viewer.ConvertAll<ViewerItem>(g => this.Director.GetObjectByIdentifier<ViewerItem>(g)).ToArray();
        }
        return _viewers;
      }
    }

    [Field("commands")]
    public List<Guid> _Commands
    {
      get;
      set;
    }
    private CommandItem[] _commands;
    public IEnumerable<CommandItem> Commands
    {

      get
      {
        if (_commands == null)
        {
          _commands = _Commands.ConvertAll<CommandItem>(g => this.Director.GetObjectByIdentifier<CommandItem>(g)).ToArray();
        }
        return _commands;
      }
    }

    [Field("filters")]
    public List<Guid> _Filters
    {
      get;
      set;
    }
    private FilterItem[] _filters;
    public IEnumerable<FilterItem> Filters
    {
      get
      {
        if (_filters == null)
        {
          _filters = _Filters.ConvertAll<FilterItem>(g => this.Director.GetObjectByIdentifier<FilterItem>(g)).ToArray();

        }
        return _filters;
      }
    }

    [Field("email text")]
    public string EmailText
    {
      get;
      set;
    }
      [Field("description")]
      public string Description { get; set; }
      

      public void RunCommand(string commandname, StringList values)
    {
      foreach (var item in Commands)
      {
        if (item.Name == commandname)
        {
          item.Run(values);
          break;
        }
      }
    }

    private HashSet<ReferenceItem> objects;

    public ReferenceItem FindItem(string name)
    {
      if (objects == null)
      {
        loadObjects();
      }
      return objects.First(ri => ri.Name == name);
    }

    public ReferenceItem FindItem(Guid name)
    {
      if (objects == null)
      {
        loadObjects();
      }
      return objects.First(ri => ri.Id == name);
    }

    private void loadObjects()
    {
      objects = new HashSet<ReferenceItem>();
      foreach (var item in Scanners) { objects.Add(item); }
      foreach (var item in Viewers) { objects.Add(item); }
      foreach (var item in Filters) { objects.Add(item); }
    }

    public string SerializeParameters()
    {
      return SerializeParameters("^", "&");
    }

    public string SerializeParameters(string valueSeparator, string parameterSeparator)
    {
      System.Collections.Specialized.NameValueCollection nvc =
          new System.Collections.Specialized.NameValueCollection();
      nvc.Add("id", new ID(Current.Context.ReportItem.Id).ToString());
      foreach (var item in Current.Context.ReportItem.Scanners)
      {
        foreach (var p in item.Parameters)
        {
          nvc.Add(string.Concat(item.Id, valueSeparator, p.Name), p.Value);
        }
      }
      foreach (var item in Current.Context.ReportItem.Filters)
      {
        foreach (var p in item.Parameters)
        {
          nvc.Add(string.Concat(item.Id, valueSeparator, p.Name), p.Value);
        }
      }
      foreach (var item in Current.Context.ReportItem.Viewers)
      {
        foreach (var p in item.Parameters)
        {
          nvc.Add(string.Concat(item.Id, valueSeparator, p.Name), p.Value);
        }
      }
      return Sitecore.StringUtil.NameValuesToString(nvc, parameterSeparator);
    }

    public static ReportItem CreateFromParameters(string parameters)
    {
      Assert.ArgumentNotNullOrEmpty(parameters, "parameters");
      var nvc = StringUtil.ParseNameValueCollection(parameters, '&', '=');
      return CreateFromParameters(nvc);
    }

    public static ReportItem CreateFromParameters(NameValueCollection nvc)
    {
      Assert.IsNotNull(nvc, "Incorrect Parameters Format");
      var id = nvc["id"];
      if (id == null) return null;
      var director = new SCDirector("master", "en");
      if (!director.ObjectExists(id)) throw new Exception("Report has been deleted");

      var reportItem = director.GetObjectByIdentifier<ReportItem>(id);

      foreach (string key in nvc.Keys)
      {
        if (key.Contains("^"))
        {
          var item_parameter = key.Split('^');
          var guid = new Guid(item_parameter[0]);

          var ri = reportItem.FindItem(guid);
          if (ri != null)
          {
            ri.SetAttributeValue(item_parameter[1], nvc[key]);
          }
        }
      }
      return reportItem;
    }

    public Report TransformToReport( Report report)
    {
      if (report == null)
      {
        report = new Report();
      }
      foreach (var sItem in this.Scanners)
      {
        report.AddScanner(sItem);
      }
      foreach (var vItem in this.Viewers)
      {
        report.AddViewer(vItem);
      }
      foreach (var fItem in this.Filters)
      {
        report.AddFilter(fItem);
      }
     return report;
    }
  }
}
