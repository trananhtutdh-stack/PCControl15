using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
namespace PCControl.Service.Config
{
    public partial class AppSettingJson : ObservableObject
    {
        public string SETTING_FILE_NAME { get;private set; } = "AppSetting.json";

        public string CurrentCultureName{ get; set; } = "en";


        public string TOJSON()
        {
            return JsonConvert.SerializeObject(this);
            
        }
        public AppSettingJson FromJSON(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<AppSettingJson>(json) ?? new AppSettingJson();
            }
            catch (Exception ex)
            {
                return new AppSettingJson();
            }
        }
    }
}
