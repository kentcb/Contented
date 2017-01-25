namespace Contented.PowerShell
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Text.RegularExpressions;

    [Cmdlet(VerbsCommon.Rename, "Contented")]
    public sealed class RenameContentCmdlet : PSCmdlet
    {
        [Parameter(
            Mandatory = false,
            HelpMessage = "Accept proposed renames. Use with extreme caution! Generally you want to run the command first without this switch, then only run it with once you've verified the proposals.")]
        public SwitchParameter Accept
        {
            get;
            set;
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Whether to recurse through subdirectories.")]
        public SwitchParameter Recursive
        {
            get;
            set;
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Whether to show abbreviated paths. i.e. don't show the leading portion of the path that the original and new names have in common.")]
        public SwitchParameter ShowAbbreviatedPaths
        {
            get;
            set;
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Skips prepending the current directory to the output file names. Use with caution.")]
        public SwitchParameter NoCurrentDirectoryPrepend
        {
            get;
            set;
        }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The regular expression used to match against file names. For example, '(?<name>.+).dll'.")]
        public string Match
        {
            get;
            set;
        }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The regular expression used to replace file names. For example, '${name}.exe'")]
        public string Replace
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var currentDirectory = this
                .SessionState
                .Path
                .CurrentFileSystemLocation
                .ProviderPath;
            var files = Directory.GetFiles(currentDirectory, "*", this.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var oldName in files)
            {
                var newName = Regex.Replace(oldName, this.Match, this.Replace);

                if (!this.NoCurrentDirectoryPrepend)
                {
                    newName = Path.Combine(currentDirectory, newName);
                }

                if (!string.Equals(oldName, newName))
                {
                    if (this.Accept)
                    {
                        File.Move(oldName, newName);
                    }

                    var output = new RenamedContent(
                        oldName,
                        newName,
                        this.GetDisplayFor(oldName),
                        this.GetDisplayFor(newName));
                    this.WriteObject(output);
                }
            }
        }

        private string GetDisplayFor(string file)
        {
            if (!this.ShowAbbreviatedPaths)
            {
                return file;
            }

            try
            {
                var currentDirectory = this
                    .SessionState
                    .Path
                    .CurrentFileSystemLocation
                    .ProviderPath + Path.DirectorySeparatorChar;
                var path1 = new Uri(currentDirectory);
                var path2 = new Uri(file);
                var delta = path1.MakeRelativeUri(path2);

                return delta.ToString();
            }
            catch (UriFormatException)
            {
                return file;
            }
        }
    }

    public sealed class RenamedContent
    {
        private readonly string originalOldName;
        private readonly string originalNewName;
        private readonly string oldName;
        private readonly string newName;

        public RenamedContent(
            string originalOldName,
            string originalNewName,
            string oldName,
            string newName)
        {
            this.originalOldName = originalOldName;
            this.originalNewName = originalNewName;
            this.oldName = oldName;
            this.newName = newName;
        }

        public string OriginalOldName => this.originalOldName;

        public string OriginalNewName => this.originalNewName;

        public string OldName => this.oldName;

        public string NewName => this.newName;
    }
}