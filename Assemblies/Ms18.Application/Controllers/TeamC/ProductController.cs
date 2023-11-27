﻿using Microsoft.AspNetCore.Mvc;
using Ms18.Database;
using Ms18.Database.Models.TeamC.Stock;
using Ms18.Database.Repository.TeamC.ViewModel;

namespace Ms18.Application.Controllers.TeamC;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductController : BaseController
{
    public ProductController(MaasgroepContext context) : base(context)
    {
    }

    [HttpGet]
    [ActionName("productGet")]
    public IActionResult ProductGet()
    {
        if (_currentUser == null) // Toegangscontrole
            return Forbidden();

        var all = _context.Product
            .Select(dbRec => new ProductViewModel(dbRec))
            .ToList()
            .Select(AddForeignData)
            .ToList()
            ;

        return Ok(all);
    }

    [HttpGet("{id}")]
    [ActionName("productGetById")]
    public IActionResult ProductGetById(long id)
    {
        if (_currentUser == null) // Toegangscontrole
            return Forbidden();

        var dbRec = _context.Product
            .Where(dbRec => dbRec.Id == id)
            .Select(dbRec => new ProductViewModel(dbRec))
            .FirstOrDefault();

        if (dbRec == null)
            return NotFound(new
            {
                status = 404,
                message = "Product niet gevonden"
            });

        return Ok(AddForeignData(dbRec));
    }
    
    [HttpPost]
    [ActionName("productCreate")]
    public IActionResult ProductCreate([FromBody] ProductViewModel vm)
    {
        
        if (_currentUser == null) // Toegangscontrole
            return Forbidden();

        // Validate the request body
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Invalid request body"
            });
        }
        
        var createdProduct = new Product
        {
            Name = vm.Name,
            MemberCreatedId = _currentUser.Id
        };

        try
        {
            _context.Product.Add(createdProduct);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                status = 422,
                message = "Kon product niet aanmaken",
            });
        }

        return Ok(new
        {
            status = 200,
            message = "Product aangemaakt",
            product = AddForeignData(new ProductViewModel(createdProduct))
        });
    }
    
    [HttpPut("{id}")]
    [ActionName("productUpdate")]
    public IActionResult ProductUpdate(long id, [FromBody] ProductViewModel vm)
    {
        if (_currentUser == null) // Toegangscontrole
            return Forbidden();

        // Validate the request body
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Invalid request body"
            });
        }
        
        if (id != vm.Id)
            return BadRequest(new
            {
                status = 400,
                message = "Invalid request body, ID in URL does not match ID in request body"
            });

        var existingProduct = _context.Product
            .FirstOrDefault(dbRec => dbRec.Id == id);

        if (existingProduct == null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Product niet gevonden"
            });
        }

        if (ProductsAreEqual(existingProduct, vm))
        {
            // If the data is the same, return a response indicating no update was performed
            return Ok(new {
                status = 200,
                message = "Product niet gewijzigd"
            });
        }

        if (vm.Name != null)
        {
            existingProduct.Name = vm.Name;
        }

        existingProduct.MemberModifiedId = _currentUser.Id;
        existingProduct.DateTimeModified = DateTime.UtcNow;
        
        _context.Product.Update(existingProduct);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ActionName("productDelete")]
    public IActionResult ProductDelete(long id)
    {
        if (_currentUser == null) // Toegangscontrole
            return Forbidden();

        var existingProduct = _context.Product
            .Where(dbRec => dbRec.Id == id)
            .FirstOrDefault();

        if (existingProduct == null)
            return NotFound(new
            {
                status = 404,
                message = "Product niet gevonden"
            });

        try
        {
            _context.Product.Remove(existingProduct);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                status = 409,
                message = "Kon product niet verwijderen",
            });
        }

        return NoContent();
    }

    private static bool ProductsAreEqual(Product existingProduct, ProductViewModel vm)
    {
        bool nameEqual = (vm.Name == null)
                         || (existingProduct.Name == vm.Name);

        return nameEqual;
    }
    
}