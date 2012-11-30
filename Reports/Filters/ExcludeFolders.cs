using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class ExcludeFolders : ASR.Interface.BaseFilter
	{
		/// <summary>
		/// Filter excludes all folders from the result set.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item!=null)
			{
				if (item.TemplateID.Guid == Sitecore.TemplateIDs.MediaFolder.Guid ||
					item.TemplateID.Guid == Sitecore.TemplateIDs.Folder.Guid)
				{
					return false;
				}
			}
			return true;
		}
	}
}
