using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FMGBND
{
    public enum FmgType : int
    {
        //Item
        ItemNames = 10,
        WeaponNames = 11,
        ArmorNames = 12,
        RingNames = 13,
        MagicNames = 14,
        FeatureNames = 15,
        FeatureDescriptions = 16,
        FeatureLongDescriptions = 17,
        NPCNames = 18,
        PlaceNames = 19,
        ItemDescriptions = 20,
        WeaponDescriptions = 21,
        ArmorDescriptions = 22,
        RingDescriptions = 23,
        ItemLongDescriptions = 24,
        WeaponLongDescriptions = 25,
        ArmorLongDescriptions = 26,
        RingLongDescriptions = 27,
        MagicDescriptions = 28,
        MagicLongDescriptions = 29,

        //Item Patch
        ItemLongDescriptions_Patch = 100,
        MagicLongDescriptions_Patch = 105,
        WeaponLongDescriptions_Patch = 106,
        ArmorLongDescriptions_Patch = 108,
        RingLongDescriptions_Patch = 109,
        ItemDescriptions_Patch = 110,
        ItemNames_Patch = 111,
        RingDescriptions_Patch = 112,
        RingNames_Patch = 113,
        WeaponDescriptions_Patch = 114,
        WeaponNames_Patch = 115,
        ArmorDescriptions_Patch = 116,
        ArmorNames_Patch = 117,
        MagicNames_Patch = 118,
        NPCNames_Patch = 119,
        PlaceNames_Patch = 120,

        //Menu
        Conversation = 1,
        SoapstoneMessages = 2,
        MovieSubtitles = 3,
        EventText = 30,
        MenuIngame = 70,
        MenuGeneralText = 76,
        MenuOther = 77,
        MenuDialog = 78,
        MenuKeyGuide = 79,
        MenuSingleLineHelp = 80,
        MenuItemHelp = 81,
        MenuTextDisplayTagList = 90,
        MenuSystemSpecificTagsWin32 = 91,
        MenuSystemMessageWin32 = 92,

        //Menu Patch
        EventText_Patch = 101,
        MenuDialog_Patch = 102,
        MenuSystemMessageWin32_Patch = 103,
        Conversation_Patch = 104,
        SoapstoneMessages_Patch = 107,
        MenuSingleLineHelp_Patch = 121,
        MenuKeyGuide_Patch = 122,
        MenuOther_Patch = 123,
        MenuGeneralText_Patch = 124,
    }
}
