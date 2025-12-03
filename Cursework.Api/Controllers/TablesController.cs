using Cursework.Application.Interfaces;
using Cursework.Domains.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cursework.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableService _tables;

    public TablesController(ITableService tables) => _tables = tables;

    [HttpGet]
    public async Task<ActionResult<List<DiningTable>>> GetAll(CancellationToken ct)
    {
        var items = await _tables.GetAllAsync(ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<DiningTable>> Create([FromBody] DiningTable table, CancellationToken ct)
    {
        try
        {
            var created = await _tables.AddAsync(table, ct);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DiningTable table, CancellationToken ct)
    {
        if (id != table.Id)
            return BadRequest(new { message = "id в URL не совпадает с моделью" });

        try
        {
            await _tables.UpdateAsync(table, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _tables.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
