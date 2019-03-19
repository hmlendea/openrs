using System;

namespace OpenRS.Net.Client
{
    /// <summary>
    /// Login exception.
    /// </summary>
    public class LoginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginException"/> exception.
        /// </summary>
        public LoginException()
            : base()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginException"/> exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public LoginException(string message)
            : base(message)
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginException"/> exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public LoginException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
