﻿using Shrooms.Contracts.Enums;

namespace Shrooms.Premium.Presentation.WebViewModels.Events
{
    public class EventOptionViewModel
    {
        public int Id { get; set; }

        public string Option { get; set; }

        public OptionRules Rule { get; set; }
    }
}