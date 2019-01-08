using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// An "asset" that is associated by name to a unity asset that defines a mesh, and is used to export it to a flver along with some export settings
/// </summary>
class FLVERAssetLink : ScriptableObject
{
    /// <summary>
    /// Defines the container/archive that the exported flver will be exported into
    /// </summary>
    public enum ContainerType
    {
        None,
        Mapbnd,
        Objbnd,
        Chrbnd,
    }

    public ContainerType Type = ContainerType.None;

    /// <summary>
    /// Interroot relative path to the archive that this asset is stored into
    /// </summary>
    public string ArchivePath = null;

    /// <summary>
    /// Path to the Flver. Either a virtual path for archived flvers, or an interroot relative path for naked flvers
    /// </summary>
    public string FlverPath = null;

    /// <summary>
    /// Triggers a full from scratch export of the flver. Note that this may break existing game meshes and should only be used for full new models.
    /// Partial submesh exports should be used for existing game models.
    /// </summary>
    public bool FullExport = false;

    /// <summary>
    /// Stores metadata about all the flver submeshes
    /// </summary>
    [System.Serializable]
    public class SubmeshInfo
    {
        /// <summary>
        /// Internal name for the submesh
        /// </summary>
        public string Name;

        /// <summary>
        /// Link the MTD asset link
        /// </summary>
        public MTDAssetLink Mtd;

        /// <summary>
        /// Describes the level of exporting this submesh. Lightmap UV only preserves everything else and tries to export the lightmap uvs only
        /// </summary>
        public enum MeshExportMode
        {
            NoExport,
            LightmapUVOnly,
            FullExport,
        }

        public MeshExportMode ExportMode = MeshExportMode.NoExport;
    }

    public List<SubmeshInfo> Submeshes = new List<SubmeshInfo>();
}

