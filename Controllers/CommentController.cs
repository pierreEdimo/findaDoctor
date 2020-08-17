﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using newsroom.DBContext;
using newsroom.DTO;
using newsroom.Model;
using newsroom.QueryClasses;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace newsroom.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private DatabaseContext _context;

        public CommentController(DatabaseContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet(Name = nameof(GetComments))]
        public async Task<ActionResult<IEnumerable<CommentDTo>>> GetComments()
        {
            IQueryable<Comments> comments = _context.Comments;

            return await comments.Include(a => a.author).Include(a => a.Answers).ThenInclude(x => x.Author).Select(x => commentToDTo(x)).ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<CommentDTo>> commentArticle(CommentDTo commentDTo)
        {
            var comment = new Comments
            {

                uid = commentDTo.uid,
                content = commentDTo.content,
                articleId = commentDTo.articleId,


            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, commentToDTo(comment));
        }


        [HttpPut]
        public async Task<IActionResult> UpdateComment(int Id, CommentDTo commentDTo)
        {
            if (Id != commentDTo.Id)
            {
                return BadRequest();
            }

            var comment = await _context.Comments.FindAsync(Id);
            if (comment == null)
            {
                return NotFound();
            }


            comment.content = commentDTo.content;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CommentExists(Id))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTo>> GetComment(int Id)
        {
            var comment = await _context.Comments.FindAsync(Id);

            if (comment == null)
            {
                return NotFound();
            }

            return commentToDTo(comment);
        }

        private bool CommentExists(int Id) => _context.Comments.Any(e => e.Id == Id);


        public static CommentDTo commentToDTo(Comments comment) => new CommentDTo
        {
            Id = comment.Id,
            uid = comment.uid,
            content = comment.content,
            articleId = comment.articleId,
            author = comment.author,
            createdAt = comment.createdAt,
            Answers = comment.Answers
        };
    }
}
