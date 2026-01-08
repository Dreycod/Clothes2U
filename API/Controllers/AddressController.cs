using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
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
    private readonly Clothes2UDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AddressController> _logger;
    private readonly IMapper _mapper;

    public AddressController(Clothes2UDbContext context, ICurrentUserService currentUserService, ILogger<AddressController> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
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
        var addresses = await _context.Adresses
            .Where(a => a.UtilisateurId == userId)
            .Select(a => _mapper.Map<AdresseDTO>(a))
            .ToListAsync();

        return Ok(addresses);
    }

    /// <summary>
    /// Obtenir une adresse par ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdresseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdresseDTO>> GetAddressById(int id)
    {
        var address = await _context.Adresses.FindAsync(id);
        
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Si c'est la première adresse, la définir par défaut
        var hasExistingAddresses = await _context.Adresses
            .AnyAsync(a => a.UtilisateurId == dto.UtilisateurId);
        
        

        var address = _mapper.Map<Adresse>(dto);

        if (!hasExistingAddresses)
        {
            address.IsDefault = true;
        }

        _context.Adresses.Add(address);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<AdresseDTO>(address);

        return CreatedAtAction(nameof(GetAddressById), new { id = address.AdresseId }, result);
    }

    /// <summary>
    /// Mettre à jour une adresse
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAdresseDTO dto)
    {
        var address = await _context.Adresses.FindAsync(id);
        
        if (address == null)
        {
            return NotFound();
        }
        
        _mapper.Map(dto, address);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Définir une adresse par défaut
    /// </summary>
    [HttpPut("{id}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        var address = await _context.Adresses.FindAsync(id);
        
        if (address == null)
        {
            return NotFound();
        }

        // Retirer le défaut de toutes les adresses de l'utilisateur
        var userAddresses = await _context.Adresses
            .Where(a => a.UtilisateurId == address.UtilisateurId)
            .ToListAsync();

        foreach (var addr in userAddresses)
        {
            addr.IsDefault = false;
        }

        // Définir la nouvelle adresse par défaut
        address.IsDefault = true;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Supprimer une adresse
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var address = await _context.Adresses.FindAsync(id);
        
        if (address == null)
        {
            return NotFound();
        }

        // Si c'était l'adresse par défaut, définir une autre comme par défaut
        if (address.IsDefault)
        {
            var anotherAddress = await _context.Adresses
                .Where(a => a.UtilisateurId == address.UtilisateurId && a.AdresseId != id)
                .FirstOrDefaultAsync();

            if (anotherAddress != null)
            {
                anotherAddress.IsDefault = true;
            }
        }

        _context.Adresses.Remove(address);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
