﻿namespace Pezza.Common.DTO
{
    using System;
    using Pezza.Common.Models.Base;

    public class StockDTO : EntityBase
    {
        public string Name { get; set; }

        public string UnitOfMeasure { get; set; }

        public double? ValueOfMeasure { get; set; }

        public int? Quantity { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string Comment { get; set; }
    }
}
