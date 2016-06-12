//copy it from https://github.com/daxnet/we-text/blob/master/src/WeText.Common/DisposableObject.cs
using System;

namespace Abp
{
    /// <summary>
    /// Represents that the derived types are disposable and disposing process
    /// will follow standard .NET design pattern.
    /// </summary>   
    /// <seealso cref="System.IDisposable" />
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableObject"/> class.
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) { }
    }
}
