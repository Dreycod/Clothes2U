using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_element_decision_message_elumes")]
    public class ElementDecisionMessage
    {
        [Key]
        [Column("elumes_id")]
        public int ElementDecisionMessageId { get; set; }

        [Column("elumes_element_decision_id")]
        public int ElementDecisionId { get; set; }

        [Column("elumes_message_id")]
        public int MessageId { get; set; }

        [ForeignKey(nameof(ElementDecisionId))]
        [InverseProperty(nameof(ElementDecision.Elementdecisionmessages))]
        public virtual ElementDecision Elementdecision { get; set; }

        [ForeignKey(nameof(MessageId))]
        [InverseProperty(nameof(Message.Elementdecisionmessages))]
        public virtual Message MessageElmtDeci { get; set; }
    }
}
