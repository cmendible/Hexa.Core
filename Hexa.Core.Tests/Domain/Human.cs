namespace Hexa.Core.Tests.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Core.Domain;

    [Serializable]
    public class Human : AuditableEntity<Human>
    {
        #region Properties

        [Required]
        public virtual bool isMale
        {
            get;
            set;
        }

        [Required]
        public virtual string Name
        {
            get;
            set;
        }

        #endregion Properties
    }
}