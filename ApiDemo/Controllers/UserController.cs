﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly TaskContext _context;
        public UserController(TaskContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.Where(p=>p.IsDeleted != true).ToListAsync();
        }

        // GET api/user/sample@mail.com
        [HttpGet("{email}")]
        public async Task<User> Get(string email)
        {
            var item = await _context.Users.FirstOrDefaultAsync(x => x.EmailAddress == email && x.IsDeleted != true);
            return item;
        }

        // POST api/user
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            else
            {
                var itemExists = await _context.Users.AnyAsync(i => i.EmailAddress == request.EmailAddress && i.IsDeleted != true);
                if (itemExists)
                {
                    return BadRequest();
                }
                User item = new Models.User();
                item.UserId = Guid.NewGuid().ToString().Replace("-", "");
                item.CreatedOnUtc = DateTime.UtcNow;
                item.UpdatedOnUtc = DateTime.UtcNow;
                item.EmailAddress = request.EmailAddress;
                _context.Users.Add(item);
                await _context.SaveChangesAsync();
                
                return new StatusCodeResult(201);
            }
        }

        // DELETE api/user/3ab4fcbd993f49ce8a21103c713bf47a
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]DeleteUserRequest request)
        {
            var item = await _context.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.IsDeleted != true);
            if (item == null)
            {
                return NotFound();
            }
            item.IsDeleted = true;
            item.UpdatedOnUtc = DateTime.UtcNow;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204); // 201 No Content
        }
    }



}
