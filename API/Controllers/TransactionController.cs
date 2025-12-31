using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Historique;
using Shared.DTO.Transaction;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository<Transaction, int> _transactionRepo;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public TransactionController(ITransactionRepository<Transaction, int> repo, IMapper mapper, ICurrentUserService currentUserService)
        {
            _transactionRepo = repo;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Retourne la liste des transactions dont on rentre l'id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactionById(int id)
        {
            var result = await _transactionRepo.GetByIdTransaction(id);
            return Ok(_mapper.Map<IEnumerable<TransactionDTO>>(result));
        }

        [HttpGet("transaction/conversation/{idconversation}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactionByConversationId(int idconversation)
        {
            var result = await _transactionRepo.GetByIdConversation(idconversation);
            return Ok(_mapper.Map<IEnumerable<TransactionDTO>>(result));
        }

        [HttpGet("historique/acheteur/{idutilisateur}")]
        public async Task<ActionResult<IEnumerable<TransactionHistoriqueDTO>>> GetHistoriqueAcheteur(int idutilisateur)
        {
            var result = await _transactionRepo.GetHistoriqueAcheter(idutilisateur);
            return Ok(_mapper.Map<IEnumerable<TransactionHistoriqueDTO>>(result));
        }

        [HttpGet("historique/vendeur/{idutilisateur}")]
        public async Task<ActionResult<IEnumerable<TransactionHistoriqueDTO>>> GetHistoriqueVendeur(int idutilisateur)
        {
            var result = await _transactionRepo.GetHistoriqueVendu(idutilisateur);
            return Ok(_mapper.Map<IEnumerable<TransactionHistoriqueDTO>>(result));
        }
    }
}
