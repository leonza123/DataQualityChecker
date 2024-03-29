﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Models
{
    public class SearchRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public ColumnRequestItem[] Columns { get; set; }
        public OrderRequestItem[] Order { get; set; }
        public SearchRequestItem Search { get; set; }
    }

    public class ColumnRequestItem
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public SearchRequestItem Search { get; set; }
    }

    public class OrderRequestItem
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class SearchRequestItem
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }
}
