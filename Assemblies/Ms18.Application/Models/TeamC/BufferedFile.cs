﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ms18.Database;
using Ms18.Database.Models.TeamC.Receipt;
using System.ComponentModel.DataAnnotations;
using Ms18.Application.Utilities;

namespace Ms18.Application.Models.TeamC;
public class BufferedSingleFileUploadDbModel : PageModel
{
    private readonly MaasgroepContext _context;
    private readonly long _fileSizeLimit;
    private readonly string[] _permittedExtensions = { ".txt" };

    public BufferedSingleFileUploadDbModel(MaasgroepContext context,
        IConfiguration config)
    {
        _context = context;
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
    }

    [BindProperty]
    public BufferedSingleFileUploadDb FileUpload { get; set; }

    public string Result { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        // Perform an initial check to catch FileUpload class
        // attribute violations.
        if (!ModelState.IsValid)
        {
            Result = "Please correct the form.";

            return Page();
        }

        var formFileContent =
            await FileHelpers.ProcessFormFile<BufferedSingleFileUploadDb>(
                FileUpload.FormFile, ModelState, _permittedExtensions,
                _fileSizeLimit);

        // Perform a second check to catch ProcessFormFile method
        // violations. If any validation check fails, return to the
        // page.
        if (!ModelState.IsValid)
        {
            Result = "Please correct the form.";

            return Page();
        }

        // **WARNING!**
        // In the following example, the file is saved without
        // scanning the file's contents. In most production
        // scenarios, an anti-virus/anti-malware scanner API
        // is used on the file before making the file available
        // for download or for use by other systems. 
        // For more information, see the topic that accompanies 
        // this sample.

        var photo = new ReceiptPhoto
        {
            Bytes = formFileContent,
            DateTimeDeleted = DateTime.UtcNow,
            fileName = FileUpload.FormFile.FileName,
            fileExtension = "png"
            //UntrustedName = FileUpload.FormFile.FileName,
            //Note = FileUpload.Note,
            //Size = FileUpload.FormFile.Length,
            //UploadDT = DateTime.UtcNow
        };

        _context.ReceiptPhotos.Add(photo);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }


    public class BufferedSingleFileUploadDb
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        [Display(Name = "Note")]
        [StringLength(50, MinimumLength = 0)]
        public string Note { get; set; }
    }
}
