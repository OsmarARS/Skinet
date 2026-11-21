using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository productRepository) : ControllerBase
{
   [HttpGet]
   public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
   {
      return Ok(await productRepository.GetProductsAsync(brand, type, sort));
   }

   [HttpGet("{id:int}")]
   public async Task<ActionResult<Product>> GetProduct(int id)
   {
      var product = await productRepository.GetProductByIdAsync(id);

      if (product is null) return NotFound();

      return product;
   }

   [HttpPost]
   public async Task<ActionResult<Product>> CreateProduct(Product product)
   {
      productRepository.AddProduct(product);
      var isSaved = await productRepository.SaveChangesAsync();

      if (isSaved)
      {
         return CreatedAtAction(nameof(GetProduct), new { id = product.Id });
      }

      return BadRequest("Problem while creating product");
   }

   [HttpPut("{id:int}")]
   public async Task<ActionResult> UpdateProduct(int id, Product product)
   {
      if (product.Id != id || !ProductExists(id))
      {
         return BadRequest("Cannot update this product");
      }

      productRepository.UpdateProduct(product);
      var isSaved = await productRepository.SaveChangesAsync();

      if (isSaved)
      {
         return NoContent();
      }

      return BadRequest("Problem while updating product");
   }

   [HttpDelete("{id:int}")]
   public async Task<ActionResult> DeleteProduct(int id)
   {
      var product = await productRepository.GetProductByIdAsync(id);

      if (product is null) return NotFound();

      productRepository.DeleteProduct(product);
      var isSaved = await productRepository.SaveChangesAsync();

      if (isSaved)
      {
         return NoContent();
      }

      return BadRequest("Problem while deleting product");
   }

   [HttpGet("brands")]
   public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
   {
      return Ok(await productRepository.GetBrandsAsync());
   }
   
   [HttpGet("types")]
   public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
   {
      return Ok(await productRepository.GetTypesAsync());
   }

   private bool ProductExists(int id)
   {
      return productRepository.ProductExists(id);
   }
}