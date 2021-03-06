﻿using System.Collections.Generic;
using QuantSA.Shared.Dates;

namespace QuantSA.Shared.Primitives
{
    /// <summary>
    /// The cashflows on a product which exercises into another are of two type:
    /// <para/>
    /// 1: Cashflows that take place until exercise, we assume that cashflows that take 
    /// place before an exercise event are independent of the time of that exercise event.
    /// <para/>
    /// 2: Cashflows that take place if exercise occurs at an exercise date.  For example 
    /// a penalty that must be paid at the point of exercise on a cancellable swap.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <seealso cref="IProduct" />
    public interface IProductWithEarlyExercise : IProduct
    {
        /// <summary>
        /// Gets the list of products that this product can exercise into.
        /// </summary>
        /// <remarks>
        /// It is a list in case the underlying product is different at each exercise date
        /// </remarks>
        /// <returns></returns>
        List<IProduct> GetPostExProducts();

        /// <summary>
        /// Gets the exercise dates of the option
        /// </summary>
        /// <returns></returns>
        List<Date> GetExerciseDates();

        /// <summary>
        /// Gets the product that will be exercised into at this date.  Returned as an index of the list of 
        /// products in
        /// </summary>
        /// <param name="exDate">The exercise date.  Must be in the list of dates returned by <see cref="GetExerciseDates"/>.</param>
        /// <returns></returns>
        int GetPostExProductAtDate(Date exDate);

        /// <summary>
        /// Is this product long optionality.  i.e. at each exercise date will the decision made by the holder in which case
        /// the value will be the maximum of the continuation or exercise value.
        /// </summary>
        /// <param name="exDate">The exercise date.  Must be in the list of dates returned by <see cref="GetExerciseDates"/>.</param>
        /// <returns></returns>
        bool IsLongOptionality(Date exDate);
    }
}