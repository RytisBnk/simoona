﻿using Shrooms.Constants.BusinessLayer.Events;

namespace Shrooms.DataTransferObjects.Models.Events
{
    public class EventOptionDTO
    {
        public int Id { get; set; }

        public string Option { get; set; }

        public OptionRules Rule { get; set; }
    }
}
