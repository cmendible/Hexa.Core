using System.ComponentModel.DataAnnotations;

using Hexa.Core.Domain;

namespace Hexa.Core.Tests.Domain
{
    public class Human : RootEntity<Human>
    {
        [Required]
        public virtual string Name { get; set; }
    }
}
