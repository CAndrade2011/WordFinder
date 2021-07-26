using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindWord
{
    public class WordFinder : IDisposable
    {
        #region Configs

        const int _maxCharsSize = 64;
        const int _maxLineSize = 64;
        const int _maxReturnCount = 10;

        #endregion

        #region Test Methods

        private List<string> _Matrix;

        /// <summary>
        /// Constructor, needs to use with scrambed chars matrix 
        /// </summary>
        /// <param name="matrix">chars matrix </param>
        public WordFinder(IEnumerable<string> matrix)
        {
            Debug.WriteLine("WordFinder started, loading matrix string.");

            // Check matrix null
            if (matrix == null)
            {
                throw new ArgumentNullException("Matrix cannot be null.");
            }

            // Check matrix empty
            if (!matrix.Any())
            {
                throw new ArgumentException("Matrix cannot be empty.");
            }

            // Check max line count to keep max of 64 lines
            if (matrix.Count() > _maxLineSize)
                throw new ArgumentException($"You need to inform a matrix with maximum of {_maxLineSize} lines. (Matrix informed with {matrix.Count()})");

            // Check max line size to keep max of 64 chars
            var maxColumn = 0;
            var linesCount = 0;
            _Matrix = new List<string>();
            foreach (var line in matrix)
            {
                Debug.WriteLine("Copying values to horizontal check");
                if (!string.IsNullOrEmpty(line))
                    _Matrix.Add(line.ToUpper());

                // Check matrix X dimension
                linesCount++;
                if (line.Length > maxColumn)
                {
                    maxColumn = line.Length;
                    if (maxColumn > _maxCharsSize)
                        throw new ArgumentException($"You need to inform a matrix with maximum of {_maxCharsSize} chars per line. (Error on line {linesCount})");
                }
            }


            Debug.WriteLine("Pivot to vertical check");
            linesCount = 0;
            for (var i = 0; i < maxColumn; i++)
            {
                linesCount++;
                var newLine = string.Empty;
                foreach (var line in matrix)
                {
                    try
                    {
                        newLine += line[i];
                    }
                    catch (Exception exLine)
                    {
                        throw new ArgumentException($"Pivot error, Matrix invalid char {i + 1}. (Error on line {linesCount})", exLine);
                    }
                }

                // Creating new line
                if (!string.IsNullOrEmpty(newLine))
                    _Matrix.Add(newLine.ToUpper());
            }

            Debug.WriteLine($"Received a {maxColumn}x{matrix.Count()} matrix, created a new {maxColumn}x{_Matrix.Count} matrix ");
        }

        /// <summary>
        /// Method to find words
        /// </summary>
        /// <param name="wordstream">Words we need to find</param>
        /// <returns>Words found</returns>
        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {
            Debug.WriteLine($"Starting find method");

            // Check wordstream null
            if (wordstream == null)
            {
                throw new ArgumentNullException("Wordstream cannot be null.");
            }

            // Check wordstream empty
            if (!wordstream.Any())
            {
                throw new ArgumentException("Wordstream cannot be empty.");
            }

            // Findign words
            var ret = new List<string>();
            foreach (var ws in wordstream)
            {
                if (!string.IsNullOrEmpty(ws) && !ret.Contains(ws.ToUpper()))
                {
                    var wsupper = ws.ToUpper();
                    foreach (var line in _Matrix)
                    {
                        if (line.Contains(wsupper) && !ret.Contains(wsupper))
                        {
                            ret.Add(wsupper);
                            if (ret.Count == _maxReturnCount)
                                break;
                        }
                    }
                }
                if (ret.Count == _maxReturnCount)
                    break;
            }

            Debug.WriteLine($"Received {wordstream.Count()}, found {ret.Count} word(s)");
            return ret;
        }

        /// <summary>
        /// Class Destructor
        /// </summary>
        ~WordFinder()
        {
            if (!Disposed)
                Dispose(false);
        }

        #endregion

        #region IDiposable Methods

        // Use it for check if object was disposed
        public bool Disposed { get; private set; } = false;

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.Disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.

                }

                // Note disposing has been done.
                Disposed = true;
            }
        }

        #endregion
    }
}
