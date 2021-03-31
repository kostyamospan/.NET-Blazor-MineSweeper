using System.ComponentModel.DataAnnotations;

namespace BlazorMinesweeper.Models
{
    public class CellModel
    {
        public bool IsOpened { get; set; }
        
        public bool IsChecked { get; set; }
        
        public int Value { get; set; }

        
        public string CssClass => "d" + Value;
    }
}