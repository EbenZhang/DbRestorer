using System;

namespace DBRestorer.Domain
{
    [Serializable]
    public class UserPreference
    {
        public string LastUsedDbInst { get; set; }
    }
}
