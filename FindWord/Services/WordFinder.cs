using FindWord.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FindWord.Services
{
    public class WordFinder : Interfaces.IWordFinder, IDisposable
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
            var ret = new List<FoundWordDTO>();
            foreach (var ws in wordstream)
            {
                if (!string.IsNullOrEmpty(ws))
                {
                    var wsupper = ws.ToUpper();
                    for (var j = 0; j < _Matrix.Count; j++)
                    {
                        var line = _Matrix[j];
                        var countWords = CountWords(line, wsupper);
                        if (countWords > 0)
                            AddOrUpdWord(ref ret, wsupper, countWords);
                    }
                }
            }

            if (ret.Count > 0)
            {
                Debug.WriteLine($"Received {wordstream.Count()}, found {ret.Count} word(s)");
                return ret.OrderByDescending(x => x.FoundCount).Take(10).Select(x => x.FoundWord).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Change values on unique return list
        /// </summary>
        /// <param name="dtos">Exists list</param>
        /// <param name="wsupper">Current word</param>
        /// <param name="countWords">How many times this word appear</param>
        private void AddOrUpdWord(ref List<FoundWordDTO> dtos, string wsupper, int countWords)
        {
            var exists = dtos.Where(x => x.FoundWord == wsupper).FirstOrDefault();
            if (exists != null && exists.FoundWord == wsupper)
            {
                exists.FoundCount += countWords;
            }
            else
            {
                exists = new FoundWordDTO { FoundCount = 0, FoundWord = wsupper };
                exists.FoundCount += countWords;
                dtos.Add(exists);
            }
        }

        /// <summary>
        /// Find how many times there is a word in a string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="wsupper"></param>
        /// <returns></returns>
        private int CountWords(string line, string wsupper)
        {
            var ret = 0;
            var countToEnd = 0;
            var arrayIndex = 0;
            var start = 0;
            var end = line.Length;
            while ((start <= end) && (arrayIndex > -1))
            {
                // start+count must be a position within -str-.
                countToEnd = end - start;
                arrayIndex = line.IndexOf(wsupper, start, countToEnd);
                if (arrayIndex == -1) break;
                Debug.WriteLine($"Found {wsupper} at {arrayIndex}");
                ret++;
                start = arrayIndex + 1;
            }
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
