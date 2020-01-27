using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Transaction
{
    public enum CashbackTransactionType
    {
        /// <summary>
        /// Кешбек за оформление заказа
        /// </summary>
        EnrollmentPurchase = 0,
            /// <summary>
        /// Оплата заказа
        /// </summary>
        OrderPayment = 1,
    }
}