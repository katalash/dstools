using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

/// <summary>
/// An in-unity representation of a Dark Souls MTD, which is linked to the dark souls asset and can be used to generate Unity shaders
/// </summary>
class MTDAssetLink : ScriptableObject
{
    public string VirtualPath = null;

    public string ShaderPath = null;

    /// <summary>
    /// The uv set that lightmaps are stored in *in the flver*. Note that IN Unity, lightmaps are always in UV set one (0 indexed), and uv sets are swapped on import.
    /// -1 indicated that there are no lightmap indices
    /// </summary>
    public int LightmapUVIndex = -1;

    [System.Serializable]
    public class TextureDefinition
    {
        /// <summary>
        /// The binding name used in the flver material parameter section
        /// </summary>
        public string Name;

        /// <summary>
        /// For Sekiro
        /// </summary>
        public string TexturePath = "";

        /// <summary>
        /// UV "number" that is stored in an mtd. It doesn't directly define the UV set for this texture, but can be used to derive it
        /// </summary>
        public int UVNumber;

        /// <summary>
        /// Calculated UV set used when sampling this texture
        /// </summary>
        public int UVIndex;

        /// <summary>
        /// Unknown
        /// </summary>
        public int ShaderDataIndex;

        /// <summary>
        /// Unknown
        /// </summary>
        public int Unk;
    }

    public List<TextureDefinition> Textures = new List<TextureDefinition>();

    public void InitializeFromMTD(MTD mtd, string virtualPath)
    {
        VirtualPath = virtualPath;
        ShaderPath = mtd.ShaderPath;
        Textures = new List<TextureDefinition>();
        foreach (var tex in mtd.Textures)
        {
            var def = new TextureDefinition();
            def.Name = tex.Type;
            def.UVNumber = tex.UVNumber;
            def.ShaderDataIndex = tex.ShaderDataIndex;
            def.Unk = tex.Unk04;
            if (tex.Extended)
            {
                def.TexturePath = tex.Path;
            }

            // Calculate the uv set index
            int uvoffset = 1;
            for (int j = 1; j < tex.UVNumber; j++)
            {
                if (!mtd.Textures.Any(t => (t.UVNumber == j)))
                {
                    uvoffset++;
                }
            }
            def.UVIndex = tex.UVNumber - uvoffset;

            if (tex.Type.ToUpper() == "G_LIGHTMAP" || tex.Type.ToUpper() == "G_DOLTEXTURE1" || tex.Type.ToUpper() == "G_GITEXTURE")
            {
                LightmapUVIndex = def.UVIndex;
            }

            Textures.Add(def);
        }
    }
}
