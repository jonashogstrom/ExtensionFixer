using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExtensionFixer.Shared
{
    internal class FileSignature
    {
        // https://en.wikipedia.org/wiki/List_of_file_signatures

        private static string[] _xmlExtensions = new string[]
        {
            "xml",  "xaml", "html", "htm", "props", "xsl", "xslt", "disco", "wsdl", "targets",
            "nuspec", "rdf",
            "vcxproj", "vdproj", "csproj", "config", "svg", "xsd", "map", "resx","vcproj", "plist", "xul", "ruleset"
        };


        private static readonly string[] _textfileExtensions = {
            "txt", "log", "manifest", "tlog", "ini", "cfg", "vb", "config", "cs",
            "json", "js", "exp",  "user", "sln", "vbproj", "nuspec",
            "settings", "cpp", "props", "c", "h", "filters", "reg", "nfo", "css",
            "clang-format", "csv"
        };

        public static FileSignature Jpg = new FileSignature("jpg", "image/jpg", new[] { "jpeg" }, new[]
        {
            C("FF D8 FF DB"),
            C("FF D8 FF E0"),
            C("FF D8 FF EE"),
            C("FF D8 FF E1"),
        });

        public static FileSignature Avi = new FileSignature("avi", "video/avi", new string[] { }, new[]
        {
            C("52 49 46 46"),
        });

        public static FileSignature Mp3 = new FileSignature("mp3", "audio/mp3", new string[] { }, new[]
        {
            C("49 44 33 03"), // ID3 #03
        });

        // https://www.ftyps.com/

        public static FileSignature Avif = new FileSignature("avi", "image/avif", new string[] { }, new[]
        {
            C("00 00 00 1C 66 74 79 70 61 76 69 66"), // null null null #1C ftypavif
        });

        public static FileSignature Mov = new FileSignature("mov", "video/mov", new string[] { }, new[]
        {
            C("00 00 00 14 66 74 79 70 71 74"), // null null null #14 ftypqt
        });

        public static FileSignature Elf = new FileSignature("", "executable/elf", new string[] { "mexa64", "mexglx" }, new[]
        {
            C("7F 45 4C 46"), // ⌂ELF
        });
        public static FileSignature WinReg = new FileSignature("reg", "windows registry", new string[] { }, new[]
        {
            C("57 69 6E 64 6F 77 73 20 52 65 67 69 73 74 72 79 20 45 64 69 74 6F 72 20 56 65 72 73 69 6F 6E"), // Windows Registry Editor Version
        });

        public static FileSignature Archive = new FileSignature("", "document/archive", new string[]
        {
            "pptx", "docx", "xlsx", "zip", "nupkg", "whl", "vsix", "potx", "jar","dxs", "apk"
        }, new[]
        {
            C("50 4B 03 04"), // PK34
        });

        public static FileSignature Pdf = new FileSignature("pdf", "doc/pdf", new string[] { }, new[]
        {
            C("25 50 44 46 2D"), // %PDF-
        });

        public static FileSignature Rtf = new FileSignature("rtf", "doc/rtf", new string[] { }, new[]
        {
            C("7B 5C 72 74 66 31 5C"), // {\rtf1\
        });

        public static FileSignature UTF8 = new FileSignature("", "text/utf8", _textfileExtensions.Union(_xmlExtensions), new[]
        {
            C("EF BB BF"),
        });
        public static FileSignature UTF16 = new FileSignature("", "text/utf16", _textfileExtensions.Union(_xmlExtensions), new[]
        {
            C("FF FE"),
            C("FE FF"),
        });
        public static FileSignature UTF32 = new FileSignature("", "text/utf32", _textfileExtensions.Union(_xmlExtensions), new[]
        {
            C("FF FE 00 00"),
            C("00 00 FE FF"),
        });
        public static FileSignature Png = new FileSignature("png", "image/png", new string[] { }, new[]
        {
            C("89 50 4E 47 0D 0A 1A 0A"),
        });

        public static FileSignature Xml = new FileSignature("", "text/xml", _xmlExtensions, new[]
        {
            C("3C 3F 78 6D 6C 20"), // <?xml
        });
        public static FileSignature XmlStylesheet = new FileSignature("xsl", "text/xsl", new string[] { }, new[]
        {
            C("3C 78 73 6C 3A 73 74 79 6C 65 73 68"), // <xsl:stylesh
        });
        public static FileSignature Noteworthy = new FileSignature("nwctxt", "Noteworthy", new string[] { "nwctxt.bak" }, new[]
        {
            C("21 4E 6F 74 65 57 6F 72 74 68 79"), // !NoteWorthy
        });

        public static FileSignature Shellfile = new FileSignature("", "shellfile", new string[] { "sh", "bash" }, new[]
        {
            C("23 21 20 2F 62 69 6E 2F"), // #! /bin/
            C("23 21 2F 75 73 72 2F 62 69 6E 2F"), // #!/usr/bin/
        });
        public static FileSignature Gif = new FileSignature("gif", "image/gif", new string[] { }, new[]
        {
            C("47 49 46 38 39 61"), // GIF89a
        });

        public static FileSignature Html = new FileSignature("html", "text/html", new[] { "htm" }, new[]
        {
            C("3C 21 44 4F 43 54 59 50 45 20 68 74 6D 6C"), // <!DOCTYPE html
            C("3C 21 44 4F 43 54 59 50 45 20 48 54 4D 4C"), // <!DOCTYPE HTML
        });

        public static FileSignature publicKey = new FileSignature("", "text/pgp", new[] { "pub", "sign", "pgp", "signture" }, new[]
             {
            C("2D 2D 2D 2D 2D 42 45 47 49 4E 20 50 55 42 4C 49 43 20 4B 45 59 2D 2D 2D 2D 2D"), // -----BEGIN PUBLIC KEY-----
            
        });
        public static FileSignature privateKey = new FileSignature("", "text/pgp", new[] { "priv", "key"}, new[]
             {
            C("2D 2D 2D 2D 2D 42 45 47 49 4E 20 52 53 41 20 50 52 49 56 41 54 45 20 4B 45 59 2D 2D 2D 2D 2D"), // -----BEGIN RSA PRIVATE KEY-----
            
        });

        public static FileSignature Mp4 = new FileSignature("mp4", "video/mp4", new string[] { }, new[]
        {
            C("66 74 79 70 6D 6D 70 34"), // ftypmmp4 
            C("66 74 79 70 6D 70 34 32"), // ftypmp42 
            C("66 74 79 70 69 73 6F 6D"), // ftypisom 
        }, 4);
        public static IEnumerable<FileSignature> AllSignatures = new FileSignature[]
        {
            Jpg, Avi, Avif, Mov, Mp4, Png, Gif,
            UTF16, UTF32, UTF8, 
            Pdf, Rtf, Archive, Html, Xml,
            XmlStylesheet,
            Noteworthy, 
            Shellfile, 
            Elf, 
            WinReg, 
            publicKey, privateKey
        };

        public static int LongestSig()
        {
            return AllSignatures.Max(s => s.Signatures.Max(fs => fs.Length + s.SigOffset));
        }

        private static byte[] C(string signature)
        {
            return signature.Split(' ').Select(s => byte.Parse(s, NumberStyles.AllowHexSpecifier)).ToArray();
        }

        public FileSignature(string extension, string name, IEnumerable<string> alternateExtensions,
            IEnumerable<byte[]> signatures, int sigOffset = 0)
        {
            AlternateExtensions = alternateExtensions.ToHashSet();
            Extension = extension;
            Name = name;
            Signatures = signatures;
            SigOffset = sigOffset;
        }

        public HashSet<string> AlternateExtensions { get; }
        public string Extension { get; }
        public string Name { get; }
        public IEnumerable<byte[]> Signatures { get; }
        public int SigOffset { get; }
    }
}