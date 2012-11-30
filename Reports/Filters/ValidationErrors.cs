using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using Sitecore.Data.Validators;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace ASR.Reports.Filters
{
	public class ValidationErrors : BaseFilter
	{
		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				ValidatorCollection validators = ValidatorManager.BuildValidators(ValidatorsMode, item);
				ValidatorManager.Validate(validators, new ValidatorOptions(false));
				foreach (BaseValidator validator in validators)
				{
					if (validator.Result >= ValidatorMinErrorLevel)
					{
						return true;
					}
				}
			}
			return false;
		}

	    public string MinErrorLevel { get; set; }
		public ValidatorResult ValidatorMinErrorLevel
		{
			get
			{
			
				return (ValidatorResult)Enum.Parse(typeof(ValidatorResult), MinErrorLevel);
			}
		}

        public string Mode { get; set; }
		public ValidatorsMode ValidatorsMode
		{
			get
			{				
				return (ValidatorsMode)Enum.Parse(typeof(ValidatorsMode), Mode);
			}
		}
	}
}
