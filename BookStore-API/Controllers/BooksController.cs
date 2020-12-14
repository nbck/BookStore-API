﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Books in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository bookRepository;
        private readonly ILoggerService logger;
        private readonly IMapper mapper;

        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper)
        {
            this.bookRepository = bookRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all books.
        /// </summary>
        /// <returns>List of books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = this.GetControllerActionNames();
            
            try
            {
                this.logger.LogInfo($"{location}: Attempted get all books");
                var books = await this.bookRepository.FindAll();
                var response = this.mapper.Map<IList<BookDTO>>(books);
                this.logger.LogInfo($"{location}: Successfully got all books");
                return this.Ok(response);
            }
            catch (Exception e)
            {
                this.logger.LogError($"{location}: An error occured: {e.Message} - {e.InnerException}\n{e.StackTrace}");
                return StatusCode(500, $"{location}: Something went wrong. Please ....\n{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Gets a book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to get</param>
        /// <returns>A book's record</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = this.GetControllerActionNames();
            try
            {
                this.logger.LogInfo($"{location}: Attempted to get book with id:{id}");
                var book = await this.bookRepository.FindById(id);
                if (book == null)
                {
                    this.logger.LogWarn($"{location}: Book with id:{id} was not found");
                    return this.NotFound();
                }

                var response = this.mapper.Map<BookDTO>(book);
                this.logger.LogInfo($"{location}: successfully got book with id:{id}");
                return this.Ok(response);
            }
            catch (Exception e)
            {
                return this.InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Creates a book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = this.GetControllerActionNames();
            try
            {
                this.logger.LogInfo($"{location}: book submission attempted");
                
                if (bookDTO == null)
                {
                    this.logger.LogWarn($"{location}: Empty request was submitted");
                    return this.BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    this.logger.LogWarn($"{location}: book data was incomplete");
                    return this.BadRequest(ModelState);
                }

                var book = this.mapper.Map<Book>(bookDTO);
                var isSuccess = await this.bookRepository.Create(book);

                if (!isSuccess)
                {
                    return InternalError($"{location}: Book creation failed");
                }

                this.logger.LogInfo($"{location}: Book created successfully:{book.Title}");
                this.logger.LogInfo($"{location}: {book}");
                
                return Created("Create", new {book});
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Updates a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            var location = this.GetControllerActionNames();
            try
            {
                this.logger.LogInfo($"{location}: Book update attempted");
                
                if (id<1 || bookDTO == null|| id!= bookDTO.Id)
                {
                    this.logger.LogWarn($"{location}: Empty request was submitted - id: {id}");
                    return this.BadRequest();
                }
                
                bool isExists = await this.bookRepository .IsExists(id);
                if (!isExists)
                {
                    this.logger.LogWarn($"{location}: Book with id:{id} not found");
                    return this.NotFound();
                }

                if (!ModelState.IsValid)
                {
                    this.logger.LogWarn($"{location}: Book data was incomplete");
                    return this.BadRequest(ModelState);
                }

                var book = this.mapper.Map<Book>(bookDTO);
                var isSuccess = await this.bookRepository.Update(book);

                if (!isSuccess)
                {
                    return InternalError($"{location}: Book update failed for record with id:{id}");
                }

                this.logger.LogInfo($"{location}: Book updated successfully:{book.Id}:{book.Title}");

                return this.NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Removes a book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = this.GetControllerActionNames();
            try
            {
                this.logger.LogInfo($"{location}: Book removal attempted");

                if (id < 1)
                {
                    this.logger.LogWarn($"{location}: Invalid id on delete- id: {id}");
                    return this.BadRequest();
                }

                bool isExists = await this.bookRepository.IsExists(id);
                if (!isExists)
                {
                    this.logger.LogWarn($"{location}: Book with id:{id} not found");
                    return this.NotFound();
                }
                
                var book = await this.bookRepository.FindById(id);
                var isSuccess = await this.bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Book removal failed");
                }

                this.logger.LogInfo($"{location}: Book with id:{id} removed successfully: {book.Title}");

                return this.NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }
        private ObjectResult InternalError(string message)
        {
            this.logger.LogError(message);
            return StatusCode(500, "something went wrong. Please contact someone.");
        }
    }
}