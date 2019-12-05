using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public enum VirtualMoneyTransactionType
    {
        /// <summary>
        /// Кешбек за оформление заказа
        /// </summary>
        EnrollmentPurchase = 0,

        /// <summary>
        /// Выплата за нового клиента
        /// </summary>
        EnrollmentReferral = 1,

        /// <summary>
        /// Реферальная выплата новому клиенту
        /// </summary>
        EnrollmentReferralBonus =2,

        /// <summary>
        /// Оплата заказа
        /// </summary>
        OrderPayment = 3,

        /// <summary>
        /// Возврат виртуальных средств
        /// </summary>
        Refund = 4
    }
}