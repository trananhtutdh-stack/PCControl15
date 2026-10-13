using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PCControl.Core.IPC;
using PCControl.Service.Config;

namespace PCControl.Machine
{
    public static class PosControl
    {
        public static RecipeSetting Recipe => App.Services.GetRequiredService<Config>().recipeSettingJson.CurrentRecipe;

        public static PosSetting CH1_ROBOTZ_WAIT_POS = Recipe.PosData[0];
        public static PosSetting CH1_ROBOTZ_DOWN_JIG_POS = Recipe.PosData[1];
        public static PosSetting CH1_ROBOTZ_DOWN_TRAY_POS = Recipe.PosData[2];
        public static PosSetting CH1_ROBOTX_1ST_TOOL1_POS = Recipe.PosData[3];
        public static PosSetting CH1_ROBOTY_1ST_TOOL1_POS = Recipe.PosData[4];
        public static PosSetting CH1_ROBOTX_1ST_TOOL2_POS = Recipe.PosData[5];
        public static PosSetting CH1_ROBOTY_1ST_TOOL2_POS = Recipe.PosData[6];
        public static PosSetting CH1_ROBOTX_1ST_TOOL3_POS = Recipe.PosData[7];
        public static PosSetting CH1_ROBOTY_1ST_TOOL3_POS = Recipe.PosData[8];
        public static PosSetting CH1_ROBOTX_1ST_TOOL4_POS = Recipe.PosData[9];
        public static PosSetting CH1_ROBOTY_1ST_TOOL4_POS = Recipe.PosData[10];
        public static PosSetting CH1_ROBOTX_1ST_TRAY_POS = Recipe.PosData[11];
        public static PosSetting CH1_ROBOTY_1ST_TRAY_POS = Recipe.PosData[12];

        public static PosSetting CH2_ROBOTZ_WAIT_POS = Recipe.PosData[13];
        public static PosSetting CH2_ROBOTZ_DOWN_POS = Recipe.PosData[14];
        public static PosSetting CH2_ROBOTX_1ST_TOOL1_POS = Recipe.PosData[15];
        public static PosSetting CH2_ROBOTY_1ST_TOOL1_POS = Recipe.PosData[16];
        public static PosSetting CH2_ROBOTX_1ST_TOOL2_POS = Recipe.PosData[17];
        public static PosSetting CH2_ROBOTY_1ST_TOOL2_POS = Recipe.PosData[18];
        public static PosSetting CH2_ROBOTX_1ST_TOOL3_POS = Recipe.PosData[19];
        public static PosSetting CH2_ROBOTY_1ST_TOOL3_POS = Recipe.PosData[20];
        public static PosSetting CH2_ROBOTX_1ST_TOOL4_POS = Recipe.PosData[21];
        public static PosSetting CH2_ROBOTY_1ST_TOOL4_POS = Recipe.PosData[22];
        public static PosSetting CH2_ROBOTX_1ST_TRAY_POS = Recipe.PosData[23];
        public static PosSetting CH2_ROBOTY_1ST_TRAY_POS = Recipe.PosData[24];
        static PosControl()
        {
            //PosBindingValidator.ValidateUniqueBindings(typeof(PosControl));
        }
    }
}
