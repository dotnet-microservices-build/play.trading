using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Trading.Service.Exceptions
{
    [Serializable]
    internal class UnknownItemException : Exception
    {
        public Guid ItemId { get; }

        public UnknownItemException()
        {
        }

        public UnknownItemException(Guid itemId) : base($"Unkown item '{itemId}' ")
        {
            this.ItemId = itemId;
        }


    }
}