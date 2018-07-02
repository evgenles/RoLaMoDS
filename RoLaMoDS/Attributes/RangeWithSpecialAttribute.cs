using System;
using System.ComponentModel.DataAnnotations;

namespace RoLaMoDS.Attributes
{
    /// <summary>
    /// Attribute or validate range and aditional value
    /// </summary>
     [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    class RangeWithSpecialAttribute : RangeAttribute
    {
        private int special;

        public RangeWithSpecialAttribute(double minimum, double maximum, int special)
              : base(minimum, maximum)
        {
            this.special = special;
        }
        public int Special
        {
            get
            {
                return this.special;
            }
            set
            {
                this.special = value;
            }
        }
        public override bool Equals(object obj)
        {
            RangeWithSpecialAttribute cra = obj as RangeWithSpecialAttribute;
            if (cra == null)
            {
                return false;
            }
            return this.special.Equals(cra.special) && base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return this.special.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid(object value)
        {
            return this.special.Equals(value) || base.IsValid(value);
        }
        protected override ValidationResult IsValid(object value,
                           ValidationContext validationContext)
        {
            if (this.special.Equals(value))
            {
                return ValidationResult.Success;
            }
            return base.IsValid(value, validationContext);
        }
    }
}