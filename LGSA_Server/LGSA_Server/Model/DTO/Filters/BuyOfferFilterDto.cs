﻿using LGSA_Server.Model.DTO.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LGSA_Server.Model.DTO.Filters
{
    public class BuyOfferFilterDto : IFilterDto<buy_Offer>
    {
        public string ProductName { get; set; }
        public int? GenreId { get; set; }
        public int? ConditionId { get; set; }
        public int? ProductTypeId { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }

        public Expression<Func<buy_Offer, bool>> GetFilter(int userId)
        {
            Expression<Func<buy_Offer, bool>> filter = b => b.buyer_id == userId && b.product.stock >= Stock && b.status_id == 1;

            if (ProductName != null)
            {
                Expression<Func<buy_Offer, bool>> f = b => b.product.Name.Contains(ProductName);
                filter = Expression.Lambda<Func<buy_Offer, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }
            if (ConditionId != null)
            {
                Expression<Func<buy_Offer, bool>> f = b => b.product.condition_id == ConditionId;
                filter = Expression.Lambda<Func<buy_Offer, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }
            if (GenreId != null)
            {
                Expression<Func<buy_Offer, bool>> f = b => b.product.genre_id == GenreId;
                filter = Expression.Lambda<Func<buy_Offer, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }
            if (ProductTypeId != null)
            {
                Expression<Func<buy_Offer, bool>> f = b => b.product.product_type_id == ProductTypeId;
                filter = Expression.Lambda<Func<buy_Offer, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }

            return filter;
        }
    }
}