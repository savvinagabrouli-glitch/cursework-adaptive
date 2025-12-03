using System.Collections.Generic;
using Cursework.Wpf.Models.HallLayout;

namespace Cursework.Wpf.Services.HallLayout
{

    public interface IHallLayoutStorageService
    {

        List<HallLayoutPresetModel> LoadPresets();


        void SavePresets(List<HallLayoutPresetModel> presets);


        List<HallLayoutPresetModel> GetPresetsForZone(string zone);


        HallLayoutPresetModel? LoadPreset(string zone, string presetIdOrName);


        void SavePreset(HallLayoutPresetModel preset);


        void DeletePreset(string presetId);
        string? GetActivePresetName(string zoneCode);
        void SetActivePresetName(string zoneCode, string presetIdOrName);
    }
}
