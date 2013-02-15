using System;
using System.Collections.Generic;
using Sitecore.Data;
using System.Linq;
using ASR.Interface;
using Sitecore.Collections;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    using System.Collections.Specialized;
    using Sitecore;
    using Sitecore.Diagnostics;

    [Serializable]    
    public class ReportItem : BaseItem
    {
        public ReportItem(Item innerItem) : base(innerItem)
        {
        }

        #region Item Fields

        private IEnumerable<ScannerItem> _scanners; 
        public IEnumerable<ScannerItem> Scanners
        {
            get { return _scanners ?? (_scanners = GetMultilistField("scanners").Select(i => new ScannerItem(i)).ToArray()); }
        }

        private IEnumerable<ViewerItem> _viewers; 
        public IEnumerable<ViewerItem> Viewers
        {
            get { return _viewers ?? (_viewers = GetMultilistField("viewers").Select(i => new ViewerItem(i)).ToArray()); }
        }

        private IEnumerable<CommandItem> _commands;
        public IEnumerable<CommandItem> Commands
        {
            get { return _commands ?? (_commands = GetMultilistField("commands").Select(i => new CommandItem(i)).ToArray()); }
        }

        private IEnumerable<FilterItem> _filters; 
        public IEnumerable<FilterItem> Filters
        {
            get { return _filters ?? (_filters = GetMultilistField("filters").Select(i => new FilterItem(i)).ToArray()); }
        }
        
        public string EmailText
        {
            get { return InnerItem["email text"]; }
        }
        
        public string Description
        {
            get { return InnerItem["description"]; }
        } 
        #endregion


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

        private HashSet<ReferenceItem> _objects;

        public ReferenceItem FindItem(string name)
        {
            if (_objects == null)
            {
                LoadObjects();
            }
            return _objects.First(ri => ri.Name == name);
        }

        public ReferenceItem FindItem(ID name)
        {
            if (_objects == null)
            {
                LoadObjects();
            }
            return _objects.First(ri => ri.ID == name);
        }

        private void LoadObjects()
        {
            _objects = new HashSet<ReferenceItem>();
            foreach (var item in Scanners) { _objects.Add(item); }
            foreach (var item in Viewers) { _objects.Add(item); }
            foreach (var item in Filters) { _objects.Add(item); }
        }

        public string SerializeParameters()
        {
            return SerializeParameters("^", "&");
        }

        public string SerializeParameters(string valueSeparator, string parameterSeparator)
        {
            var nvc = new NameValueCollection { { "id", Current.Context.ReportItem.ID.ToString() } };

            foreach (var item in Current.Context.ReportItem.Scanners)
            {
                foreach (var p in item.Parameters)
                {
                    nvc.Add(string.Concat(item.ID, valueSeparator, p.Name), p.Value);
                }
            }
            foreach (var item in Current.Context.ReportItem.Filters)
            {
                foreach (var p in item.Parameters)
                {
                    nvc.Add(string.Concat(item.ID, valueSeparator, p.Name), p.Value);
                }
            }
            foreach (var item in Current.Context.ReportItem.Viewers)
            {
                foreach (var p in item.Parameters)
                {
                    nvc.Add(string.Concat(item.ID, valueSeparator, p.Name), p.Value);
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
            var database = Sitecore.Configuration.Factory.GetDatabase(Settings.Instance.ConfigurationDatabase);            
            var reportItem = new ReportItem(database.GetItem(id));

            if (reportItem == null) throw new Exception("Report has been deleted");

            foreach (string key in nvc.Keys)
            {
                if (key.Contains("^"))
                {
                    var item_parameter = key.Split('^');
                    var guid = new ID(item_parameter[0]);

                    var ri = reportItem.FindItem(guid);
                    if (ri != null)
                    {
                        ri.SetAttributeValue(item_parameter[1], nvc[key]);
                    }
                }
            }
            return reportItem;
        }

        public Report TransformToReport(Report report)
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
