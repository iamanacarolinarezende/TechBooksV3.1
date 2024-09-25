using System;
using System.Collections.Generic;

namespace TechBooks.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
