using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMapper
{
    /// <summary>
    /// TypeKey
    /// </summary>
    struct TypeKey : IEquatable<TypeKey>
    {
        public TypeKey(Type sourceType, Type destinationType)
        {
            SourceType = sourceType;
            DestinationType = destinationType;
        }

        /// <summary>
        /// SourceType
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// DestinationType
        /// </summary>
        public Type DestinationType { get; }

        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            TypeKey other = (TypeKey)obj;

            return Equals(other);
        }

        public bool Equals(TypeKey other)
        {
            return SourceType == other.SourceType && DestinationType == other.DestinationType;
        }

        public override int GetHashCode()
        {
            return SourceType.GetHashCode() ^ DestinationType.GetHashCode();
        }
    }
}
