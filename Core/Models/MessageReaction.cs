﻿namespace Core.Models
{
    public class MessageReaction
    {
        public int Id { get; set; }
        public string? Reaction { get; set; }
        public Person? Person { get; set; }
    }
}
