using Maasgroep.Database;
using Maasgroep.Database.Context.Tables.PhotoAlbum;
using Microsoft.EntityFrameworkCore;
using ms18_applicatie.Interfaces;
using ms18_applicatie.Models.team_d;

namespace ms18_applicatie.Repository.PhotoAlbum;

public class AlbumRepository : IAlbumRepository
{
    private readonly MaasgroepContext _context;

    public AlbumRepository(MaasgroepContext context)
    {
        _context = context;
    }

    public async Task<bool> AlbumExists(string albumName, Guid? parentAlbumId)
    {
        return await _context.Albums.AnyAsync(a => a.Name == albumName && a.ParentAlbumId == parentAlbumId);
    }

    public async Task<Guid?> AddAlbum(AlbumCreateModel albumCreateModel)
    {
        var album = new Album(albumCreateModel.Name)
        {
            ParentAlbumId = albumCreateModel.ParentAlbumId,
            Year = albumCreateModel.Year
        };

        _context.Albums.Add(album);
        var affectedRows = await _context.SaveChangesAsync();

        if (affectedRows > 0 && album.Id != Guid.Empty) return album.Id;

        return null;
    }

    public async Task<AlbumViewModel?> GetAlbumViewModelById(Guid id)
    {
        return await _context.Albums
            .Where(a => a.Id == id)
            .Include(a => a.ChildAlbums)
            .ThenInclude(ca => ca.CoverPhoto)
            .Include(a => a.Photos)
            .Select(a => new AlbumViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Year = a.Year,
                CoverPhotoId = a.CoverPhotoId,
                ParentAlbumId = a.ParentAlbumId,
                PhotoCount = a.Photos.Count(),
                ChildAlbums = a.ChildAlbums.Select(ca => new ChildAlbumViewModel
                {
                    Id = ca.Id,
                    Name = ca.Name,
                    CoverPhotoId = ca.CoverPhotoId
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Album?> GetAlbumById(Guid id)
    {
        return await _context.Albums
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<AlbumViewModel>> GetAllAlbums()
    {
        return await _context.Albums
            .Include(a => a.Photos)
            .Select(a => new AlbumViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Year = a.Year,
                CoverPhotoId = a.CoverPhotoId,
                ParentAlbumId = a.ParentAlbumId,
                PhotoCount = a.Photos.Count()
            })
            .ToListAsync();
    }

    public async Task UpdateAlbum(Album album)
    {
        _context.Albums.Update(album);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAlbum(Guid id)
    {
        var album = await _context.Albums.FindAsync(id);

        if (album == null) return false;

        _context.Albums.Remove(album);
        await _context.SaveChangesAsync();
        return true;
    }
}

