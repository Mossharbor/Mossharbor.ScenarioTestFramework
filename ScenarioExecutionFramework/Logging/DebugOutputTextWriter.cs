using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class DebugOutputTextWriter : TextWriter
    {
        private string keptItem = "";
        private string newLine = "\n";
        IFormatProvider fmt;

        // Summary:
        // Initializes a new instance of the System.IO.TextWriter class.
        public DebugOutputTextWriter()
            : base()
        {
            this.fmt = new CultureInfo("en-US");
        }

        //
        // Summary:
        // Initializes a new instance of the System.IO.TextWriter class with the
        // specified format provider.
        // 
        // Parameters:
        // formatProvider: 
        //    An System.IFormatProvider object that controls formatting. 
        protected DebugOutputTextWriter(IFormatProvider formatProvider)
            : base(formatProvider)
        {
            this.fmt = formatProvider;
        }

        // Summary:
        // When overridden in a derived class, returns the System.Text.Encoding in
        // which the output is written.
        // 
        // Returns:
        // The Encoding in which the output is written.
        public override Encoding Encoding
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        //
        // Summary:
        // Gets an object that controls formatting.
        // 
        // Returns:
        // An System.IFormatProvider object for a specific culture, or the formatting
        // of the current culture if no other culture is specified.
        public override IFormatProvider FormatProvider
        {
            get { return this.fmt; }
        }

        //
        // Summary:
        // Gets or sets the line terminator string used by the current TextWriter.
        // 
        // Returns:
        // The line terminator string for the current TextWriter.
        public override string NewLine
        {
            get { return newLine; }
            set { newLine = value; }
        }

        // Summary:
        // Closes the current writer and releases any system resources associated with
        // the writer.
        public override void Close()
        {
            if (!String.IsNullOrEmpty(keptItem))
            {
                this.WriteLine(keptItem);
                keptItem = null;
            }
        }

        //
        // Summary:
        // Releases the unmanaged resources used by the System.IO.TextWriter and
        // optionally releases the managed resources.
        // 
        // Parameters:
        // disposing: 
        //    true to release both managed and unmanaged resources; false to release
        //    only unmanaged resources. 
        protected override void Dispose(bool disposing)
        {
        }

        //
        // Summary:
        // Clears all buffers for the current writer and causes any buffered data to be
        // written to the underlying device.
        public override void Flush()
        {
            if (!String.IsNullOrEmpty(keptItem))
            {
                this.WriteLine(keptItem);
                keptItem = null;
            }
        }


        //
        // Summary:
        // Writes a line terminator to the text stream.
        // 
        // Returns:
        // The default line terminator is a carriage return followed by a line feed
        // ("\r\n"), but this value can be changed using the
        // System.IO.TextWriter.NewLine property.
        // 
        // Exceptions:
        //    System.IO.IOException
        // 
        //    System.ObjectDisposedException
        // 
        public override void WriteLine()
        {
            if (!String.IsNullOrEmpty(keptItem))
            {
                this.WriteLine(keptItem);
                keptItem = null;
            }
            else
            {
                Debug.WriteLine("");
            }
        }

        public override void Write(string value)
        {
            keptItem += value;
        }

        //
        // Summary:
        // Writes a string followed by a line terminator to the text stream.
        // 
        // Parameters:
        // value: 
        //    The string to write. If value is null, only the line termination
        //    characters are written. 
        // 
        // Exceptions:
        //    System.IO.IOException
        // 
        //    System.ObjectDisposedException
        // 
        public override void WriteLine(string value)
        {
            if (!String.IsNullOrEmpty(keptItem))
            {
                Debug.WriteLine(keptItem);
                keptItem = null;
            }
            else
            {
                Debug.WriteLine(value);
            }
        }

        public override void WriteLine(string value, params object[] args)
        {
            if (!String.IsNullOrEmpty(keptItem))
            {
                Debug.WriteLine(keptItem);
                keptItem = null;
            }
            else
            {
                Debug.WriteLine(String.Format(value, args));
            }
        }
    }
}
