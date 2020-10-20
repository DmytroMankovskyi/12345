﻿using System;
using System.Collections.Generic;

namespace EventsExpress.DTO
{
    public class EventPreviewDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string PhotoUrl { get; set; }

        public UserPreviewDto User { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public Guid CityId { get; set; }

        public Guid CountryId { get; set; }

        public bool IsBlocked { get; set; }

        public int CountVisitor { get; set; }

        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
