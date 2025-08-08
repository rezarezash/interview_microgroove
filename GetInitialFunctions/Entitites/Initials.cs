using GetInitialFunctions.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GetInitialFunctions.Entitites
{
    public class Initials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [NotNull]
        [MaxLength(30)]
        public required string FirstName { get; set; }

        [NotNull]
        [MaxLength(50)]
        public required string LastName { get; set; }

        public string? Svg { get; set; }

        public static Initials Create(InitialsDto initialsDto) => new()
        {
            FirstName = initialsDto.FirstName,
            LastName = initialsDto.LastName
        };
    }
}
