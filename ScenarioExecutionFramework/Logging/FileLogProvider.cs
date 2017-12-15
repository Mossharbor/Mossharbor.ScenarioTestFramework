using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class FileLogProvider : DefaultLogProvider, ILogProvider
    {
        /// <summary>
        /// constructor
        /// </summary>
        public FileLogProvider()
            : base(new StreamWriter(GetFileName(), false), GetFileName())
        {
            this.LogProviderName = "FileLogProvider";
            ((StreamWriter)base.Stream).AutoFlush = true;
        }

        /// <summary>
        /// Get file provider name
        /// </summary>
        /// <returns>File name</returns>
        private static string GetFileName()
        {
            return Path.ChangeExtension(Logger.Instance.LogFileName, "txt");
        }
    }
}
