using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AVA_LabelEditor.Helper
{
    public class Helper
    {
        public static string Hash(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static bool CheckFailed(string message, string caption = "")
        {
            MessageBox.Show(message,caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

    }
}