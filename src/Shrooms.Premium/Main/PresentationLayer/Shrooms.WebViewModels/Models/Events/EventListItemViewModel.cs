﻿using System;

namespace Shrooms.WebViewModels.Models.Events
{
    public class EventListItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImageName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RegistrationDeadlineDate { get; set; }

        public string Place { get; set; }

        public int MaxParticipants { get; set; }

        public int ParticipantsCount { get; set; }

        public bool IsCreator { get; set; }

        public bool IsParticipating { get; set; }

        public int MaxChoices { get; set; }
    }
}
