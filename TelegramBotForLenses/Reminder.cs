using System.ComponentModel.DataAnnotations;
namespace TelegramBotForLenses;


public class Reminder
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Message { get; set; } = "Линзы";
    [Required]
    public DateTime Date { get; set; }
}