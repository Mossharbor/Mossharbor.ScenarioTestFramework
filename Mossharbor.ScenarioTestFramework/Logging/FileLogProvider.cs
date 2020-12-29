using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class FileLogProvider : DefaultLogProvider, ILogProvider
    {
        private string logFile = null;

        /// <summary>
        /// constructor
        /// </summary>
        public FileLogProvider(string logFile)
            : base(new StreamWriter(logFile, false), Path.GetFileNameWithoutExtension(logFile))
        {
            this.LogProviderName = "FileLogProvider";
            ((StreamWriter)base.Stream).AutoFlush = true;
            this.logFile = logFile;
        }
    }
}
