using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Cliente : IAuditableEntity
{
    public long Id { get; set; }
    public string? CodigoCliente { get; set; } // Ex: "3223"
    public string Nome { get; set; } = string.Empty; // Ex: "CONSUMIDOR" ou Razao Social
    public string? Documento { get; set; } // CPF/CNPJ flexível (pode ser genérico como no modelo real)
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    public string? Cep { get; set; }
    public bool Ativo { get; set; } = true; // Exclusão lógica obrigatória (histórico comercial nunca apaga)

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }

    public ICollection<Contato> Contatos { get; set; } = new List<Contato>();
    public ICollection<Proposta> Propostas { get; set; } = new List<Proposta>();
}
