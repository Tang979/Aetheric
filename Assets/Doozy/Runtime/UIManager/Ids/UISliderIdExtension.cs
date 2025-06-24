// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Collections.Generic;
// ReSharper disable All
namespace Doozy.Runtime.UIManager.Components
{
    public partial class UISlider
    {
        public static IEnumerable<UISlider> GetSliders(UISliderId.Demo id) => GetSliders(nameof(UISliderId.Demo), id.ToString());
        public static bool SelectSlider(UISliderId.Demo id) => SelectSlider(nameof(UISliderId.Demo), id.ToString());
    }
}

namespace Doozy.Runtime.UIManager
{
    public partial class UISliderId
    {
        public enum Demo
        {
            MainMenu,
            SettingMenu
        }    
    }
}
