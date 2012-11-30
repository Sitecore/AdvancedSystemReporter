using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ASR.Interface;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.Items
{
    public class ItemViewer : BaseViewer
    {
        #region Fields

        public static string COLUMNS_PARAMETER = "columns";
        public static string HEADERS_PARAMETER = "headers";
        public static string MAX_LENGHT_PARAMETER = "maxlength";

        private int maxLength = -1;

        #endregion

        #region Public properties

        public int MaxLength
        {
            get
            {
                if (maxLength < 0)
                {
                    if (!int.TryParse(getParameter(MAX_LENGHT_PARAMETER), out maxLength))
                    {
                        maxLength = 100;
                    }
                }
                return maxLength;
            }
        }

        public string DateFormat { get; set; }

        #endregion

        #region Public methods

        public override void Display(DisplayElement d_element)
        {
            Item itemElement = null;
            if (d_element.Element is Item)
            {
                itemElement = (Item)d_element.Element;
            }
            else if (d_element.Element is ID)
            {
                itemElement = Sitecore.Context.ContentDatabase.GetItem(d_element.Element as ID);
            }

            if (itemElement == null)
            {
                return;
            }
            d_element.Value = itemElement.Uri.ToString();

            d_element.Header = itemElement.Name;

            foreach (var column in Columns)
            {
                if (!d_element.HasColumn(column.Header))
                {
                    var text = getColumnText(column.Name, itemElement);
                    d_element.AddColumn(column.Header, string.IsNullOrEmpty(text) ? itemElement[column.Name] : text);
                }
            }
            d_element.Icon = itemElement.Appearance.Icon;
        }

        #endregion

        #region Protected methods

        protected virtual string formatDateField(Item item, ID fieldID)
        {
            DateField field = item.Fields[fieldID];
            if (field != null && !String.IsNullOrEmpty(field.Value))
            {
                var dateTimeFormatInfo = CultureInfo.CurrentUICulture.DateTimeFormat;

                var format = string.IsNullOrEmpty(DateFormat)
                                   ? dateTimeFormatInfo.ShortDatePattern
                                   : DateFormat;
                if (field.InnerField.TypeKey == "datetime")
                    return field.DateTime.ToString(string.Concat(format, " ", dateTimeFormatInfo.ShortTimePattern));
                else
                    return field.DateTime.ToString(format);
            }
            return string.Empty;
        }

        protected virtual string getColumnText(string name, Item itemElement)
        {
            switch (name)
            {
                case "name":
                    return itemElement.Name;

                case "display name":
                    return itemElement.DisplayName;

                case "created by":
                    return itemElement[FieldIDs.CreatedBy];

                case "updated":
                    return formatDateField(itemElement, FieldIDs.Updated);

                case "updated by":
                    return itemElement[FieldIDs.UpdatedBy];

                case "created":
                    return formatDateField(itemElement, FieldIDs.Created);

                case "locked by":
                    LockField lf = itemElement.Fields["__lock"];
                    var text = "unlocked";
                    if (lf != null)
                    {
                        if (!string.IsNullOrEmpty(lf.Owner))
                            text = lf.Owner + " " + lf.Date.ToString("dd/MM/yy HH:mm");
                    }
                    return text;
                case "template":
                    return itemElement.Template.Name;

                case "path":
                    return itemElement.Paths.FullPath;

                case "owner":
                    return itemElement[FieldIDs.Owner];

                case "workflow":
                    return getWorkflowInfo(itemElement);

                case "children count":
                    return itemElement.Children.Count.ToString();

                case "version":
                    return itemElement.Version.ToString();

                case "versions":
                    return itemElement.Versions.Count.ToString();

                case "language":
                    return itemElement.Language.CultureInfo.DisplayName;
                default:
                    return GetFriendlyFieldValue(name, itemElement);
            }
        }

        #endregion

        #region Private methods

        private static string getWorkflowInfo(Item itemElement)
        {
            var sb = new StringBuilder();
            var iw = itemElement.State.GetWorkflow();
            if (iw != null)
            {
                sb.Append(iw.Appearance.DisplayName);
            }
            var ws = itemElement.State.GetWorkflowState();

            if (ws != null)
            {
                sb.AppendFormat(" ({0})", ws.DisplayName);
            }

            if (iw != null)
            {
                IEnumerable<WorkflowEvent> events = iw.GetHistory(itemElement).OrderByDescending(e => e.Date);
                var enumerator = events.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    var span = DateTime.Now.Subtract(enumerator.Current.Date);
                    sb.AppendFormat(" for {0} days {1} hours {2} minutes", span.Days, span.Hours, span.Minutes);
                }
            }
            return sb.ToString();
        }

        protected virtual string GetFriendlyFieldValue(string name, Item itemElement)
        {
            // to allow forcing fields rather than properties, allow prepending the name with #
            name = name.TrimStart('@');
            var field = itemElement.Fields[name];
            if (field != null)
            {
                switch (field.TypeKey)
                {
                    case "date":
                    case "datetime":
                        return formatDateField(itemElement, field.ID);
                    case "droplink":
                    case "droptree":
                    case "reference":
                    case "grouped droplink":
                        var lookupFld = (LookupField)field;
                        if (lookupFld.TargetItem != null)
                        {
                            return lookupFld.TargetItem.Name;
                        }
                        break;
                    case "checklist":
                    case "multilist":
                    case "treelist":
                    case "treelistex":
                        var multilistField = (MultilistField)field;
                        var strBuilder = new StringBuilder();
                        foreach (var item in multilistField.GetItems())
                        {
                            strBuilder.AppendFormat("{0}, ", item.Name);
                        }
                        return StringUtil.Clip(strBuilder.ToString().TrimEnd(',', ' '), this.MaxLength, true);
                        break;
                    case "link":
                    case "general link":
                        var lf = new LinkField(field);
                        switch (lf.LinkType)
                        {
                            case "media":
                            case "internal":
                                if (lf.TargetItem != null)
                                {
                                    return lf.TargetItem.Paths.ContentPath;
                                }
                                return lf.Value == string.Empty ? "[undefined]" : "[broken link] " + lf.Value;
                            case "anchor":
                            case "mailto":
                            case "external":
                                return lf.Url;
                            default:
                                return lf.Text;
                        }
                    default:
                        return StringUtil.Clip(StringUtil.RemoveTags(field.Value), MaxLength, true);
                }
            }
            return String.Empty;
        }

        #endregion
    }
}