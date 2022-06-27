using UnityEngine;
using System.IO;
using UnityEditor.AssetImporters;
using System;

[ScriptedImporter(1, "bin")]
public class BinImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
//        BinaryAsset binAsset = ScriptableObject.CreateInstance<BinaryAsset>();
        BinaryAsset binAsset = new BinaryAsset();
        binAsset.byteArray = File.ReadAllBytes(ctx.assetPath);
        ctx.AddObjectToAsset("Bin Asset", binAsset);
        ctx.SetMainObject(binAsset);
    }

}
