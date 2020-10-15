using System;
using System.Collections.Generic;

namespace HandiCrafts.Core.Exceptions
{
    public class HandiCraftsException : Exception
    {
        private ICollection<string> _relatedErrors;

        public HandiCraftsException()
            : base()
        {
        }

        public HandiCraftsException(string message)
            : base(message)
        {
        }

        public HandiCraftsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HandiCraftsException(string message, Exception innerException, List<string> relatedErrors)
            : base(message, innerException)
        {
            RelatedErrors = relatedErrors;
        }

        public HandiCraftsException(string message, List<string> relatedErrors)
          : base(message)
        {
            RelatedErrors = relatedErrors;
        }

        public object[] Arguments { get; set; }

        public virtual ICollection<string> RelatedErrors
        {
            get => _relatedErrors ?? (_relatedErrors = new List<string>());
            set => _relatedErrors = value;
        }
    }
}
