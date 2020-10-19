﻿using System;

namespace EventsExpress.DTO
{
    public class UserPreviewDto
    {
        public Guid Id { get; set; }

        public string PhotoUrl { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateTime Birthday { get; set; }

        public double Rating { get; set; }
    }
}
