using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDataRepository<Annonce, int> _annonceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderRepository orderRepository,
        IDataRepository<Annonce, int> annonceRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<OrderController> logger)
    {
        _orderRepository = orderRepository;
        _annonceRepository = annonceRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Créer une nouvelle commande
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] CreateOrderDTO dto)
    {
        try
        {
            _logger.LogInformation($"Creating order for annonce {dto.AnnonceId}");

            // Vérifier que l'annonce existe et est disponible
            var annonce = await _annonceRepository.GetByIdAsync(dto.AnnonceId);
            if (annonce == null)
            {
                return NotFound("Annonce not found");
            }

            if (annonce.Etat.NomEtat != "Disponible")
            {
                return BadRequest("Annonce is not available");
            }

            // Créer la commande
            var commande = new Commande
            {
                AnnonceId = dto.AnnonceId,
                AcheteurId = dto.AcheteurId,
                VendeurId = dto.VendeurId,
                AdresseLivraisonId = dto.AdresseLivraisonId,
                MontantTotal = dto.MontantTotal,
                FraisService = dto.FraisService,
                FraisLivraison = dto.FraisLivraison,
                StripePaymentIntentId = dto.StripePaymentIntentId,
                Statut = "Payée",
                DateCommande = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(commande);

            // Mettre à jour le statut de l'annonce
            //TODO : changer avec l'id
            annonce.EtatId = 0;
            await _annonceRepository.UpdateAsync(annonce);

            _logger.LogInformation($"✅ Order created: {commande.CommandeId}");

            // Récupérer la commande complète avec toutes les relations
            var orderWithDetails = await _orderRepository.GetOrderWithDetailsAsync(commande.CommandeId);
            var orderDto = _mapper.Map<OrderDTO>(orderWithDetails);

            return CreatedAtAction(nameof(GetOrderById), new { id = commande.CommandeId }, orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error creating order: {ex.Message}");
            return StatusCode(500, "An error occurred while creating the order");
        }
    }

    /// <summary>
    /// Obtenir une commande par ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var orderDto = _mapper.Map<OrderDTO>(order);
        return Ok(orderDto);
    }

    /// <summary>
    /// Obtenir toutes les commandes d'un acheteur
    /// </summary>
    [HttpGet("user")]
    [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDTO>>> GetUserOrders()
    {
        var userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        var orders = await _orderRepository.GetOrdersByUserIdAsync((int)userId);
        var orderDtos = _mapper.Map<List<OrderDTO>>(orders);
        return Ok(orderDtos);
    }

    /// <summary>
    /// Obtenir toutes les ventes d'un vendeur
    /// </summary>
    [HttpGet("seller")]
    [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDTO>>> GetSellerOrders()
    {
        var sellerId = await _currentUserService.GetUserId();
        if (sellerId == null)
            return Unauthorized();
        var orders = await _orderRepository.GetOrdersBySellerIdAsync((int)sellerId);
        var orderDtos = _mapper.Map<List<OrderDTO>>(orders);
        return Ok(orderDtos);
    }

    /// <summary>
    /// Obtenir une commande par Payment Intent ID
    /// </summary>
    [HttpGet("payment-intent/{paymentIntentId}")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDTO>> GetOrderByPaymentIntent(string paymentIntentId)
    {
        var order = await _orderRepository.GetOrderByPaymentIntentIdAsync(paymentIntentId);
        if (order == null)
        {
            return NotFound();
        }

        var orderDto = _mapper.Map<OrderDTO>(order);
        return Ok(orderDto);
    }

    /// <summary>
    /// Mettre à jour le statut d'une commande
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
    {
        var success = await _orderRepository.UpdateOrderStatusAsync(id, dto.Statut, dto.NumeroSuivi);
        
        if (!success)
        {
            return NotFound();
        }

        _logger.LogInformation($"✅ Order {id} status updated to: {dto.Statut}");
        return NoContent();
    }

    /// <summary>
    /// Obtenir les statistiques des commandes d'un utilisateur
    /// </summary>
    [HttpGet("user/{userId}/stats")]
    [ProducesResponseType(typeof(OrderStatsDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderStatsDTO>> GetUserOrderStats(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

        var stats = new OrderStatsDTO
        {
            TotalCommandes = orders.Count,
            CommandesEnCours = orders.Count(o => o.Statut == "Payée" || o.Statut == "Expédiée"),
            CommandesLivrees = orders.Count(o => o.Statut == "Livrée"),
            MontantTotal = orders.Sum(o => o.MontantTotal)
        };

        return Ok(stats);
    }
}