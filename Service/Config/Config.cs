using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PCControl.Service.Config
{
    public class Config
    {
        public Dictionary<int, string[]> _AlarmList { get; private set; } = new();
        private string AlarmPth => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"alarmList.csv");
        public AppSettingJson appSettingJson { get; set; } = new();
        public RecipeSettingJson recipeSettingJson { get; set; } = new();
        public Config()
        {
            LoadAppSettingJson();
            LoadRecipeSettingJson();
            LoadAlarmList();
        }

        public void SaveAppSettingJson()
        {
            try
            {
                appSettingJson ??= new AppSettingJson();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appSettingJson.SETTING_FILE_NAME);
                File.WriteAllText(path, appSettingJson.TOJSON());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save app settings: {ex.Message}");
            }
        }

        public void LoadAppSettingJson()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), appSettingJson.SETTING_FILE_NAME);
            if(File.Exists(path)) 
            {
                var json = File.ReadAllText(path);
                appSettingJson = appSettingJson.FromJSON(json);
            }
            else
            {
                appSettingJson = new AppSettingJson();
            }
            
        }
        private void LoadAlarmList()
        {
            
        }

        public void SaveRecipeSettingJson()
        {
            try
            {
                recipeSettingJson ??= new RecipeSettingJson();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recipeSettingJson.SETTING_FILE_NAME);
                File.WriteAllText(path, recipeSettingJson.TOJSON());
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Failed to save recipe settings: {ex.Message}");
            }
        }
        private void LoadRecipeSettingJson()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), recipeSettingJson.SETTING_FILE_NAME);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                recipeSettingJson = recipeSettingJson.FromJSON(json);
            }
            else
            {
                recipeSettingJson = new RecipeSettingJson();
            }
        }

        
    }

    
}
