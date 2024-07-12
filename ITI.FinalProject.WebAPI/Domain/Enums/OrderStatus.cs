using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum OrderStatus
    {
        New,
        Pending ,
        RepresentativeDelivered,
        Delivered ,
        Unreachable ,
        Delayed,
        PartiallyDelivered,
        Cancelled,
        RejectedWithPayment,
        RejectedWithPartiallyPayment,
        RejectedWithoutPayment

    }
}
