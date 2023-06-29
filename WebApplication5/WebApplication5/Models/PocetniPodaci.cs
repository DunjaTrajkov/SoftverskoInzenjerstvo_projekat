using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class PocetniPodaci
    {
        [Key]
        public int Id_pocetne {get; set;}
        [DataType(DataType.MultilineText)]
        [StringLength(3000,MinimumLength = 5)]
    
        public string ONama { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(3000, MinimumLength = 5)]
        public string KorisnickaUsluga { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(3000, MinimumLength = 5)]
        public string UsloviKoriscenja { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(3000, MinimumLength = 5)]
        public string PolitikaPrivatnost { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(3000, MinimumLength = 5)]
        public string InformacijeODostavi { get; set; }
    }
}
