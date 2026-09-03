namespace Crm.Domain.Entities;

public class StatusProposta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Rascunho, Enviada, Aprovada, Recusada, Cancelada, Expirada
    public string? Descricao { get; set; }
    public string CorHex { get; set; } = "#757575";

    public const int Rascunho = 1;
    public const int Enviada = 2;
    public const int Aprovada = 3;
    public const int Recusada = 4;
    public const int Cancelada = 5;
    public const int Expirada = 6;
}
