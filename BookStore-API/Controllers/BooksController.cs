using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    ///     Endpoint used to interact with the Books in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper, IWebHostEnvironment env)
        {
            this._bookRepository = bookRepository;
            this._logger = logger;
            this._mapper = mapper;
            this._env = env;
        }

        /// <summary>
        ///     Gets all books.
        /// </summary>
        /// <returns>List of books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            string location = this.GetControllerActionNames();

            try
            {
                this._logger.LogInfo($"{location}: Attempted get all books");
                IList<Book> books = await this._bookRepository.FindAll();
                IList<BookDTO> response = this._mapper.Map<IList<BookDTO>>(books);
                foreach (BookDTO bookDto in response)
                {
                    await this.LoadImageData(bookDto);
                }

                this._logger.LogInfo($"{location}: Successfully got all books");
                return this.Ok(response);
            }
            catch (Exception e)
            {
                this._logger.LogError($"{location}: An error occurred: {e.Message} - {e.InnerException}\n{e.StackTrace}");
                return this.StatusCode(500, $"{location}: Something went wrong. Please ....\n{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        ///     Gets a book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to get</param>
        /// <returns>A book's record</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            string location = this.GetControllerActionNames();
            try
            {
                this._logger.LogInfo($"{location}: Attempted to get book with id:{id}");
                Book book = await this._bookRepository.FindById(id);
                if (book == null)
                {
                    this._logger.LogWarn($"{location}: Book with id:{id} was not found");
                    return this.NotFound();
                }

                var bookDto = this._mapper.Map<BookDTO>(book);

                await this.LoadImageData(bookDto);

                this._logger.LogInfo($"{location}: successfully got book with id:{id}");
                return this.Ok(bookDto);
            }
            catch (Exception e)
            {
                return this.InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Creates a book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            string location = this.GetControllerActionNames();
            try
            {
                this._logger.LogInfo($"{location}: book submission attempted");

                if (bookDTO == null)
                {
                    this._logger.LogWarn($"{location}: Empty request was submitted");
                    return this.BadRequest(this.ModelState);
                }

                if (!this.ModelState.IsValid)
                {
                    this._logger.LogWarn($"{location}: book data was incomplete");
                    return this.BadRequest(this.ModelState);
                }

                var book = this._mapper.Map<Book>(bookDTO);
                bool isSuccess = await this._bookRepository.Create(book);

                if (!isSuccess)
                {
                    return this.InternalError($"{location}: Book creation failed");
                }

                if (!string.IsNullOrEmpty(bookDTO.ImageName))
                {
                    var imgPath = GetImagePath(bookDTO.ImageName);
                    byte[] imageBytes = GetImageFromString(bookDTO);
                    await System.IO.File.WriteAllBytesAsync(imgPath, imageBytes);
                }

                this._logger.LogInfo($"{location}: Book created successfully:{book.Title}");
                this._logger.LogInfo($"{location}: {book}");

                return this.Created("Create", new {book});
            }
            catch (Exception e)
            {
                return this.InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Updates a book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            string location = this.GetControllerActionNames();
            try
            {
                this._logger.LogInfo($"{location}: Book update attempted");

                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    this._logger.LogWarn($"{location}: Empty request was submitted - id: {id}");
                    return this.BadRequest();
                }

                bool isExists = await this._bookRepository.IsExists(id);
                if (!isExists)
                {
                    this._logger.LogWarn($"{location}: Book with id:{id} not found");
                    return this.NotFound();
                }

                if (!this.ModelState.IsValid)
                {
                    this._logger.LogWarn($"{location}: Book data was incomplete");
                    return this.BadRequest(this.ModelState);
                }

                var oldImageName = await _bookRepository.GetImageFileName(id);
                var book = this._mapper.Map<Book>(bookDTO);
                bool isSuccess = await this._bookRepository.Update(book);
                if (!isSuccess)
                {
                    return this.InternalError($"{location}: Book update failed for record with id:{id}");
                }

                if (!bookDTO.ImageName.Equals(oldImageName))
                {
                    var oldImagePath = GetImagePath(oldImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                if (!string.IsNullOrEmpty(bookDTO.ImageName))
                {
                    byte[] imageBytes = GetImageFromString(bookDTO);
                    await System.IO.File.WriteAllBytesAsync(GetImagePath(bookDTO.ImageName), imageBytes);
                }

                this._logger.LogInfo($"{location}: Book updated successfully:{book.Id}:{book.Title}");

                return this.NoContent();
            }
            catch (Exception e)
            {
                return this.InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        ///     Removes a book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            string location = this.GetControllerActionNames();
            try
            {
                this._logger.LogInfo($"{location}: Book removal attempted");

                if (id < 1)
                {
                    this._logger.LogWarn($"{location}: Invalid id on delete- id: {id}");
                    return this.BadRequest();
                }

                bool isExists = await this._bookRepository.IsExists(id);
                if (!isExists)
                {
                    this._logger.LogWarn($"{location}: Book with id:{id} not found");
                    return this.NotFound();
                }

                Book book = await this._bookRepository.FindById(id);
                bool isSuccess = await this._bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return this.InternalError($"{location}: Book removal failed");
                }

                this._logger.LogInfo($"{location}: Book with id:{id} removed successfully: {book.Title}");

                return this.NoContent();
            }
            catch (Exception e)
            {
                return this.InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            string controller = this.ControllerContext.ActionDescriptor.ControllerName;
            string action = this.ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            this._logger.LogError(message);
            return this.StatusCode(500, "something went wrong. Please contact someone.");
        }

        private string GetImagePath(string fileName) => $"{_env.ContentRootPath}\\uploads\\{fileName}";

        private static byte[] GetImageFromString(BookCreateDTO bookDTO) => Convert.FromBase64String(bookDTO.ImageData);

        private static byte[] GetImageFromString(BookUpdateDTO bookDTO) => Convert.FromBase64String(bookDTO.ImageData);


        private async Task LoadImageData(BookDTO bookDto)
        {
            if (!string.IsNullOrEmpty(bookDto.ImageName))
            {
                var imgPath = this.GetImagePath(bookDto.ImageName);
                if (System.IO.File.Exists(imgPath))
                {
                    byte[] imgBytes = await System.IO.File.ReadAllBytesAsync(imgPath);
                    bookDto.ImageData = Convert.ToBase64String(imgBytes);
                }
            }
        }
    }
}