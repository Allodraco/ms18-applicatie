using Maasgroep.Database.Context.Tables.PhotoAlbum;
using Microsoft.AspNetCore.Mvc;
using ms18_applicatie.Attributes;
using ms18_applicatie.Interfaces;
using ms18_applicatie.Models.team_d;

namespace ms18_applicatie.Controllers.team_d;

[ApiController]
[Route("api/albums")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumRepository _albumRepository;
    private readonly ILogger<AlbumsController> _logger;

    public AlbumsController(IAlbumRepository albumRepository, ILogger<AlbumsController> logger)
    {
        _albumRepository = albumRepository;
        _logger = logger;
    }

    [HttpPost]
    [PhotoAlbumAuthorization("photoAlbum.edit")]
    [ProducesResponseType(typeof(CreateResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateAlbum ([FromBody] AlbumCreateModel albumCreateModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var exists = await _albumRepository.AlbumExists(albumCreateModel.Name, albumCreateModel.ParentAlbumId);
            if (exists) return BadRequest($"An album with the name '{albumCreateModel.Name}' already exists in the specified parent album.");

            var albumId = await _albumRepository.AddAlbum(albumCreateModel);

            if (albumId == null || albumId == Guid.Empty)
            {
                _logger.LogError($"Failed to create album with Name {albumCreateModel.Name}");
                return StatusCode(500, "An error occurred while creating the album.");
            }

            return CreatedAtRoute("GetAlbum", new { id = albumId }, new CreateResponseModel {Id = (Guid)albumId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating the album.");
            return StatusCode(500, "An error occurred while creating the album.");
        }
    }

    [HttpGet("{id}", Name = "GetAlbum")]
    [PhotoAlbumAuthorization("photoAlbum")]
    [ProducesResponseType(typeof(AlbumViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AlbumViewModel>> GetAlbum([FromRoute]Guid id)
    {
        // This doesn't get the photos for the album that is in the PhotosController since it has to be paginated.
        try
        {
            var albumViewModel = await _albumRepository.GetAlbumViewModelById(id);
            if (albumViewModel == null)
            {
                return NotFound($"Album with ID {id} not found.");
            }

            return Ok(albumViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving album with ID {id}.");
            return StatusCode(500, "An internal error occurred.");
        }
    }

    [HttpGet]
    [PhotoAlbumAuthorization("photoAlbum")]
    [ProducesResponseType(typeof(IEnumerable<AlbumViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AlbumViewModel>>> GetAllAlbums()
    {
        try
        {
            var albums = await _albumRepository.GetAllAlbums();
            return Ok(albums);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving albums.");
            return StatusCode(500, "An internal error occurred.");
        }
    }

    [HttpPut("{id}")]
    [PhotoAlbumAuthorization("photoAlbum.edit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAlbum([FromRoute]Guid id, [FromBody] AlbumUpdateModel updatedAlbum)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var album = await _albumRepository.GetAlbumById(id);
            if (album == null)
            {
                return NotFound($"Album with ID {id} not found.");
            }

            album.Name = updatedAlbum.Name;
            album.Year = updatedAlbum.Year;
            album.ParentAlbumId = updatedAlbum.ParentAlbumId;
            album.CoverPhotoId = updatedAlbum.CoverPhotoId;

            await _albumRepository.UpdateAlbum(album);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating album with ID {id}.");
            return StatusCode(500, "An internal error occurred.");
        }
    }

    [HttpDelete("{id}")]
    [PhotoAlbumAuthorization("photoAlbum.edit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAlbum([FromRoute] Guid id)
    {
        try
        {
            var deleteSuccess = await _albumRepository.DeleteAlbum(id);

            if (!deleteSuccess) return NotFound($"Album with ID {id} not found.");
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting album with ID {id}.");
            return StatusCode(500, "An internal error occurred.");
        }
    }
}

