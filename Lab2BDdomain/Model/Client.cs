using System;
using System.Collections.Generic;

namespace Lab2BDDomain.Model;

public partial class Client: Entity
{
    public int Id { get; set; }

    public string ClientName { get; set; } = null!;

    public string Contacts { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
