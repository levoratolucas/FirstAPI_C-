using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace firstORM.models
{
    public class VendaModel
    {
         public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DataVenda { get; set; }

        public string NumeroNotaFiscal { get; set; }
        public int ClienteId { get; set; }
        public ClienteModel Cliente { get; set; }
        public int ProdutoId { get; set; }
        public ProdutoModel Produto { get; set; }
        public int QuantidadeVendida { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }
    }
}