using System.Linq;
using EyesDown.Settings.SettingBlocks;
using UnityEngine;

namespace Settings.SettingBlocks
{
    public class ResolutionsDropdownSettingBlock : DropdownSettingBlock
    {
        protected override string[] GetValues()
        {
            return Screen.resolutions.Select((x) => $"{x.width}x{x.height}").Distinct().ToArray();
        }
    }
}