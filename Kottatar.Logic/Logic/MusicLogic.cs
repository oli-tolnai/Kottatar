using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kottatar.Data;
using Kottatar.Entities.Dtos.Music;
using Kottatar.Entities.Entity_Models;
using Kottatar.Logic.Helpers;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Kottatar.Logic.Logic
{
    public class MusicLogic
    {
        private readonly Repository<Music> repo;
        private readonly Repository<Instrument> instRepo;
        private readonly DtoProvider dtoProvider;
        private readonly KottatarContext context;

        public MusicLogic(Repository<Music> repo, DtoProvider dtoProvider, KottatarContext context)
        {
            this.repo = repo;
            this.dtoProvider = dtoProvider;
            this.context = context;
            this.instRepo = new Repository<Instrument>(context);
        }

        public void AddMusic(MusicCreateUpdateDto dto)
        {
            Music m = dtoProvider.Mapper.Map<Music>(dto);

            if (repo.GetAll().FirstOrDefault(m => m.Title == dto.Title) == null)
            {
                repo.Create(m);
            }
            else
            {
                throw new ArgumentException("Music with this title already exists");
            }
        }

        public IEnumerable<MusicShortViewDto> GetAllMusic()
        {
            return repo.GetAll().Select(x =>
                dtoProvider.Mapper.Map<MusicShortViewDto>(x)
            );
        }

        public void DeleteMusic(string id)
        {
            repo.DeleteById(id);
        }

        public void UpdateMusic(string id, MusicCreateUpdateDto dto)
        {
            Music oldm = repo.FindById(id);
            dtoProvider.Mapper.Map(dto, oldm);
            repo.Update(oldm);
        }

        public MusicViewDto GetMusic(string id)
        {
            Music mModel = repo.FindById(id);
            return dtoProvider.Mapper.Map<MusicViewDto>(mModel);
        }

        public void UpdateDatabase(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }

            try
            {
                // Try to update incrementally
                SyncWithFileSystem(path);
            }
            catch (Exception ex)
            {
                // If anything fails, recreate the database from scratch
                RecreateDatabase(path);
            }
        }

        private void SyncWithFileSystem(string path)
        {
            var musicDirectories = Directory.GetDirectories(path);
            var musicFolderNames = musicDirectories
                .Select(dir => new DirectoryInfo(dir).Name)
                .ToHashSet();

            var existingMusicRecords = repo.GetAll().ToList();
            
            // Track changes for diagnostics
            int musicAdded = 0;
            int musicUpdated = 0;
            int musicRemoved = 0;
            int instrumentsAdded = 0;
            int instrumentsUpdated = 0;
            int instrumentsRemoved = 0;
            
            try
            {
                // Process removed music folders
                var musicToRemove = existingMusicRecords
                    .Where(music => !musicFolderNames.Contains(music.Title))
                    .ToList();
                
                foreach (var music in musicToRemove)
                {
                    try
                    {
                        // Delete all associated instruments first
                        var instruments = instRepo.GetAll().Where(i => i.MusicId == music.Id).ToList();
                        foreach (var instrument in instruments)
                        {
                            instRepo.Delete(instrument);
                            instrumentsRemoved++;
                        }
                        
                        // Then delete the music entry
                        repo.Delete(music);
                        musicRemoved++;
                    }
                    catch (Exception ex)
                    {
                        // Log but continue with other entries
                        //Console.WriteLine($"Error removing music '{music.Title}': {ex.Message}");
                        throw new Exception($"Failed to remove music '{music.Title}': {ex.Message}", ex);
                    }
                }
                
                // Process existing and new music folders
                foreach (var dir in musicDirectories)
                {
                    try
                    {
                        var dirName = new DirectoryInfo(dir).Name;
                        var existingMusic = existingMusicRecords.FirstOrDefault(m => m.Title == dirName);
                        
                        // Find the sheet music (PDF file)
                        var pdfFiles = Directory.GetFiles(dir, "*.pdf");
                        
                        // Find audio files to check if the folder should be processed
                        var audioFiles = Directory.GetFiles(dir, "*.mp3")
                            .Concat(Directory.GetFiles(dir, "*.wav"))
                            .ToArray();
                        
                        // Skip folders with no content (neither PDFs nor audio files)
                        if (pdfFiles.Length == 0 && audioFiles.Length == 0)
                        {
                            // If the folder exists in the database but has no files now, remove it
                            if (existingMusic != null)
                            {
                                // Delete all associated instruments first
                                var instruments = instRepo.GetAll().Where(i => i.MusicId == existingMusic.Id).ToList();
                                foreach (var instrument in instruments)
                                {
                                    instRepo.Delete(instrument);
                                    instrumentsRemoved++;
                                }
                                
                                // Then delete the music entry
                                repo.Delete(existingMusic);
                                musicRemoved++;
                                //Console.WriteLine($"Removed music '{dirName}' because folder is empty.");
                            }
                            continue; // Skip further processing for this folder
                        }
                        
                        // Determine sheet music file path - empty string if no PDF exists
                        string sheetMusicFile = pdfFiles.Length > 0 ? pdfFiles[0] : string.Empty;
                        
                        if (existingMusic == null)
                        {
                            // Create new music entry
                            var music = new Music(dirName, sheetMusicFile);
                            repo.Create(music);
                            musicAdded++;
                            
                            // Add instruments
                            instrumentsAdded += AddInstrumentsForMusic(dir, music.Id);
                        }
                        else
                        {
                            bool updated = false;
                            
                            // Update the sheet music file if needed
                            // If both are empty, no change needed
                            // If both exist but different path, update
                            // If one is empty and the other not, update
                            if (existingMusic.SheetMusicFile != sheetMusicFile)
                            {
                                existingMusic.SheetMusicFile = sheetMusicFile;
                                updated = true;
                            }
                            // If SheetMusicFile is not empty, check if it exists
                            else if (!string.IsNullOrEmpty(existingMusic.SheetMusicFile) && !File.Exists(existingMusic.SheetMusicFile))
                            {
                                existingMusic.SheetMusicFile = sheetMusicFile;
                                updated = true;
                            }
                            
                            if (updated)
                            {
                                repo.Update(existingMusic);
                                musicUpdated++;
                            }
                            
                            // Update instruments - count changes
                            var changes = UpdateInstrumentsForMusic(dir, existingMusic.Id);
                            instrumentsAdded += changes.added;
                            instrumentsUpdated += changes.updated;
                            instrumentsRemoved += changes.removed;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue with other folders
                        //Console.WriteLine($"Error processing directory '{dir}': {ex.Message}");
                        throw new Exception($"Failed to process directory '{dir}': {ex.Message}", ex);
                    }
                }
                
                // Additional check: Verify all files in the database actually exist
                VerifyDatabaseFileReferences();
                
                // Log changes for diagnostics
                //Console.WriteLine($"Database sync completed: Music: +{musicAdded} ~{musicUpdated} -{musicRemoved}, " +
                //                 $"Instruments: +{instrumentsAdded} ~{instrumentsUpdated} -{instrumentsRemoved}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to sync database with file system: {ex.Message}", ex);
            }
        }
        
        private void VerifyDatabaseFileReferences()
        {
            // Check all music entries to make sure their sheet music files exist if they're not empty
            var musicEntries = repo.GetAll().ToList();
            foreach (var music in musicEntries)
            {
                if (!string.IsNullOrEmpty(music.SheetMusicFile) && !File.Exists(music.SheetMusicFile))
                {
                    // Sheet music file doesn't exist anymore - set it to empty string
                    music.SheetMusicFile = string.Empty;
                    repo.Update(music);
                    //Console.WriteLine($"Updated music '{music.Title}' with empty sheet music path because the file no longer exists.");
                }
            }
            
            // Check all instrument entries to make sure their audio files exist
            var instrumentEntries = instRepo.GetAll().ToList();
            foreach (var instrument in instrumentEntries)
            {
                if (!File.Exists(instrument.AuidoFile))
                {
                    // Audio file doesn't exist anymore
                    instRepo.Delete(instrument);
                    //Console.WriteLine($"Removed instrument '{instrument.Type}' because its audio file no longer exists.");
                }
            }
            
            // Check for music entries with no instruments and remove them
            foreach (var music in musicEntries)
            {
                var instruments = instRepo.GetAll().Where(i => i.MusicId == music.Id).ToList();
                if (instruments.Count == 0 && string.IsNullOrEmpty(music.SheetMusicFile))
                {
                    // Music has no instruments and no sheet music file - delete it
                    repo.Delete(music);
                    //Console.WriteLine($"Removed music '{music.Title}' because it has no instruments and no sheet music file.");
                }
            }
        }

        private void RecreateDatabase(string path)
        {
            try
            {
                // Drop and recreate the database
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                
                // Get all music folders
                var directories = Directory.GetDirectories(path);
                int musicCreated = 0;
                int instrumentsCreated = 0;
                
                foreach (var dir in directories)
                {
                    try
                    {
                        var dirName = new DirectoryInfo(dir).Name;
                        
                        // Find the sheet music (PDF file)
                        var pdfFiles = Directory.GetFiles(dir, "*.pdf");
                        
                        // Find audio files to check if the folder should be processed
                        var audioFiles = Directory.GetFiles(dir, "*.mp3")
                            .Concat(Directory.GetFiles(dir, "*.wav"))
                            .ToArray();
                        
                        // Skip empty folders
                        if (pdfFiles.Length == 0 && audioFiles.Length == 0)
                        {
                            continue;
                        }
                        
                        // Determine sheet music file path - empty string if no PDF exists
                        string sheetMusicFile = pdfFiles.Length > 0 ? pdfFiles[0] : string.Empty;
                        
                        // Create music entry
                        var music = new Music(dirName, sheetMusicFile);
                        repo.Create(music);
                        musicCreated++;
                        
                        // Add instruments
                        instrumentsCreated += AddInstrumentsForMusic(dir, music.Id);
                    }
                    catch (Exception ex)
                    {
                        // Log but continue with other folders
                        //Console.WriteLine($"Error processing directory '{dir}' during recreation: {ex.Message}");
                        throw new Exception($"Failed to process directory '{dir}' during database recreation: {ex.Message}", ex);
                    }
                }
                
                //Console.WriteLine($"Database recreated: {musicCreated} music entries and {instrumentsCreated} instruments created");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to recreate database: {ex.Message}", ex);
            }
        }

        private int AddInstrumentsForMusic(string musicFolderPath, string musicId)
        {
            int count = 0;
            
            try
            {
                // Find all audio files
                var audioFiles = Directory.GetFiles(musicFolderPath, "*.mp3")
                    .Concat(Directory.GetFiles(musicFolderPath, "*.wav"))
                    .ToArray();
                
                foreach (var audioFile in audioFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(audioFile);
                    
                    // Create instrument with type = file name (without extension)
                    var instrument = new Instrument(musicId, fileName, audioFile);
                    instRepo.Create(instrument);
                    count++;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error adding instruments for music ID {musicId}: {ex.Message}");
                // Continue despite errors
                throw new Exception($"Failed to add instruments for music ID {musicId}: {ex.Message}", ex);
            }
            
            return count;
        }

        private (int added, int updated, int removed) UpdateInstrumentsForMusic(string musicFolderPath, string musicId)
        {
            int added = 0;
            int updated = 0;
            int removed = 0;
            
            try
            {
                // Get current instruments for this music
                var currentInstruments = instRepo.GetAll().Where(i => i.MusicId == musicId).ToList();
                
                // Find all audio files
                var audioFiles = Directory.GetFiles(musicFolderPath, "*.mp3")
                    .Concat(Directory.GetFiles(musicFolderPath, "*.wav"))
                    .ToArray();
                
                // Create a dictionary of audio file names for faster lookups
                var audioFileNames = audioFiles.ToDictionary(
                    file => Path.GetFileNameWithoutExtension(file),
                    file => file
                );
                
                // Track processed instruments to identify which should be removed
                var processedInstrumentIds = new HashSet<string>();
                
                // Update existing instruments
                foreach (var instrument in currentInstruments)
                {
                    if (audioFileNames.TryGetValue(instrument.Type, out var audioFile))
                    {
                        // Update existing instrument if needed
                        if (instrument.AuidoFile != audioFile)
                        {
                            instrument.AuidoFile = audioFile;
                            instRepo.Update(instrument);
                            updated++;
                        }
                        processedInstrumentIds.Add(instrument.Id);
                        audioFileNames.Remove(instrument.Type); // Remove from dictionary to track new ones
                    }
                }
                
                // Add new instruments
                foreach (var kvp in audioFileNames)
                {
                    var instrument = new Instrument(musicId, kvp.Key, kvp.Value);
                    instRepo.Create(instrument);
                    added++;
                }
                
                // Remove instruments that no longer exist in the folder
                foreach (var instrument in currentInstruments)
                {
                    if (!processedInstrumentIds.Contains(instrument.Id))
                    {
                        instRepo.Delete(instrument);
                        removed++;
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error updating instruments for music ID {musicId}: {ex.Message}");
                // Continue despite errors
                throw new Exception($"Failed to update instruments for music ID {musicId}: {ex.Message}", ex);
            }
            
            return (added, updated, removed);
        }
    }
}
