using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtensionFixer.Shared
{
    public class ExtensionFixerTool
    {
        private readonly Action<string> _log;
        private readonly HashSet<string> _unknownExt = new HashSet<string>();
        private readonly HashSet<string> _warnings = new HashSet<string>();

        public ExtensionFixerTool(Action<string> log)
        {
            _log = log;
        }

        public void Main(string folder, bool verbose, bool doRename, IMyProgress<double> progress)
        {
            var l = FileSignature.LongestSig();
            progress.SetIndefinite();
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).ToList();
            progress.SetMax(files.Count);

            var counter = 0;
            foreach (var f in files)
            {
                try
                {
                    progress.Report(counter++);
                    CheckFile(f, l, verbose, doRename);
                }
                catch (Exception e)
                {
                    _log("Unable to inspect file " + f);
                }
            }
        }

        private void CheckFile(string fileName, int sigLength, bool verbose, bool doRename)
        {
            var ext = Path.GetExtension(fileName).TrimStart('.');

            if (ext == "exe" || ext == "dll" || ext == "pdb")
                return;

            var needsRename = true;
            FileSignature matchingSignature = null;

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var buff = new byte[sigLength];
                var res = stream.Read(buff, 0, sigLength);
                var detected = false;
                foreach (var fileSignature in FileSignature.AllSignatures)
                {
                    foreach (var signature in fileSignature.Signatures)
                    {
                        if (res >= signature.Length && ArrayEqual(buff, signature, fileSignature.SigOffset, signature.Length))
                        {
                            if (ext.Equals(fileSignature.Extension, StringComparison.InvariantCultureIgnoreCase) ||
                                fileSignature.AlternateExtensions.Any(e =>
                                    e.Equals(ext, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                if (verbose)
                                    _log($"+ {fileName} - OK ({fileSignature.Extension})");
                                needsRename = false;
                            }
                            else if (fileSignature.Extension == "")
                            {
                                // detected file, no specific ending required
                                if (!fileSignature.AlternateExtensions.Contains(ext))
                                {
                                    if (!_warnings.Contains(ext + ":" + fileSignature.Name) || verbose)
                                    {
                                        _log($"! {fileName} - detected as ({fileSignature.Name}), unexpected file extension!");
                                        _warnings.Add(ext + ":" + fileSignature.Name);
                                    }
                                }
                                else if (verbose)
                                    _log($"+ {fileName} - detected as ({fileSignature.Name})");
                            }
                            else
                            {
                                _log($"! {fileName} - Should be {fileSignature.Extension}");
                                matchingSignature = fileSignature;
                            }

                            detected = true;
                        }
                    }
                }
                if (!detected && !_unknownExt.Contains(ext))
                {
                    var sig = string.Join(" ", buff.Select(b => b.ToString("X2")));
                    var sig2 = Encoding.ASCII.GetString(buff).Replace(Environment.NewLine, "\\n");
                    _log($"? {fileName} - unknown file format: [{sig}] [{sig2}]");
                    _unknownExt.Add(ext);
                }

            }

            if (doRename && needsRename && matchingSignature != null)
            {
                _log($"! {fileName} - Adding file extension {matchingSignature.Extension}");

                File.Move(fileName, fileName + "." + matchingSignature.Extension);
            }
        }

        private static bool ArrayEqual(byte[] buffer, byte[] signature, int sigOffset, int count)
        {
            for (int i = 0; i < count; i++)
                if (buffer[i + sigOffset] != signature[i])
                    return false;
            return true;
        }
    }

    public interface IMyProgress<T> : IProgress<T>
    {
        void SetMax(int maxValue);
        void SetIndefinite();
    }

}
