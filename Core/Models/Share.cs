﻿namespace Core.Models
{
    using System.Diagnostics.CodeAnalysis;

    public class Share
    {
        public int Id {  get; set; }
        public string? Url { get; set; }
        public string? ShareText { get; set; }
        public string? OriginalContentOwner { get; set; }
    }
}
