using GerenciarAssinaturas.Application.DTOs;
using GerenciarAssinaturas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciarAssinaturas.Controllers;

[ApiController]
[Route("assinantes")]
public class AssinantesController : ControllerBase
{
    private readonly IAssinanteService _service;

    public AssinantesController(IAssinanteService service)
    {
        _service = service;
    }

    /// <summary>Cria um novo assinante.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AssinanteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarAssinanteRequest request)
    {
        var response = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
    }

    /// <summary>Lista assinantes ativos com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<AssinanteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var response = await _service.ListarAtivosAsync(pagina, tamanhoPagina);
        return Ok(response);
    }

    /// <summary>Retorna os detalhes de um assinante ativo.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AssinanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var response = await _service.ObterPorIdAsync(id);
        return Ok(response);
    }

    /// <summary>Edita os dados de um assinante ativo.</summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(AssinanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarAssinanteRequest request)
    {
        var response = await _service.EditarAsync(id, request);
        return Ok(response);
    }

    /// <summary>Desativa um assinante (soft delete).</summary>
    [HttpPatch("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Desativar(Guid id)
    {
        await _service.DesativarAsync(id);
        return NoContent();
    }

    /// <summary>Remove fisicamente um assinante.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Remover(Guid id)
    {
        await _service.RemoverAsync(id);
        return NoContent();
    }
}
