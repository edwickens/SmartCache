using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cache;

namespace Cache.Data
{
    public class CacheContext : DbContext
    {
        public CacheContext (DbContextOptions<CacheContext> options)
            : base(options)
        {
        }

        public DbSet<Cache.ToDo> ToDo { get; set; } = default!;
    }
}
