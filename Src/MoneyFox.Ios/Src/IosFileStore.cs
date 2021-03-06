﻿namespace MoneyFox.iOS.Src
{
    public class IosFileStore : FileStoreIoBase
    {
        public IosFileStore(string basePath) : base(basePath)
        {
        }

        private const string ResScheme = "res:";

        protected override string AppendPath(string path)
        {
            if(path.StartsWith(ResScheme))
            {
                return path.Substring(ResScheme.Length);
            }

            return base.AppendPath(path);
        }
    }
}
