using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[Route("api/[controller]")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class BaseController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    private static readonly List<string> _items = ["Item1", "Item2", "Item3"];

    [HttpGet]
    public ActionResult<IEnumerable<string>> GetAll()
    {
        return Ok(_items);
    }

    // GET: api/sample/1
    [HttpGet("{id:int}")]
    public ActionResult<string> GetById(int id)
    {
#pragma warning disable IDE0046 // Convert to conditional expression
        if (id < 0 || id >= _items.Count)
        {
            return NotFound("Item not found");
        }
#pragma warning restore IDE0046 // Convert to conditional expression

        return Ok(_items[id]);
    }

    // POST: api/sample
    [HttpPost]
    public ActionResult<string> Create([FromBody] string newItem)
    {
        _items.Add(newItem);
        return CreatedAtAction(nameof(GetById), new { id = _items.Count - 1 }, newItem);
    }

    // PUT: api/sample/1
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] string updatedItem)
    {
        if (id < 0 || id >= _items.Count)
        {
            return NotFound("Item not found");
        }

        _items[id] = updatedItem;
        return NoContent();
    }

    // DELETE: api/sample/1
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (id < 0 || id >= _items.Count)
        {
            return NotFound("Item not found");
        }

        _items.RemoveAt(id);
        return NoContent();
    }
}
