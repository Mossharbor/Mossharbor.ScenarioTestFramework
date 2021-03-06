﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mossharbor.ScenarioTestFramework
{
    using System.IO;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using Mossharbor.ScenarioTestFramework.Exceptions;

    class ArgumentParser
    {
        // Variables
        private StringDictionary parameters;
        private StringDictionary unknownParams;
        private string[] args;

        public ArgumentParser(string[] Args)
        {
            args = Args;
            parameters = new StringDictionary();
            unknownParams = new StringDictionary();
            string lastOption = null;
            string potentialOption = null;
            
            foreach (string arg in args)
            {
                if (IsOption(arg, out potentialOption))
                {
                    if (null != lastOption)
                    {
                        parameters.Add(lastOption, "true");
                        unknownParams.Add(lastOption, arg);
                    }

                    lastOption = potentialOption;
                    continue;
                }
                else if (null != lastOption)
                {
                    if (!parameters.ContainsKey(lastOption))
                    {
                        unknownParams.Add(lastOption, arg);
                        parameters.Add(lastOption, arg);
                    }
                    else
                    {
                        parameters[lastOption] = arg;
                        unknownParams.Add(lastOption, arg);
                    }

                    lastOption = null;
                }
            }

            if (null != lastOption)
            {
                parameters.Add(lastOption, "true");
                unknownParams.Add(lastOption, "true");
            }
        }

        public System.Collections.IEnumerator UnknownParams
        {
            get { return unknownParams.GetEnumerator(); }
        }

        private bool IsOption(string arg, out string lastOption)
        {
            bool startsWithSlash = arg.Trim().StartsWith("/");
            bool startsWithDash = arg.Trim().StartsWith("-");
            bool isOption = startsWithSlash || startsWithDash;

            if (!isOption)
            {
                lastOption = String.Empty;
                return isOption;
            }
            else if (startsWithSlash && (Directory.Exists(arg.Trim()) || File.Exists(arg.Trim())))
            {
                lastOption = String.Empty;
                return false;
            }

            lastOption = arg.ToLowerInvariant().Trim().TrimStart("-".ToCharArray()).TrimStart("/".ToCharArray());

            return isOption;
        }

        public bool ContainsParam(string param)
        {
            return (this[param] != null) ? true : false;
        }

        public void ValidateCommandLineParams(IEnumerable<string> arguments)
        {
            Dictionary<string,bool> checker = new Dictionary<string,bool>();

            foreach(string passedInParam in parameters.Keys)
            {
                bool found = false;
                foreach(string validCmdParam in arguments)
                {
                    if (validCmdParam.Equals(passedInParam,StringComparison.CurrentCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                checker.Add(passedInParam, found);
            }

            StringBuilder unkownParameters = new StringBuilder();
            foreach (string passedInParam in parameters.Keys)
            {
                if (checker[passedInParam])
                    continue;

                unkownParameters.AppendLine(passedInParam);
            }

            if (0 != unkownParameters.Length)
                throw new UnknownCommandLineParametersException(unkownParameters.ToString());
        }

        public System.Collections.ICollection Keys { get { return parameters.Keys; } }

        public string this[string index]
        {
            get
            {
                if (!parameters.ContainsKey(index.ToLowerInvariant().Trim()))
                    return null;

                if (!unknownParams.ContainsKey(index.ToLowerInvariant().Trim()))
                    unknownParams.Remove(index.ToLowerInvariant().Trim());

                return (parameters[index.ToLowerInvariant().Trim()]);
            }
        }

        public string this[int index]
        {
            get
            {
                // NOTE: the +1 skips the command
                if (index + 1 >= this.Count)
                    return String.Empty;

                return args[index+1];
            }
        }

        public int Count
        {
            get
            {
                return args.Length;
            }
        }
    }
}
