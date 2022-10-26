using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MimimalAPIsDemo.Entities
{
    [Table("TESTTABLE", Schema = "MAG")]
    public partial class Testtable
    {
        [Key]
        [Column("ID")]
        [Precision(9)]
        public int Id { get; set; }
        [Column("NAME")]
        [StringLength(20)]
        [Unicode(false)]
        public string Name { get; set; }
        [Column("AGE")]
        [StringLength(20)]
        [Unicode(false)]
        public string Age { get; set; }
    }
}
