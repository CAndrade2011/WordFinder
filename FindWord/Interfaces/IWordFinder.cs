using System;
using System.Collections.Generic;
using System.Text;

namespace FindWord.Interfaces
{
    public interface IWordFinder : IDisposable
    {
        /// <summary>
        /// Method to find words
        /// </summary>
        /// <param name="wordstream">Words we need to find</param>
        /// <returns>Words found</returns>
        IEnumerable<string> Find(IEnumerable<string> wordstream);
    }
}
