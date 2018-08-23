using System.IO;
using System.Xml.Serialization;
using Nicologies;

namespace DBRestorer.Ctrl.Domain
{
    public interface IUserPreferencePersist
    {
        void SavePreference(UserPreference pref);
        UserPreference LoadPreference();
    }
    public class UserPreferencePersist : IUserPreferencePersist
    {
        private static readonly string PreferenceFilePath = Path.Combine(PathHelper.ProcessAppDir, "preference.xml");

        public void SavePreference(UserPreference pref)
        {
            using (var stream = File.Open(PreferenceFilePath, FileMode.Create))
            {
                new XmlSerializer(typeof(UserPreference)).Serialize(stream, pref);
            }
        }

        public UserPreference LoadPreference()
        {
            try
            {
                using (var stream = File.OpenRead(PreferenceFilePath))
                {
                    return new XmlSerializer(typeof(UserPreference)).Deserialize(stream) as UserPreference;
                }
            }
            catch
            {
                // ignored
            }
            return new UserPreference();
        }
    }
}