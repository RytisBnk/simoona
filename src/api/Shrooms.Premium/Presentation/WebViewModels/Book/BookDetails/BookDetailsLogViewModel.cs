﻿using System;

namespace Shrooms.Premium.Presentation.WebViewModels.Book.BookDetails
{
    public class BookDetailsLogViewModel
    {
        public string UserId { get; set; }

        public int LogId { get; set; }

        public DateTime TakenFrom { get; set; }

        public DateTime TakenTill { get; set; }

        public DateTime? Returned { get; set; }

        public string FullName { get; set; }
    }
}
