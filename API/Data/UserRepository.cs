using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        public async Task<MemberDto> GetMemberAsync(string userName)
        {
            return await _context.Users
                                 .Where(u => u.UserName == userName)
                                 .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                                 .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UsersParams  usersParams)
        {
            var query = _context.Users.AsQueryable();

            query =query.Where(u => u.UserName != usersParams.CurrentUserName);
            query = query.Where(u => u.Gender == usersParams.Gender);

            var minnDob = DateTime.Today.AddYears(-usersParams.MaxAge -1);
            var maxDob = DateTime.Today.AddYears(-usersParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minnDob && u.DateOfBirth <= maxDob);

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking()
            ,usersParams.PageNumber, usersParams.pageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.Include(u => u.Photos).SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}