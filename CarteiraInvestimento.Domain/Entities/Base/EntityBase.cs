using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraInvestimento.Domain.Entities.Base
{
    public abstract class EntityBase
    {
        public Guid Id { get; protected set; }
        public DateTime DataInclusao { get; set; }
        public ValidationResult ValidationResult { get; protected set; }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as EntityBase;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase a, EntityBase b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() * 907 + Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + " [Id=" + Id + "]";
        }
    }
}