﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shrooms.WebViewModels.Models.Events
{
    public class EventJoinViewModel
    {
        [Required]
        public Guid EventId { get; set; }

        public IEnumerable<int> ChosenOptions { get; set; }
    }
}
