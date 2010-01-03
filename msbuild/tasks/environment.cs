using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace HSBuild.Tasks
{
    public class SetEnv : Task
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        [Output]
        public string OutputEnvironmentVariable
        {
            get { return Environment.GetEnvironmentVariable(Name); }
        }

        public override bool Execute()
        {
            Environment.SetEnvironmentVariable(Name, Value);
            return true;
        }
    }

    public class GetEnv : Task
    {
        [Required]
        public string Name { get; set; }
        [Required, Output]
        public string Value { get; set; }

        public override bool Execute()
        {
            Value = Environment.GetEnvironmentVariable(Name);
            return true;
        }
    }
}
