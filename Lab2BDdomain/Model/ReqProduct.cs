using System;
using System.Collections.Generic;

namespace Lab2BDDomain.Model
{
    public partial class ReqProduct : Entity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ReqProductAmount { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
