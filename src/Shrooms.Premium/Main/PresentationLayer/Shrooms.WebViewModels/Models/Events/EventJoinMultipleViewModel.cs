﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shrooms.WebViewModels.Models.Events
{
    public class EventJoinMultipleViewModel
    {
        [Required]
        public Guid EventId { get; set; }

        [Required]
        public ICollection<string> ParticipantIds { get; set; }

        public ICollection<int> ChosenOptions { get; set; }
    }
}