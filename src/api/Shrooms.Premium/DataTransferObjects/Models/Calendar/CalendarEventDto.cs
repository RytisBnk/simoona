﻿using System;

namespace Shrooms.Premium.DataTransferObjects.Models.Calendar
{
    public class CalendarEventDto
    {
        public Guid EventId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Location { get; set; }
    }
}
