using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace FinalosProjektos
{
    public class FileManager
    {
        private readonly string filePath;
        private readonly string exitFlagPath;

        public FileManager(string fileName)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GambleshData");
            Directory.CreateDirectory(folderPath);
            filePath = Path.Combine(folderPath, fileName);
            exitFlagPath = Path.Combine(folderPath, "exit_flag.txt");           
        }

        public void SaveProgress(GameState gameState)
        {
            string json = JsonConvert.SerializeObject(gameState, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public GameState LoadProgress()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<GameState>(json);
            }
            return new GameState();
        }

        public void NewGame()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void CreateExitFlag()
        {
            File.WriteAllText(exitFlagPath, "ExitedCorrectly");            
        }

        public void DeleteExitFlag()
        {
            if (File.Exists(exitFlagPath))
            {
                File.Delete(exitFlagPath);
            }
        }

        public bool ExitFlagExists()
        {
            return File.Exists(exitFlagPath);
        }
    }
}
