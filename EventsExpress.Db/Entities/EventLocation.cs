﻿using System;
using EventsExpress.Db.EF;
using EventsExpress.Db.Enums;
using NetTopologySuite.Geometries;

namespace EventsExpress.Db.Entities
{
    [Track]
    public class EventLocation : BaseEntity
    {
        [Track]
        public Point Point { get; set; }

        [Track]
        public Uri OnlineMeeting { get; set; }

        [Track]
        public LocationType Type { get; set; }
    }
}
