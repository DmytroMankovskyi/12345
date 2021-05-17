﻿using System;
using System.Collections.Generic;
using EventsExpress.Db.Enums;

namespace EventsExpress.ViewModels
{
    public class UserManageViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public DateTime Birthday { get; set; }

        public Gender Gender { get; set; }

        public bool IsBlocked { get; set; }

        public byte Attitude { get; set; }

        public IEnumerable<RoleViewModel> Roles { get; set; }

        public double Rating { get; set; }
    }
}
