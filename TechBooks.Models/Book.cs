using System;
using System.Collections.Generic;

namespace TechBooks.Models;

public partial class Book
{
    public int BookId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();

    public virtual Category Category { get; set; } = null!;
}
