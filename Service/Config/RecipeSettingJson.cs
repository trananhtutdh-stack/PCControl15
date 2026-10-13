using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PCControl.Service.Config
{
    public class RecipeSettingJson
    {
        public string SETTING_FILE_NAME { get; private set; } = "RecipeSetting.json";

        public RecipeSetting CurrentRecipe { get; set; } = new RecipeSetting();
        public ObservableCollection<RecipeSetting> List { get; set; } = [];

        public string TOJSON()
        {
            return JsonConvert.SerializeObject(this);

        }
        public RecipeSettingJson FromJSON(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<RecipeSettingJson>(json) ?? new RecipeSettingJson();
            }
            catch (Exception ex)
            {

                return new RecipeSettingJson();
            }
        }
    }
}
