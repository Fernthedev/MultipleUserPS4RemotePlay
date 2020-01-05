using System;
using System.Collections.Generic;
using System.IO;

namespace PS4RemotePlayMultipleUser {
    class Program {
        private static string docPath;
        private static string ps4Path;
        private static DirectoryInfo docFolder;
        private static List<DirectoryInfo> userFolders;
        private static DirectoryInfo ps4Folder;

        static void Main(string[] args) {
            scan();
            Console.WriteLine("Select using numbers, or n to create a new profile based on the current one selected in PS4 Remote Play, or exit to close.\nUse scan to refresh");

            bool running = true;

            while(running) {
                string line = Console.ReadLine();

                if(line.ToLower().Equals("n")) {
                    Console.Write("What is the name of this new profile? ");
                    bool requiredNewName = true;

                    DirectoryInfo newFolder;
                    string name = null;

                    while (requiredNewName) {
                        name = Console.ReadLine();

                        if(name == " " || name == "") {
                            Console.WriteLine("Name cannot be empty");
                            continue;
                        }

                        newFolder = new DirectoryInfo(docPath + "\\" + name);
                        if (newFolder.Exists) {
                            bool validated = validateFolder(newFolder);
                            if (validated) {
                                Console.WriteLine("Name is taken already. Please choose another one");
                                continue;
                            }
                        }

                        requiredNewName = false;
                    }
                    newFolder = docFolder.CreateSubdirectory(name);

                    

                    copyFolder(ps4Folder, newFolder.FullName, true);
                    Console.WriteLine("Sucessfully created new profile");
                }

                if(line.ToLower().Equals("exit")) {
                    Console.WriteLine("Exiting");
                    return;
                }

                if(line.ToLower().Equals("scan") || line.ToLower().Equals("list")) {
                    scan();
                }

                if(int.TryParse(line, out int indexFile)) {
                    if(userFolders.Count - 1 < indexFile) {
                        Console.WriteLine("Number exceeds registered user accounts. Index starts at 0");
                        continue;
                    }

                    DirectoryInfo backup = docFolder.CreateSubdirectory("backup");
                    copyFolder(ps4Folder, backup.FullName, true);
                    Console.WriteLine("Created backup");

                    DirectoryInfo dir = userFolders[indexFile];
                    copyFolder(dir, ps4Folder.FullName, true);
                    Console.WriteLine("Successfully selected current user profile");
                    
                }
            }
            
        }
        static void copyFolder(DirectoryInfo sourceFolder, string destFolder, bool overwrite) {
            foreach (FileInfo file in sourceFolder.GetFiles()) {
                File.Copy(file.FullName, destFolder + "\\" + file.Name, overwrite);
            }
        }

        static void scan()
        {
            // Init variables
            docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MultiUserPS4";
            ps4Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Sony Corporation\\PS4 Remote Play";

            docFolder = new DirectoryInfo(@docPath);
            ps4Folder = new DirectoryInfo(@ps4Path);
            if (!ps4Folder.Exists)
            {
                Console.WriteLine("Error: Cannot find PS4 Remote play folder. Do you have it installed?");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }

            if (!docFolder.Exists)
            {
                docFolder.Create();
            }


            DirectoryInfo[] Files = docFolder.GetDirectories("*"); //Getting folders

            userFolders = new List<DirectoryInfo>(); // The folders with users

            // Check all the folders within the document folder named MultiUserPS4
            foreach (DirectoryInfo file in Files)
            {

                // Check if the folder contains 2 required files
                bool bothMet = validateFolder(file);

                // If both requirements are met, add them to the registry.
                if (bothMet)
                {
                    userFolders.Add(file);
                }
            }

            Console.WriteLine("Found " + userFolders.Count + " valid user accounts");

            foreach (DirectoryInfo file in userFolders)
            {
                Console.WriteLine(" " + userFolders.IndexOf(file) + ": " + file.Name);
            }
        }

        static bool validateFolder(DirectoryInfo folder) {
            bool existsBin = false;
            bool existsSettings = false;
            foreach (FileInfo file1 in folder.GetFiles()) {
                if (file1.Name == "data.bin") {
                    existsBin = true;
                }
                if (file1.Name == "setting.cache") {
                    existsSettings = true;
                }
            }
            return existsBin && existsSettings;
        }
    }
}
