using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2BDDomain.Model
{
    public enum OrderStatus
    {
        Received,
        Declined,
        InProcess,
        GivenToDeliveryService,
        Done,
        Canceled
    }

}
