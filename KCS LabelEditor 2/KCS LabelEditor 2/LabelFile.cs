﻿using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KCS_LabelEditor_2
{
    public class LabelFile
    {

        public string Name => System.IO.Path.GetFileName(Path);
        public string Hash { get; }
        public string Path { get; }
        public Language Language { get; }

        public LabelFile(string path, Language language)
        {
            Path = path;
            Hash = Helper.Hash(File.ReadAllText(Path));
            Language = language;
        }

    }
}