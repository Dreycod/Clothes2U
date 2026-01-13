using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAdresseRepository _addressManager;
    private readonly ILogger<AddressController> _logger;
    private readonly IMapper _mapper;

    public AddressController(ICurrentUserService currentUserService, IAdresseRepository addressManager, ILogger<AddressController> logger, IMapper mapper)
    {
        _logger = logger;
        _addressManager = addressManager;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtenir toutes les adresses d'un utilisateur
    /// </summary>
    [Authorize]
    [HttpGet("user")]
    [ProducesResponseType(typeof(List<AdresseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AdresseDTO>>> GetUserAddresses()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        var addresses = await _addressManager.GetUserAddressesAsync(userId);
        return Ok(addresses.Select(a => _mapper.Map<AdresseDTO>(a)).ToList());
    }

    /// <summary>
    /// Obtenir une adresse par ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdresseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdresseDTO>> GetAddressById(int id)
    {
        var address = await _addressManager.GetByIdAsync(id);
        
        if (address == null)
        {
            return NotFound();
        }

        var dto = _mapper.Map<AdresseDTO>(address);
        return Ok(dto);
    }

    /// <summary>
    /// Créer une nouvelle adresse
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AdresseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdresseDTO>> CreateAddress([FromBody] CreateAdresseDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = _mapper.Map<Adresse>(dto);
            
            await _addressManager.CreateAsync(address);
            
            var result = _mapper.Map<AdresseDTO>(address);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.AdresseId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Mettre à jour une adresse
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAdresseDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ✅ FIX: Map the DTO and set the ID
            var address = _mapper.Map<Adresse>(dto);
            address.AdresseId = id;

            // ✅ FIX: Call UpdateAsync with just the address object
            await _addressManager.UpdateAsync(address);
            
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Address not found: {Id}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Définir une adresse par défaut
    /// </summary>
    [HttpPut("{id}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        try
        {
            await _addressManager.SetDefaultAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Address not found: {Id}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting default address {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Supprimer une adresse
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        try
        {
            await _addressManager.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Address not found: {Id}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}