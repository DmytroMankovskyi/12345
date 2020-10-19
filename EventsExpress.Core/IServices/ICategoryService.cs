﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventsExpress.Core.DTOs;
using EventsExpress.Core.Infrastructure;
using EventsExpress.Db.Entities;

namespace EventsExpress.Core.IServices
{
    public interface ICategoryService
    {
        IEnumerable<CategoryDTO> GetAllCategories();

        Category Get(Guid id);

        Task<OperationResult> Create(string title);

        Task<OperationResult> Edit(CategoryDTO category);

        Task<OperationResult> Delete(Guid id);
    }
}
