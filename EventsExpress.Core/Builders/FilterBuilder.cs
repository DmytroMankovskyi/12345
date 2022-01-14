﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace EventsExpress.Core.Builders
{
    public class FilterBuilder<T>
    {
        private bool filterWillApply = true;
        private IQueryable<T> _queryable;

        public FilterBuilder(IQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        public FilterBuilder<T> If(bool condition)
        {
            filterWillApply = filterWillApply && condition;
            return this;
        }

        public FilterBuilder<T> IfNotNull<TValue>(params TValue[] values)
        {
            foreach (var value in values)
            {
                If(value is not null);
            }

            return this;
        }

        public NextFilterBuilder<T> AddFilter(Expression<Func<T, bool>> filter)
        {
            _queryable = filterWillApply ? _queryable.Where(filter) : _queryable;
            filterWillApply = true;
            return new NextFilterBuilder<T>(_queryable);
        }
    }
}