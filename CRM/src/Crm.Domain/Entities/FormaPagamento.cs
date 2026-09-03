namespace Crm.Domain.Entities;

public class FormaPagamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // "A Vista", "28 DDL", "50% Entrada + 50% Entrega", "Cartao", "A Combinar"
    public bool Ativo { get; set; } = true;

    public const int AVista = 1;
    public const int DDL28 = 2;
    public const int EntradaMaisEntrega = 3;
    public const int Cartao = 4;
    public const int ACombinar = 5;
}
