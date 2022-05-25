﻿using Shrooms.Contracts.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shrooms.Contracts.DataTransferObjects.FilterPresets
{
    public class FilterPresetItemViewModel
    {
        [Required]
        [EnumDataType(typeof(FilterType))]
        public FilterType ForType { get; set; }

        [Required]
        public IEnumerable<string> Types { get; set; }
    }
}
