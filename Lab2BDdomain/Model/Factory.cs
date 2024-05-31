using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab2BDDomain.Model;

public partial class Factory : Entity
{
    public int Id { get; set; }
    public string Adress { get; set; } = null!;
    public int Maintenance { get; set; } 
    public string FactoryName { get; set; } = null!;

    public virtual ICollection<FactoryProduct> FactoryProducts { get; set; } = new List<FactoryProduct>();

}
