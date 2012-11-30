using System;
using System.Collections;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Configuration;
using System.Linq;
using Sitecore.Links;
using ASR.Reports.DisplayItems;

namespace ASR.Reports.Scanners
{
	class MediaUsageScanner : ASR.Interface.BaseScanner
	{
		private Database _db;
		/// <summary>
		/// Gets the db.
		/// </summary>
		/// <value>The db.</value>
		protected Database Db
		{
			get
			{
				if (_db == null)
				{
					_db = Sitecore.Context.ContentDatabase == null ? Factory.GetDatabase("master") : Sitecore.Context.ContentDatabase;
				}
				return _db;
			}
		}
		public const string ROOT_PARAMETER = "root";

		public override ICollection Scan()
		{
			string rootParameter = base.getParameter(ROOT_PARAMETER);
			List<MediaUsageItem> itemList = new List<MediaUsageItem>();
			Item rootItem = null;
			if (!String.IsNullOrEmpty(rootParameter))
			{
				rootItem = Db.GetItem(rootParameter);
			}
			else
			{
				rootItem = Db.GetRootItem();
			}
			foreach (Item version in rootItem.Versions.GetVersions())
			{
				addItem(itemList, version);
			}

			Item[] descendants = rootItem.Axes.GetDescendants();
			foreach (Item item in descendants)
			{
				foreach (Item version in item.Versions.GetVersions())
				{
					addItem(itemList, version);
				}
			}

			return itemList;
		}

		private void addItem(List<MediaUsageItem> itemList, Item version)
		{
			if (Globals.LinkDatabase.GetReferenceCount(version) > 0)
			{
				foreach (var itemLink in Globals.LinkDatabase.GetReferences(version).Distinct(ItemLinkComparer.Instance))
				{
					Item linkItem = Db.GetItem(itemLink.TargetItemID);
					if (linkItem != null && linkItem.Paths.IsMediaItem)
					{
						itemList.Add(new MediaUsageItem(version, linkItem));
					}
				}
			}
		}

		class ItemLinkComparer : IEqualityComparer<ItemLink>
		{
			private static ItemLinkComparer _instance;
			public static ItemLinkComparer Instance
			{
				get
				{
					if (_instance == null)
					{
						_instance = new ItemLinkComparer();
					}
					return _instance;
				}
			}

			protected ItemLinkComparer() { }

			#region IEqualityComparer<ItemLink> Members

			public bool Equals(ItemLink x, ItemLink y)
			{
				return x.SourceDatabaseName == y.SourceDatabaseName
					&& x.SourceFieldID == y.SourceFieldID
					&& x.SourceItemID == y.SourceItemID
					&& x.TargetDatabaseName == y.TargetDatabaseName
					&& x.TargetItemID.ToString() == y.TargetItemID.ToString()
					&& x.TargetPath == y.TargetPath;
			}

			public int GetHashCode(ItemLink obj)
			{
				return obj.SourceDatabaseName.GetHashCode() & obj.SourceFieldID.GetHashCode() & obj.SourceItemID.GetHashCode()
					& obj.TargetDatabaseName.GetHashCode() & obj.TargetItemID.ToString().GetHashCode() & obj.TargetPath.GetHashCode();
			}

			#endregion
		}
	}
}
