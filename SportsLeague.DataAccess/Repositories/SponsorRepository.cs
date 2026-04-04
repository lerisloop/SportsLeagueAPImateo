using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository

    {
        public SponsorRepository(LeagueDbContext context) : base(context) { }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }
        public async Task<Sponsor?> GetByNameAsync(string name)

        {

            return await _dbSet

            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

        }

    }

}
