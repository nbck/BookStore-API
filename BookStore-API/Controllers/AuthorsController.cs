using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    ///     Endpoint used to interact with the Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository authorRepository;
        private readonly ILoggerService logger;
        private readonly IMapper mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            this.authorRepository = authorRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Gets all authors.
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                this.logger.LogInfo("Attempted get all authors");
                IList<Author> authors = await this.authorRepository.FindAll();
                var response = this.mapper.Map<IList<AuthorDTO>>(authors);
                this.logger.LogInfo("Successfully got all authors");
                return this.Ok(response);
            }
            catch (Exception e)
            {
                this.logger.LogError($"An error occurred: {e.Message} - {e.InnerException}\n{e.StackTrace}");
                return this.StatusCode(500, $"Something went wrong. Please ....\n{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        ///     Gets an author by ID.
        /// </summary>
        /// <param name="id">The ID of the author to get</param>
        /// <returns>An author's record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                this.logger.LogInfo($"Attempted to get author with id:{id}");
                Author author = await this.authorRepository.FindById(id);
                if (author == null)
                {
                    this.logger.LogWarn($"Author with id{id} was not found");
                    return this.NotFound();
                }

                var response = this.mapper.Map<AuthorDTO>(author);
                this.logger.LogInfo($"successfully got author with id:{id}");
                return this.Ok(response);
            }
            catch (Exception e)
            {
                return this.InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Creates an author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                this.logger.LogInfo("Author submission attempted");

                if (authorDTO == null)
                {
                    this.logger.LogWarn("Empty request was submitted");
                    return this.BadRequest(this.ModelState);
                }

                if (!this.ModelState.IsValid)
                {
                    this.logger.LogWarn("Author data was incomplete");
                    return this.BadRequest(this.ModelState);
                }

                var author = this.mapper.Map<Author>(authorDTO);
                bool isSuccess = await this.authorRepository.Create(author);

                if (!isSuccess)
                {
                    return this.InternalError("Author creation failed");
                }

                this.logger.LogInfo($"Author created successfully:{author.Firstname}, {author.Lastname}");

                return this.Created("Create", new {author});
            }
            catch (Exception e)
            {
                return this.InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Updates an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                this.logger.LogInfo("Author update attempted");

                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    this.logger.LogWarn($"Empty request was submitted - id: {id}");
                    return this.BadRequest();
                }

                bool isExists = await this.authorRepository.IsExists(id);
                if (!isExists)
                {
                    this.logger.LogWarn($"Author with id:{id} not found");
                    return this.NotFound();
                }

                if (!this.ModelState.IsValid)
                {
                    this.logger.LogWarn("Author data was incomplete");
                    return this.BadRequest(this.ModelState);
                }

                var author = this.mapper.Map<Author>(authorDTO);
                bool isSuccess = await this.authorRepository.Update(author);

                if (!isSuccess)
                {
                    return this.InternalError("Author update failed");
                }

                this.logger.LogInfo($"Author updated successfully:{author.Firstname}, {author.Lastname}");

                return this.NoContent();
            }
            catch (Exception e)
            {
                return this.InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Removes an author by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                this.logger.LogInfo("Author removal attempted");

                if (id < 1)
                {
                    this.logger.LogWarn($"Invalid id on delete- id: {id}");
                    return this.BadRequest();
                }

                bool isExists = await this.authorRepository.IsExists(id);
                if (!isExists)
                {
                    this.logger.LogWarn($"Author with id:{id} not found");
                    return this.NotFound();
                }

                Author author = await this.authorRepository.FindById(id);
                bool isSuccess = await this.authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return this.InternalError("Author removal failed");
                }

                this.logger.LogInfo($"Author with id:{id} removed successfully: {author.Firstname}, {author.Lastname}");

                return this.NoContent();
            }
            catch (Exception e)
            {
                return this.InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        private ObjectResult InternalError(string message)
        {
            this.logger.LogError(message);
            return this.StatusCode(500, "something went wrong. Please contact someone.");
        }
    }
}