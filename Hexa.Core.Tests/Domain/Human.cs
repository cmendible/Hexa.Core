namespace Hexa.Core.Tests.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Domain;

    [Serializable]
    public class Human : AuditableRootEntity<Human>
    {
        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual bool isMale { get; set; }
    }
}