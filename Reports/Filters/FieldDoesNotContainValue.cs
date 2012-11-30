using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	/// <summary>
	/// Filters items based on field values they contains.
	/// </summary>
	class FieldDoesNotContainValue : ASR.Interface.BaseFilter
	{
		

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		/// <value>The name of the field.</value>
		public string FieldName
		{
			get;set;			
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get;set;
		}

		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				Field field = item.Fields[FieldName];
				if (field != null && field.HasValue && field.Value != Value)
				{
					return true;
				}
			}
			return false;
		}
	}
}
