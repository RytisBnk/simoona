﻿using System.Collections.Generic;

namespace Shrooms.Premium.DataTransferObjects.Models.Committees
{
    public class CommitteeViewDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PictureId { get; set; }

        public string Website { get; set; }

        public bool IsKudosCommittee { get; set; }

        public ICollection<CommitteeMembersDto> Members { get; set; }
    }
}
