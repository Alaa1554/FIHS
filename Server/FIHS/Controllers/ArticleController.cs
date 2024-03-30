using Microsoft.AspNetCore.Mvc;
using CarShopAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using FIHS.Dtos.ArticleDtos;
using FIHS.Interfaces.IArticle;

namespace FIHS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IArticleInteractionService _articleInteractionService;

        public ArticleController(IArticleService articleService, IArticleInteractionService articleInteractionService)
        {
            _articleService = articleService;
            _articleInteractionService = articleInteractionService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllArticlesAsync([FromQuery] int offset, [FromQuery] int limit)
        {
            var articles = await _articleService.GetAllArticlesAsync(offset, limit);

            return Ok(articles);
        }

        [HttpGet("get/{articleId}")]
        public async Task<IActionResult> GetArticleAsync(int articleId)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _articleService.GetArticleAsync(articleId, refreshToken);

            return result.Succeeded ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string query, [FromQuery] int offset, [FromQuery] int limit)
        {
            var articles = await _articleService.SearchAsync(query, offset, limit);

            return Ok(articles);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddArticleAsync([FromForm] AddArticleDto articleDto)
        {
            var modelValidation = ValidationHelper<AddArticleDto>.Validate(articleDto);
            if (!string.IsNullOrEmpty(modelValidation))
                return BadRequest(modelValidation);

            var article = await _articleService.AddArticleAsync(articleDto);

            return Ok(article);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-tag")]
        public async Task<IActionResult> AddTagAsync([FromBody] TagDto tagDto)
        {
            var modelValidation = ValidationHelper<TagDto>.Validate(tagDto);
            if (!string.IsNullOrEmpty(modelValidation))
                return BadRequest(modelValidation);

            var tag = await _articleService.AddTagAsync(tagDto);

            return Ok(tag);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-section")]
        public async Task<IActionResult> AddSectionAsync([FromBody] AddSectionDto sectionDto)
        {
            var modelValidation = ValidationHelper<AddSectionDto>.Validate(sectionDto);
            if (!string.IsNullOrEmpty(modelValidation))
                return BadRequest(modelValidation);

            return Ok(await _articleService.AddSectionAsync(sectionDto));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{articleId}")]
        public async Task<IActionResult> UpdateArticleAsync(int articleId, [FromForm] UpdateArticleDto articleDto)
        {
            var modelValidation = ValidationHelper<UpdateArticleDto>.Validate(articleDto);
            if (!string.IsNullOrEmpty(modelValidation))
                return BadRequest(modelValidation);

            var article = await _articleService.UpdateArticleAsync(articleId, articleDto);

            if (article == null)
                return BadRequest("��� ������ ��� �����");

            return Ok(article);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-section/{sectionId}")]
        public async Task<IActionResult> UpdateSectionAsync(int sectionId, [FromBody] UpdateSectionDto sectionDto)
        {
            var modelValidation = ValidationHelper<UpdateSectionDto>.Validate(sectionDto);
            if (!string.IsNullOrEmpty(modelValidation))
                return BadRequest(modelValidation);

            var section = await _articleService.UpdateSectionAsync(sectionId, sectionDto);

            if (section == null)
                BadRequest("��� ����� ��� �����");

            return Ok(section);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{articleId}")]
        public async Task<IActionResult> DeleteArticleAsync(int articleId)
        {
            var result = await _articleService.DeleteArticleAsync(articleId);

            if (!result)
                BadRequest("��� ������ ��� �����");

            return Ok("�� ��� ������ �����");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-tag/{tagId}")]
        public async Task<IActionResult> DeleteTagAsync(int tagId)
        {
            var result = await _articleService.DeleteTagAsync(tagId);

            if (!result)
                BadRequest("��� �����");

            return Ok("�� ����� �����");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-section/{sectionId}")]
        public async Task<IActionResult> DeleteSectionAsync(int sectionId)
        {
            var result = await _articleService.DeleteSectionAsync(sectionId);

            if (!result)
                BadRequest("��� ����� ��� �����");

            return Ok("�� ��� ����� �����");
        }

        [Authorize]
        [HttpGet("like/{articleId}")]
        public async Task<IActionResult> LikeAsync(int articleId)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Token is required!");

            var result = await _articleInteractionService.LikeAsync(articleId, refreshToken);

            return result.Succeeded ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}