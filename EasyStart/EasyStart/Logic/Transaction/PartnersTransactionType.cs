using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Transaction
{
    public enum PartnersTransactionType
    {
        /// <summary>
        /// Выплата за нового клиента
        /// </summary>
        EnrollmentReferral = 0,

        /// <summary>
        /// Реферальная выплата новому клиенту
        /// </summary>
        EnrollmentReferralBonus =1,
    }
}