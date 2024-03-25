using Maasgroep.Database;
using Maasgroep.Database.Context.Tables.PhotoAlbum;
using Microsoft.EntityFrameworkCore;
using ms18_applicatie.Interfaces;
using ms18_applicatie.Models.team_d;

namespace ms18_applicatie.Repository.PhotoAlbum;

public class PhotoRepository : IPhotoRepository
{
    private readonly MaasgroepContext _context;

    public PhotoRepository(MaasgroepContext context)
    {
        _context = context;
    }

    public async Task<Guid?> AddPhoto(long? uploaderId, bool needsApproval, PhotoUploadModel photoUploadModel)
    {
        var imageBytes = Convert.FromBase64String(photoUploadModel.ImageData);

        var photo = new Photo(
            uploaderId: uploaderId,
            imageData: imageBytes,
            contentType: photoUploadModel.ContentType,
            needsApproval: needsApproval,
            albumLocationId: photoUploadModel.AlbumId
            )
        {
            Title = photoUploadModel.Title,
            TakenOn = photoUploadModel.TakenOn,
            Location = photoUploadModel.Location,
        };

        _context.Photos.Add(photo);
        var affectedRows = await _context.SaveChangesAsync();

        if (affectedRows > 0 && photo.Id != Guid.Empty) return photo.Id;

        return null;
    }

    public async Task<bool> DeletePhoto(Guid id)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo == null) return false;

        _context.Photos.Remove(photo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PhotoViewModel?> GetPhotoViewModelById(Guid id)
    {
        return await _context.Photos
            .Where(p => p.Id == id)
            .Include(p => p.Likes)
            .Select(p => new PhotoViewModel
            {
                Id = p.Id,
                UploaderId = p.UploaderId,
                UploadDate = p.UploadDate,
                Title = p.Title,
                ImageBase64 = Convert.ToBase64String(p.ImageData),
                ContentType = p.ContentType,
                TakenOn = p.TakenOn,
                Location = p.Location,
                AlbumLocationId = p.AlbumLocationId,
                LikesCount = p.Likes.Count(),
                NeedsApproval = p.NeedsApproval,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<Photo> Photos, int TotalCount)> GetPhotosByAlbumId(Guid albumId, int pageNumber, int pageSize, bool showUnapproved)
    {
        var query = _context.Photos.Where(p => p.AlbumLocationId == albumId);

        if (!showUnapproved)
        {
            query = query.Where(p => !p.NeedsApproval);
        }

        query = query.Include(p => p.Likes);

        var totalCount = await query.CountAsync();
        var photos = await query.OrderBy(p => p.UploadDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (photos, totalCount);
    }

    public async Task<(IEnumerable<Photo> Photos, int TotalCount)> GetUnapprovedPhotos(int pageNumber, int pageSize)
    {
        var query = _context.Photos.Where(p => p.NeedsApproval);

        var totalCount = await query.CountAsync();
        var photos = await query
            .OrderBy(p => p.UploadDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (photos, totalCount);
    }

    public async Task UpdatePhoto(Photo photo)
    {
        _context.Entry(photo).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<Photo?> GetPhotoById(Guid id)
    {
        return await _context.Photos
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

