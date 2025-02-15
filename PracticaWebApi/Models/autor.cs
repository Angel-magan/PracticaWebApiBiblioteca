using System.ComponentModel.DataAnnotations;

namespace PracticaWebApi.Models
{
    public class autor
    {
        [Key]
        public int id_autor {  get; set; }
        public string? nombre { get; set; }
        public string? nacionalidad { get; set; }
    }
}
