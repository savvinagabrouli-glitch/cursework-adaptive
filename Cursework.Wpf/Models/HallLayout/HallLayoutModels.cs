using System;
using System.Collections.Generic;

namespace Cursework.Wpf.Models.HallLayout
{
    public class HallTableLayoutModel
    {
        public int TableId { get; set; }
        public string Zone { get; set; } = string.Empty;

        public double X { get; set; }
        public double Y { get; set; }

        public string ModelType { get; set; } = "Single";
    }

    public class HallLayoutPresetModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string Name { get; set; } = string.Empty;


        public string Zone { get; set; } = string.Empty;


        public bool IsActive { get; set; }

        public List<HallTableLayoutModel> Tables { get; set; } = new();
    }


    public class HallLayoutsFileModel
    {
        public List<HallLayoutPresetModel> Presets { get; set; } = new();
    }
}
